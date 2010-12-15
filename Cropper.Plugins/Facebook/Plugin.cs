// Facebook/Plugin.cs
//
// Code for a cropper plugin that sends a screen snap to
// Facebook.com.
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
//using System.Xml.Serialization;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Utils;        // for Tracing
using System.Text;                 // Encoding, StringBuilder
using RE=System.Text.RegularExpressions;

using System.Web.Script.Serialization;  // for JavaScriptSerializer


namespace Cropper.SendToFacebook
{
    public class Plugin : DesignablePluginThatUsesFetchOutputStream, IConfigurablePlugin
    {
        public override string Description
        {
            get
            {
                return "Send to Facebook";
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
            return "Send to Facebook [Dino Chiesa]";
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            if (!VerifyAuthentication()) return;

            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
        }


        private bool VerifyAuthentication()
        {
            if (!PluginSettings.HaveAccess)
            {
                AuthorizeApp();

                if (!PluginSettings.HaveAccess)
                {
                    MessageBox.Show("You must approve Cropper for use with Facebook\n" +
                                    "before uploading an image.\n\n",
                                    "No Authorizaiton for Facebook plugin");
                    return false;
                }
            }
            return true;
        }


        private void AuthorizeApp()
        {
            string url = String.Format(FacebookSettings.AUTHORIZE_URL_FORMAT,
                                       FacebookSettings.APP_ID);

            // create and display a new form
            var f = new System.Windows.Forms.Form();
            var web1 = new System.Windows.Forms.WebBrowser();
            f.SuspendLayout();

            var cursor = f.Cursor;
            f.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            // event handlers
            WebBrowserDocumentCompletedEventHandler docCompleted = (sender, e) => {
                Tracing.Trace("web1.completed, url = '{0}'", web1.Url.ToString());
                var url2 = web1.Url.ToString();

                // It's possible there will be multiple pages in the flow.
                // We want to respond only to the "login_success" page.  Don't
                // be confused by the name: login_success does not mean "access
                // granted."
                if (url2.StartsWith("http://www.facebook.com/connect/login_success.html#"))
                {
                    web1.Visible = false;

                    var token =
                        RE.Regex.Replace(url2,
                                         ".+login_success.html#access_token=([^&]+).+",
                                         "$1");

                    PluginSettings.AccessToken = token;

                    // If token is null or empty, then The user has declined access.
                    // Otherwise, then the user has granted access.
                    // Either way, we want to close the form and return.
                    f.Close();
                }
            };

            WebBrowserNavigatingEventHandler navigating = (sender,e) => {
                Tracing.Trace("web1.navigating, url = '{0}'", web1.Url.ToString());
                f.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                f.Update();
            };

            // The embedded browser can navigate through HTTP 302
            // redirects, download images, and so on. The display will
            // initially be blank while it is waiting for downloads and
            // redirects. Also, after the user clicks "Login", there's a
            // delay.  In those cases we want the wait cursor. Only turn
            // it off if the status text is "Done."
            EventHandler statusChanged = (sender,e) => {
                var t = web1.StatusText;
                if (t == "Done")
                    f.Cursor = cursor;
                else if (!String.IsNullOrEmpty(t))
                {
                    Tracing.Trace("web1.status = '{0}'", t);
                    f.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                }
            };

            //
            // web1
            //
            web1.Location = new System.Drawing.Point(4, 166);
            web1.Name = "web1";
            web1.DocumentCompleted += docCompleted;
            web1.Navigating += navigating;
            web1.StatusTextChanged += statusChanged;
            web1.Dock = DockStyle.Fill;
            f.Name = "embeddedBrowserForm";
            f.Text = "Please approve the Cropper Plugin for Facebook";
            f.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            f.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            f.ClientSize = new System.Drawing.Size(460, 320);
            f.Controls.Add(web1);
            f.ResumeLayout(false);
            web1.Url = new Uri(url);

            f.ShowDialog(); // will this wait for form exit?
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
                Tracing.Trace("Facebook::SaveImage ({0})", _fileName);

                SaveImageInDesiredFormat(stream, image);

                success = true;
            }
            catch (Exception exception1)
            {
                Tracing.Trace("Exception while saving the image: {0}", exception1.Message);
                string msg = "There's been an exception while saving the image: " +
                             exception1.Message + "\n" + exception1.StackTrace;
                msg+= "\n\nYou will have to Upload this file manually: " + this._fileName ;
                MessageBox.Show(msg);
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
        ///   This plugin handles BMP, PNG and JPG.
        /// </summary>
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
            Tracing.Trace("Facebook::SaveImageInDesiredFormat");
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
            var myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var cInfo = GetEncoderInfo("image/jpeg");
            using (var p1 = new System.Drawing.Imaging.EncoderParameters(1))
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


        private string GetCaption()
        {
            if (PluginSettings.Caption)
            {
                // prompt for the tweet here
                var f = new System.Windows.Forms.Form();
                var btnOK = new System.Windows.Forms.Button();
                var btnCancel = new System.Windows.Forms.Button();
                var label = new System.Windows.Forms.Label();
                var txt = new System.Windows.Forms.TextBox();
                label.Text = "Caption?";
                label.AutoSize = true;
                label.Location = new System.Drawing.Point(4, 12);
                txt.Text = "";
                txt.TabIndex = 11;
                txt.Multiline = true;
                txt.Location = new System.Drawing.Point(54, 8);
                txt.Size = new System.Drawing.Size(268, 54);
                btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                btnCancel.Location = new System.Drawing.Point(250, 66);
                btnCancel.Name = "btnCancel";
                btnCancel.Size = new System.Drawing.Size(68, 23);
                btnCancel.TabIndex = 71;
                btnCancel.Text = "&Cancel";
                btnCancel.UseVisualStyleBackColor = true;
                btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
                btnOK.Location = new System.Drawing.Point(174, 66);
                btnOK.Name = "btnOK";
                btnOK.Size = new System.Drawing.Size(68, 23);
                btnOK.TabIndex = 61;
                btnOK.Text = "&OK";
                btnOK.UseVisualStyleBackColor = true;
                f.Controls.Add(label);
                f.Controls.Add(txt);
                f.Controls.Add(btnOK);
                f.Controls.Add(btnCancel);
                f.Name = "Caption";
                f.Text = "Provide a caption...";
                //f.ClientSize = new System.Drawing.Size(324, 118);
                f.MinimumSize = new System.Drawing.Size(342, 130);
                f.MaximumSize = new System.Drawing.Size(342, 130);
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
        ///  This does the real work of the plugin - uploading to Facebook.com.
        /// </summary>
        ///
        /// <remarks>
        ///   First upload the image, and then place the raw
        ///   image URL onto the clipboard for easy reference/paste.
        //    Optionally, post a Tweet to Twitter.
        /// </remarks>
        private void UploadImage()
        {
            Tracing.Trace("Facebook::UploadImage");

            try
            {
            // Can publish a photo to a specific, existing photo album with
            // a POST to http://graph.facebook.com/ALBUM_ID/photos.
            // But it seems more appropriate to just implicitly post the image to the
            // application-specific album, which means POST to
            // http://graph.facebook.com/me/photos.

                var url = FacebookSettings.URL_UPLOAD_STUB + PluginSettings.AccessToken;

                var encoding = Encoding.GetEncoding("iso-8859-1");

                // prepare the upload POST request
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.PreAuthenticate = true;
                request.AllowWriteStreamBuffering = true;
                var boundary = "xxx"+Guid.NewGuid().ToString().Replace("-","");
                request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

                // prepare the payload
                var separator = "--" + boundary;
                var footer = separator + "--";

                var contents = new StringBuilder();
                contents.AppendLine(separator);

                contents.AppendLine("Content-Disposition: form-data; name=\"message\"");
                contents.AppendLine();
                contents.AppendLine(GetCaption());
                contents.AppendLine(separator);

                string shortFileName = Path.GetFileName(this._fileName);
                string fileHeader = string.Format("Content-Disposition: file; " +
                                                  "name=\"source\"; filename=\"{0}\"",
                                                  shortFileName);
                // TODO:  make this so I don't have to store
                // all the image data in a single buffer. One option is
                // to do it chunked transfer. need to do the proper encoding in
                // any case.
                string fileData = encoding
                    .GetString(File.ReadAllBytes(this._fileName));

                contents.AppendLine(fileHeader);
                contents.AppendLine(string.Format("Content-Type: {0}",
                                                  GetMimeType(shortFileName)));
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

                            // get the id for the uploaded photo
                            var jss = new JavaScriptSerializer();
                            var resource = jss.Deserialize<Facebook.Data.Resource>(responseText);
                            // now get the other properties for that photo
                            var src = "https://graph.facebook.com/"+
                                      resource.id +
                                      "?access_token=" + PluginSettings.AccessToken;
                            var json = FbFetch(src);
                            var photo = jss.Deserialize<Facebook.Data.Photo>(json);
                            Clipboard.SetDataObject(photo.source, true);
                            if (PluginSettings.PopBrowser)
                                System.Diagnostics.Process.Start(photo.source);
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
                                this._fileName);
            }
            return ;
        }

        private string FbFetch(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseText = reader.ReadToEnd();
                    return responseText;
                }
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
            Tracing.Trace("Facebook::Connect");
            base.Connect(persistableOutput);
        }

        public override void Disconnect()
        {
            Tracing.Trace("Facebook::Disconnect");
            base.Disconnect();
        }

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("Facebook::MenuClick");
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
                    _configForm = new FacebookOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            FacebookOptionsForm form = sender as FacebookOptionsForm;
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
            set { PluginSettings = value as FacebookSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private FacebookSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new FacebookSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        #endregion

        private FacebookSettings _settings;
        private FacebookOptionsForm _configForm;
        private string _fileName;
    }

}


namespace Facebook.Data
{
    public class Resource
    {
        public string id { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public int width { get; set; }
        public string source { get; set; }
    }

    public class Person : Resource
    {
        public string name { get; set; }
    }

    public class Photo : Resource
    {
        public Person from { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string source { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public List<Image> images { get; set; }
        public string link { get; set; }
        public string icon { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
    }

    // The following can be used to de-serialize Album info from
    // Facebook.  This plugin doesn't deal with Albums, so
    // these classes are unnecessary.
    //
    // public sealed class Album
    // {
    //     public string id { get; set; }
    //     public Person from { get; set; }
    //     public string name { get; set; }
    //     public string link { get; set; }
    //     public int count { get; set; }
    //
    //     public override string ToString()
    //     {
    //         return String.Format("{{id:{0}, from:{1}, name={2}, link={3}, count={4}}}",
    //                              id, from.ToString(), name, link, count);
    //     }
    // }
    //
    // public sealed class Albums
    // {
    //     public List<Album> data { get; set; }
    //     public Dictionary<string, string> paging { get; set; }
    //     private string _data
    //     {
    //         get { return ListToString(data); }
    //     }
    //     private string _paging
    //     {
    //         get { return DictToString(paging); }
    //     }
    //
    //     private static string ListToString(List<Album> list)
    //     {
    //         if (list == null) return "";
    //         return String.Join(",\n    ", list.Select(x => x.ToString()).ToArray());
    //     }
    //
    //     private static string DictToString(Dictionary<String,String> hash)
    //     {
    //         if (hash == null) return "";
    //         return String.Join(",\n    ", hash.Select(kvp => kvp.Key + ":" + kvp.Value).ToArray());
    //     }
    //
    //     public override string ToString()
    //     {
    //         string s = String.Format("Albums::\n{{data:\n    {0},\n paging:\n    {1}}}",
    //                                  _data, _paging);
    //         return s;
    //     }
    // }

}


