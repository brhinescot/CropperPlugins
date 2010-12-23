// Picasa/Plugin.cs
//
// Code for a cropper plugin that sends a screen snap to
// Picasa, google's photo sharing service.
//
// To enable tracing for this DLL, build like so:
//    msbuild /p:Platform=x86 /p:DefineConstants=Trace
//
// Dino Chiesa
// Sat, 11 Dec 2010  08:20
//

// #define Trace

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Utils;       // for Tracing
using Microsoft.Http;             // HttpClient, etc

namespace Cropper.SendToPicasa
{
    public class Plugin : DesignablePluginThatUsesFetchOutputStream, IConfigurablePlugin
    {
        public override string Description
        {
            get { return "Send to Picasa"; }
        }

        public override string Extension
        {
            get { return PluginSettings.ImageFormat; }
        }

        public override string ToString()
        {
            return "Send to Picasa [Dino Chiesa]";
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            if (!VerifyBasicSettings()) return;

            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
        }


        private bool VerifyBasicSettings()
        {
            return true;
        }


        /// <summary>
        ///   Saves the captured image to an on-disk file.
        /// </summary>
        ///
        /// <remarks>
        ///   Saving the image to a disk file isn't strictly necessary to enable upload
        ///   of the image via the HTML FORM, but it's nice to have a cached version of
        ///   the image in the filesystem.
        /// </remarks>
        protected override void SaveImage(Stream stream, System.Drawing.Image image)
        {
            bool success = false;
            try
            {
                Tracing.Trace("+--------------------------------");
                Tracing.Trace("Picasa::SaveImage ({0})", _fileName);

                SaveImageInDesiredFormat(stream, image);

                success = true;
            }
            catch (Exception exception1)
            {
                Tracing.Trace("Exception while saving the image: {0}", exception1.Message);
                string msg = "There's been an exception while saving the image: " +
                             exception1.Message + "\n" + exception1.StackTrace;
                msg+= "\n\nYou will have to Upload this file manually: " + this._fileName ;
                MessageBox.Show(msg,
                                "Cropper Plugin for Picasa",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            finally
            {
                image.Dispose();
                stream.Close();
            }

            if (success)
                UploadImage();
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



        private void SaveImageInDesiredFormat(Stream stream, System.Drawing.Image image)
        {
            Tracing.Trace("Picasa::SaveImageInDesiredFormat");
            if (String.Compare(Extension, "jpg", true) == 0)
            {
                SaveImage_Jpg(stream, image);
            }
            else
            {
                image.Save(stream, DesiredImageFormat);
            }
        }


        private void SaveImage_Jpg(Stream stream, System.Drawing.Image image)
        {
            var myEncoder = Encoder.Quality;
            var cInfo = GetEncoderInfo("image/jpeg");
            using (var p1 = new EncoderParameters(1))
            {
                using (var p2 = new EncoderParameter(myEncoder, PluginSettings.JpgImageQuality))
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



        /// <summary>
        ///   Prompts user for summary text for the photo; returns
        ///   that text.
        /// </summary>
        private string GetSummaryText()
        {
            if (PluginSettings.UseFixedComment)
                return PluginSettings.AllPhotosComment;

            // prompt for the "summary" here
            var f = new System.Windows.Forms.Form();
            var btnOK = new System.Windows.Forms.Button();
            var btnCancel = new System.Windows.Forms.Button();
            var label = new System.Windows.Forms.Label();
            var txt = new System.Windows.Forms.TextBox();
            label.Text = "Summary:";
            label.AutoSize = true;
            label.Location = new System.Drawing.Point(4, 6);
            txt.Text = "";
            txt.TabIndex = 11;
            txt.Multiline = true;
            txt.Location = new System.Drawing.Point(62, 8);
            txt.Size = new System.Drawing.Size(260, 82);
            var tooltip = new System.Windows.Forms.ToolTip();
            tooltip.AutoPopDelay = 2400;
            tooltip.InitialDelay = 500;
            tooltip.ReshowDelay = 500;
            tooltip.SetToolTip(txt, "provide a summary description\nof that photo.");
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(250, 94);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(68, 23);
            btnCancel.TabIndex = 71;
            btnCancel.Text = "&Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(174, 94);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(68, 23);
            btnOK.TabIndex = 61;
            btnOK.Text = "&OK";
            btnOK.UseVisualStyleBackColor = true;
            f.Controls.Add(label);
            f.Controls.Add(txt);
            f.Controls.Add(btnOK);
            f.Controls.Add(btnCancel);
            f.Name = "Summary";
            f.Text = "Provide a description of that photo...";
            //f.ClientSize = new System.Drawing.Size(324, 118);
            f.MinimumSize = new System.Drawing.Size(342, 158);
            f.MaximumSize = new System.Drawing.Size(342, 158);
            var result = f.ShowDialog();

            if (result == DialogResult.OK)
                return txt.Text;

            // user has cancelled, so we return "no comment"
            return "";
        }


        /// <summary>
        ///  This does the real work of the plugin - uploading to Picasa.
        /// </summary>
        ///
        /// <remarks>
        ///   First upload the main image, and then place the raw
        ///   image URL onto the clipboard for easy reference/paste.
        /// </remarks>
        private void UploadImage()
        {
            Tracing.Trace("Picasa::UploadImage");

            try
            {
                var address =
                    GdataSession.Authenticate(PluginSettings.EmailAddress, "picasa");

                // reset this in case user has changed it in the authn dialog.
                PluginSettings.EmailAddress = address;

                var headers =
                    GdataSession.Instance.GetHeaders(PluginSettings.EmailAddress, "picasa");

                // user has declined or failed to authenticate
                if (headers == null)
                    return ;

                // Now, upload the photo...
                var summaryText = GetSummaryText();

                var uploadform = new PicasaUploadHttpForm
                    {
                        File = this._fileName,
                        Summary = summaryText
                    };

                var u = String.Format("/data/feed/api/user/default/albumid/{0}",
                                      PluginSettings.Album.Id);

                Tracing.Trace("Upload to album: {0} ({1})",
                              PluginSettings.Album.Name,
                              PluginSettings.Album.Id);

                var uri =  new Uri(u, UriKind.RelativeOrAbsolute);

                // POST the request
                using (var requestMsg =
                       new HttpRequestMessage("POST",
                                              uri,
                                              headers,
                                              uploadform.CreateHttpContent()))
                {
                    using (var picasa = new HttpClient(_basePicasaUrl))
                    {
                        var response = picasa.Send(requestMsg);
                        response.EnsureStatusIsSuccessful();
                        var foo = response.Content.ReadAsXmlSerializable<UploadResponse>();
                        string rawImageUri = foo.content.mediaUrl;
                        if (PluginSettings.PopBrowser)
                            System.Diagnostics.Process.Start(rawImageUri);
                        Clipboard.SetDataObject(rawImageUri, true);
                    }
                }

                Tracing.Trace("all done.");
                Tracing.Trace("---------------------------------");
            }
            catch (Exception exc1)
            {
                Tracing.Trace("Exception. {0}", exc1.Message);
                Tracing.Trace("{0}", exc1.StackTrace);

                Tracing.Trace("---------------------------------");
                MessageBox.Show("There's been an exception uploading your image:" +
                                Environment.NewLine +
                                exc1.Message +
                                Environment.NewLine +
                                Environment.NewLine +
                                "You will have to upload this file manually: " +
                                Environment.NewLine +
                                this._fileName,
                                "Cropper Plugin for Picasa",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return ;
        }


#if Trace
        // these methods are needed only for diagnostic purposes.
        public override void Connect(IPersistableOutput persistableOutput)
        {
            Tracing.Trace("Picasa::Connect");
            base.Connect(persistableOutput);
        }

        public override void Disconnect()
        {
            Tracing.Trace("Picasa::Disconnect");
            base.Disconnect();
        }

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("Picasa::MenuClick");
            base.OnImageFormatClick(sender, e);
        }
#endif


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
                    _configForm = new PicasaOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            Tracing.Trace("Plugin::OptionsSaved");
            PicasaOptionsForm form = sender as PicasaOptionsForm;
            if (form == null) return;
            form.ApplySettings();
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
            set { PluginSettings = value as PicasaSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private PicasaSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new PicasaSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        #endregion

        private PicasaSettings _settings;
        private PicasaOptionsForm _configForm;
        private string _fileName;

        private static readonly string _basePicasaUrl = "https://picasaweb.google.com";
    }



    [XmlType("entry", Namespace="http://www.w3.org/2005/Atom")]
    [XmlRoot("entry", Namespace="http://www.w3.org/2005/Atom")]
    public partial class UploadResponse
    {
        public string id        { get;set; }
        public string title     { get;set; }
        public string summary   { get;set; }
        [XmlElement("content")]
        public UploadContent content   { get;set; }
    }

    [XmlType("content", Namespace="http://www.w3.org/2005/Atom")]
    public partial class UploadContent
    {
        [XmlAttribute("type")]
        public string mimeType   { get;set; }
        [XmlAttribute("src")]
        public string mediaUrl   { get;set; }
    }


}

