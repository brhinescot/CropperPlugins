// Twitter/Plugin.cs
//
// Code for a cropper plugin that sends a screen snap to
// Twitter.com.
//
// To enable tracing for this DLL, build like so:
//    msbuild /p:Platform=x86 /p:DefineConstants=Trace
//
//
// Dino Chiesa
// Sat, 22 Oct 2011  13:36
//

// Cropper workitem 14970
#define HACK

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Common;       // for Tracing
using CropperPlugins.OAuth;

namespace Cropper.SendToTwitter
{
    public class Plugin :
        DesignablePluginThatUsesFetchOutputStream,
        IConfigurablePlugin
#if HACK
#else
        , CropperPlugins.Common.IUpload
#endif
    {
        public override string Description
        {
            get { return "Send to Twitter"; }
        }

        public override string Extension
        {
            get { return PluginSettings.ImageFormat; }
        }

        public override string ToString()
        {
            return "Send to Twitter [Dino Chiesa]";
        }

        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
        }

        private bool VerifyAuthentication()
        {
            if (!PluginSettings.Completed)
            {
                var dlg = new CropperPlugins.OAuth.TwitterOauthForm(this.oauth);
                dlg.ShowDialog();
                if (dlg.DialogResult == DialogResult.OK)
                {
                    Tracing.Trace("dlg.OK, Storing tokens...");
                    dlg.StoreTokens(PluginSettings);
                }
            }

            if (!PluginSettings.Completed)
            {
                MessageBox.Show("You must grant approval for this plugin with Twitter\n" +
                                "before uploading an image to Twitter.\n\n",
                                "No Authorizaiton for Tweet plugin",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return false;
            }
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
                Tracing.Trace("TwitPic::{0:X8}::SaveImage ({1})",
                              this.GetHashCode(), _fileName);

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
                                "Upload to TwitPic failed",
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



        /// <summary>
        ///   TwitPic supports only PNG and JPG and GIF. This plugin does
        ///   only PNG and JPG.
        /// </summary>
        private ImageFormat DesiredImageFormat
        {
            get
            {
                if (String.Compare(Extension, "jpg", true) == 0)
                {
                    return ImageFormat.Jpeg;
                }
                else
                {
                    return ImageFormat.Png;
                }
            }
        }



        private void SaveImageInDesiredFormat(Stream stream, System.Drawing.Image image)
        {
            Tracing.Trace("TwitPic::{0:X8}::SaveImageInDesiredFormat", this.GetHashCode());
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


        private string GetTweet()
        {
            if (PluginSettings.Tweet)
            {
                // prompt for the tweet here
                var f = new System.Windows.Forms.Form();
                var btnOK = new System.Windows.Forms.Button();
                var btnCancel = new System.Windows.Forms.Button();
                var label = new System.Windows.Forms.Label();
                var txt = new System.Windows.Forms.TextBox();
                //
                // tooltip
                //
                var tooltip = new System.Windows.Forms.ToolTip();
                tooltip.AutoPopDelay = 2400;
                tooltip.InitialDelay = 500;
                tooltip.ReshowDelay = 500;
                label.Text = "Tweet?";
                label.AutoSize = true;
                label.Location = new System.Drawing.Point(4, 6);
                txt.Text = "";
                txt.TabIndex = 11;
                txt.Multiline = true;
                txt.Location = new System.Drawing.Point(54, 8);
                txt.Size = new System.Drawing.Size(268, 82);
                tooltip.SetToolTip(txt, "Your Twitter message");
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
                f.Name = "Tweet";
                f.Text = "What's going on?";
                //f.ClientSize = new System.Drawing.Size(324, 118);
                f.MinimumSize = new System.Drawing.Size(342, 158);
                f.MaximumSize = new System.Drawing.Size(342, 158);
                var result = f.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (!String.IsNullOrEmpty(txt.Text))
                        return txt.Text;
                }
            }

            return  "uploaded from Cropper.";
        }


        public void UploadFile(string fileName)
        {
            this._fileName = fileName;
            UploadImage();
        }


        /// <summary>
        ///  This does the real work of the plugin - uploading to TwitPic.com.
        /// </summary>
        ///
        /// <remarks>
        ///   First upload the image, and then place the raw
        ///   image URL onto the clipboard for easy reference/paste.
        //    Optionally, post a Tweet to Twitter.
        /// </remarks>
        private void UploadImage()
        {
            Tracing.Trace("Tweet::{0:X8}::UploadImage", this.GetHashCode());

            if (!VerifyAuthentication()) return;

            try
            {
                oauth["token"] = PluginSettings.AccessToken;
                oauth["token_secret"] = PluginSettings.AccessSecret;
                string tweet = GetTweet();
                var url = TwitterSettings.URL_UPLOAD;
                var authzHeader = oauth.GenerateAuthzHeader(url, "POST");
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.PreAuthenticate = true;
                request.AllowWriteStreamBuffering = true;
                request.Headers.Add("Authorization", authzHeader);

                string boundary = "~~~~~~" +
                                  Guid.NewGuid().ToString().Substring(18).Replace("-","") +
                                  "~~~~~~";

                var separator = "--" + boundary;
                var footer = "\r\n" + separator + "--\r\n";
                string shortFileName = Path.GetFileName(this._fileName);
                string fileContentType = GetMimeType(shortFileName);
                string fileHeader = string.Format("Content-Disposition: file; " +
                                                  "name=\"media\"; filename=\"{0}\"",
                                                  shortFileName);
                var encoding = System.Text.Encoding.GetEncoding("iso-8859-1");

                var contents = new System.Text.StringBuilder();
                contents.AppendLine(separator);
                contents.AppendLine("Content-Disposition: form-data; name=\"status\"");
                contents.AppendLine();
                contents.AppendLine(tweet);
                contents.AppendLine(separator);
                contents.AppendLine(fileHeader);
                contents.AppendLine(string.Format("Content-Type: {0}", fileContentType));
                contents.AppendLine();

                // actually send the request
                request.ServicePoint.Expect100Continue = false;
                request.ContentType = "multipart/form-data; boundary=" + boundary;

                using (var s = request.GetRequestStream())
                {
                    byte[] bytes = encoding.GetBytes(contents.ToString());
                    s.Write(bytes, 0, bytes.Length);
                    bytes = File.ReadAllBytes(this._fileName);
                    s.Write(bytes, 0, bytes.Length);
                    bytes = encoding.GetBytes(footer);
                    s.Write(bytes, 0, bytes.Length);
                }


                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        MessageBox.Show("There's been a problem trying to tweet:" +
                                        Environment.NewLine +
                                        response.StatusDescription +
                                        Environment.NewLine +
                                        Environment.NewLine +
                                        "You will have to tweet manually." +
                                        Environment.NewLine);

                    }
                    else
                    {
                        // parse the response from Twitter here,
                        // to gran the URL of the tweeted image ?

                    }
                }
            }
            catch (Exception exception2)
            {
                Tracing.Trace("Exception.");
                Tracing.Trace("---------------------------------");
                MessageBox.Show("There's been an exception uploading your image:" +
                                Environment.NewLine +
                                exception2.Message +
                                Environment.NewLine +
                                Environment.NewLine +
                                "You will have to upload this file manually: " +
                                Environment.NewLine +
                                this._fileName,
                                "Failed to upload to TwitPic",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return ;
        }


        private static string GetMimeType(String filename)
        {
            var extension = System.IO.Path.GetExtension(filename).ToLower();
            var regKey =  Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);

            string result =
                ((regKey != null) && (regKey.GetValue("Content Type") != null))
                ? regKey.GetValue("Content Type").ToString()
                : "image/unknown" ;
            return result;
        }

#if Trace
        // these methods are needed only for diagnostic purposes.
        public override void Connect(IPersistableOutput persistableOutput)
        {
            Tracing.Trace("TwitPic::{0:X8}::Connect", this.GetHashCode());
            base.Connect(persistableOutput);
        }

        public override void Disconnect()
        {
            Tracing.Trace("TwitPic::{0:X8}::Disconnect", this.GetHashCode());
            base.Disconnect();
        }

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("TwitPic::{0:X8}::MenuClick", this.GetHashCode());
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
                    _configForm = new TwitterOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            TwitterOptionsForm form = sender as TwitterOptionsForm;
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
            set { PluginSettings = value as TwitterSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private TwitterSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new TwitterSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        #endregion


        private OAuth.Manager _oauth;
        private OAuth.Manager oauth
        {
            get
            {
                if (_oauth == null)
                {
                    _oauth = new OAuth.Manager();
                    _oauth["consumer_key"] = TwitterSettings.TWITTER_CONSUMER_KEY;
                    _oauth["consumer_secret"] = TwitterSettings.TWITTER_CONSUMER_SECRET;
                }
                return _oauth;
            }
        }

        private TwitterSettings _settings;
        private TwitterOptionsForm _configForm;
        private string _fileName;
    }

}

