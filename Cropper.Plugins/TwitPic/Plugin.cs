// TwitPic/Plugin.cs
//
// Code for a cropper plugin that sends a screen snap to
// TwitPic.com, and optionally tweets a message. (updates Twitter status).
//
// To enable tracing for this DLL, build like so:
//    msbuild /p:Platform=x86 /p:DefineConstants=Trace
//
//
// Dino Chiesa
// Sat, 04 Dec 2010  20:58
//

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Utils;       // for Tracing


namespace Cropper.SendToTwitPic
{
    public class Plugin : DesignablePluginThatUsesFetchOutputStream, IConfigurablePlugin
    {
        public override string Description
        {
            get
            {
                return "Send to TwitPic";
            }
        }

        public override string Extension
        {
            get
            {
                return PluginSettings.ImageFormat;
            }
        }

        public override string ToString()
        {
            return "Send to TwitPic [Dino Chiesa]";
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
                var dlg = new TwitPicOauthForm(this.oauth);
                dlg.ShowDialog();
                if (dlg.DialogResult == DialogResult.OK)
                {
                    dlg.StoreTokens(PluginSettings);
                }
            }

            if (!PluginSettings.Completed)
            {
                MessageBox.Show("You must connect to Twitter to approve this plugin\n" +
                                "before uploading an image to TwitPic.\n\n",
                                "No Authorizaiton for TwitPic plugin",
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
                Tracing.Trace("TwitPic::SaveImage ({0})", _fileName);

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
            Tracing.Trace("TwitPic::SaveImageInDesiredFormat");
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
                        label.Text = "Tweet?";
                        label.AutoSize = true;
                        label.Location = new System.Drawing.Point(4, 6);
                        txt.Text = "";
                        txt.TabIndex = 11;
                        txt.Multiline = true;
                        txt.Location = new System.Drawing.Point(54, 8);
                        txt.Size = new System.Drawing.Size(268, 82);
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
            Tracing.Trace("TwitPic::UploadImage");

            if (!VerifyAuthentication()) return;

            try
            {
                oauth["token"] = PluginSettings.AccessToken;
                oauth["token_secret"] = PluginSettings.AccessSecret;
                var authzHeader = oauth.GenerateCredsHeader(TwitPicSettings.URL_VERIFY_CREDS,
                                                            "GET",
                                                            TwitPicSettings.AUTHENTICATION_REALM);
                string tweet = GetTweet();

                // prepare the upload POST request
                var request = (HttpWebRequest)WebRequest.Create(TwitPicSettings.URL_UPLOAD);
                request.Method = "POST";
                request.PreAuthenticate = true;
                request.AllowWriteStreamBuffering = true;
                var boundary = "xxx"+Guid.NewGuid().ToString().Substring(12).Replace("-","");
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                request.Headers.Add("X-Auth-Service-Provider", TwitPicSettings.URL_VERIFY_CREDS);
                request.Headers.Add("X-Verify-Credentials-Authorization",
                                    authzHeader);

                // prepare the payload
                var separator = "--" + boundary;
                var footer = separator + "--";

                var contents = new System.Text.StringBuilder();
                contents.AppendLine(separator);

                contents.AppendLine("Content-Disposition: form-data; name=\"key\"");
                contents.AppendLine();
                contents.AppendLine(TwitPicSettings.TWITPIC_API_KEY);
                contents.AppendLine(separator);

                // THE TWITPIC DOC SAYS that the message parameter is required;
                // it is not. We'll send it anyway.  Keep in mind that posting
                // this message to TwitPic does not "tweet" the message.
                // Apparently the OAuth implementation is not developed enough
                // to do that, yet.
                //
                contents.AppendLine("Content-Disposition: form-data; name=\"message\"");
                contents.AppendLine();
                contents.AppendLine(String.Format("{0} at {1}",
                                                  tweet,
                                                  DateTime.Now.ToString("G")));
                contents.AppendLine(separator);

                string shortFileName = Path.GetFileName(this._fileName);
                string fileContentType = GetMimeType(shortFileName);
                string fileHeader = string.Format("Content-Disposition: file; " +
                                                  "name=\"media\"; filename=\"{0}\"",
                                                  shortFileName);
                // TODO:  make this so I don't have to store
                // all the image data in a single buffer. One option is
                // to do it chunked transfer. need to do the proper encoding in
                // any case.
                var encoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                string fileData = encoding
                    .GetString(File.ReadAllBytes(this._fileName));

                contents.AppendLine(fileHeader);
                contents.AppendLine(string.Format("Content-Type: {0}", fileContentType));
                contents.AppendLine();
                contents.AppendLine(fileData);

                contents.AppendLine(footer);

                byte[] bytes = encoding.GetBytes(contents.ToString());
                request.ContentLength = bytes.Length;

                // actually send the request
                using (var s = request.GetRequestStream())
                {
                    s.Write(bytes, 0, bytes.Length);

                    using (var r = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(r.GetResponseStream()))
                        {
                            var responseText = reader.ReadToEnd();
                            var s1 = new XmlSerializer(typeof(TwitPicUploadResponse));
                            var sr = new System.IO.StringReader(responseText);
                            var tpur= (TwitPicUploadResponse) s1.Deserialize(new System.Xml.XmlTextReader(sr));

                            if (PluginSettings.PopBrowser)
                                System.Diagnostics.Process.Start(tpur.url);

                            Clipboard.SetDataObject(tpur.url, true);

                            if (PluginSettings.Tweet)
                                Tweet(tweet, tpur.url);
                        }
                    }
                }
                Tracing.Trace("all done.");
                Tracing.Trace("---------------------------------");
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

        private void Tweet(string message, string imageUri)
        {
            var twitterUpdateUrlBase = "http://api.twitter.com/1/statuses/update.xml?status=";
            var msg = String.Format("{0} {1}", message, imageUri);
            var url = twitterUpdateUrlBase + OAuth.Manager.UrlEncode(msg);

            var authzHeader = oauth.GenerateAuthzHeader(url, "POST");

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;
            request.Headers.Add("Authorization", authzHeader);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("There's been a problem trying to tweet:" +
                                Environment.NewLine +
                                response.StatusDescription +
                                Environment.NewLine +
                                Environment.NewLine +
                                "You will have to tweet manually." +
                                Environment.NewLine);
            }
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
            Tracing.Trace("TwitPic::Connect");
            base.Connect(persistableOutput);
        }

        public override void Disconnect()
        {
            Tracing.Trace("TwitPic::Disconnect");
            base.Disconnect();
        }

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("TwitPic::MenuClick");
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
                    _configForm = new TwitPicOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            TwitPicOptionsForm form = sender as TwitPicOptionsForm;
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
            set { PluginSettings = value as TwitPicSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private TwitPicSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new TwitPicSettings();
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
                    _oauth["consumer_key"] = TwitPicSettings.TWITTER_CONSUMER_KEY;
                    _oauth["consumer_secret"] = TwitPicSettings.TWITTER_CONSUMER_SECRET;
                }
                return _oauth;
            }
        }

        private TwitPicSettings _settings;
        private TwitPicOptionsForm _configForm;
        private string _fileName;
    }



    // Example of response from http://api.twitpic.com/2/upload.xml :
    //
    // <image>
    //     <id>3fq924</id>
    //     <text />
    //     <url>http://twitpic.com/3fq924</url>
    //     <width>747</width>
    //     <height>158</height>
    //     <size>14318</size>
    //     <type>jpg</type>
    //     <timestamp>Tue, 14 Dec 2010 01:50:38 +0000</timestamp>
    //     <user>
    //         <id>59152613</id>
    //         <screen_name>dpchiesa</screen_name>
    //     </user>
    // </image>


    /// <summary>
    ///   Class to de-serialize the upload response from TwitPic.
    /// </summary>
    [XmlRoot("image")]
    [XmlType("image")]
    public class TwitPicUploadResponse
    {
        public string url { get;set; }
    }
}

