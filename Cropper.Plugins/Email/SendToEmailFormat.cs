// SendToEmailFormat.cs
//
// Provides a Cropper Plugin that captures a screenshot, then opens
// (begins composing) an email in the default email client, and attaches
// the captured image to that new email.
//

// Cropper workitem 14970
#define HACK

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using CropperPlugins.Common;       // for Tracing
using Fusion8.Cropper.Extensibility;


namespace Cropper.Email
{
    public class SendToEmailFormat :
        DesignablePlugin,
        IConfigurablePlugin
#if HACK
#else
        , CropperPlugins.Common.IUpload
#endif
    {
        public override string Description
        {
            get { return  "Send to Email"; }
        }

        public override string Extension
        {
            get { return PluginSettings.ImageFormat; }
        }

        /// <summary>
        ///   This shows up in the list of plugins on the Dropdown
        ///   in the plugins tab in the options.... form.
        /// </summary>
        public override string ToString()
        {
            return "Send to e-mail [Jon Galloway]";
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            Tracing.Trace("+--------------+");
            Tracing.Trace("Email::ImageCaptured filename={0}", e.ImageNames.FullSize);
            this.output.FetchOutputStream(new StreamHandler(this.SaveImage), e.ImageNames.FullSize, e.FullSizeImage);
            FileInfo file = new FileInfo(e.ImageNames.FullSize);
            SendImageInEmail(file);
        }

        public void UploadFile(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            SendImageInEmail(file);
        }

        private void SendImageInEmail(FileInfo file)
        {
            string subject = PluginSettings.Subject.ReplaceTokens(file);
            string body = PluginSettings.Message.ReplaceTokens(file);
            MapiMailMessage message = new MapiMailMessage(subject, body);
            message.Files.Add(file.Name);
            message.ShowDialog();
        }

        private void SaveImage(Stream stream, Image image)
        {
            Tracing.Trace("Email::Save");
            if (PluginSettings.ImageFormat == "jpg")
            {
                SaveImage_Jpg(stream, image);
            }
            else
            {
                image.Save(stream, DesiredImageFormat);
            }
        }


        private void SaveImage_Jpg(Stream stream, Image image)
        {
            Encoder myEncoder = Encoder.Quality;
            var cInfo = GetEncoderInfo("image/jpeg");
            using (var p1 = new EncoderParameters(1))
            {
                using (var p2 = new EncoderParameter(myEncoder,
                                                     PluginSettings.JpgImageQuality))
                {
                    p1.Param[0] = p2;
                    image.Save(stream, cInfo, p1);
                }
            }
        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; j++)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }


        private ImageFormat DesiredImageFormat
        {
            get
            {
                if (String.Compare(Extension, "jpg", true) == 0)
                {
                    return ImageFormat.Jpeg;
                }
                else if (String.Compare(Extension, "bmp", true) == 0)
                {
                    return ImageFormat.Bmp;
                }
                else
                {
                    return ImageFormat.Png;
                }
            }
        }


        #region IConfigurablePlugin Implementation

        /// <summary>
        /// Gets the plug-ins impementation of the <see cref="BaseConfigurationForm"/> used
        /// for setting plug-in specific options.
        /// </summary>
        BaseConfigurationForm IConfigurablePlugin.ConfigurationForm
        {
            get
            {
                if (_configForm == null)
                {
                    _configForm = new EmailOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        /// <summary>
        /// Gets a value indicating if the <see cref="ConfigurationForm"/> should
        /// be hosted in the options dialog or shown in its own dialog window.
        /// </summary>
        bool IConfigurablePlugin.HostInOptions
        {
            get { return true; }
        }

        /// <summary>
        /// The settings for this plugin. Required by IConfigurablePlugin.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This property is set during startup with the settings contained in the
        ///   applications configuration file.
        /// </para>
        /// <para>
        ///   The object returned by this property is serialized into the applications
        ///   configuration file on shutdown.
        /// </para>
        /// </remarks>
        public object Settings
        {
            get { return PluginSettings; }
            set { PluginSettings = value as EmailSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private EmailSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new EmailSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        #endregion


        /// <summary>
        ///   Invoked when the options form is hosted in the tabbed UI,
        ///   and the user clicks OK.
        /// </summary>
        private void OptionsSaved(object sender, EventArgs e)
        {
            EmailOptionsForm form = sender as EmailOptionsForm;
            if (form == null) return;
            form.ApplySettings();
        }

        private EmailOptionsForm _configForm;
        private EmailSettings _settings;
    }




    static class Extensions
    {
        public static string ReplaceTokens(this string input, FileInfo file)
        {
            if (!file.Exists)
                throw new System.IO.FileNotFoundException("Temporary file not found", file.FullName);

            string output = input.Replace("$NAME$", file.Name);
            output = output.Replace("$SIZE$", GetFileSizeStringFromContentLength(file.Length));
            output = output.Replace("$OPERATINGSYSTEM$", System.Environment.OSVersion.VersionString);

            foreach (Match m in Regex.Matches(output, @"\$(?<TOKEN>.+?)\$",RegexOptions.Multiline))
            {
                if(m.Groups["TOKEN"] != null)
                    output = output.Replace(m.Value,
                                            System.Environment.GetEnvironmentVariable(m.Groups["TOKEN"].Value));
            }
            return output;
        }

        // http://weblogs.asp.net/jamauss/archive/2005/10/25/428482.aspx
        private static string GetFileSizeStringFromContentLength(long ContentLength)
        {
            if (ContentLength > 1048575)
            {
                return String.Format("{0:0.00 mb}", (Convert.ToDecimal((Convert.ToDouble(ContentLength) / 1024) / 1024)));
            }
            else
            {
                return String.Format("{0:0.00 kb}", (Convert.ToDecimal(Convert.ToDouble(ContentLength) / 1024)));
            }
        }

    }
}
