// TwitPic/Plugin.cs
//
// Code for a cropper plugin that sends a screen snap to
// TwitPic.com
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
using Microsoft.Http;             // HttpClient

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
            if (!VerifyBasicSettings()) return;

            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
        }


        private bool VerifyBasicSettings()
        {
            if (!PluginSettings.Completed)
            {
                var dlg = new TwitPicOptionsForm(PluginSettings);
                dlg.MakeButtonsVisible();
                dlg.ShowDialog();
            }

            if (!PluginSettings.Completed)
            {
                MessageBox.Show("You must configure a Twitter username and password " +
                                "before uploading an image to TwitPic.\n\n",
                                "Missing Settings for TwitPic plugin");
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



        /// <summary>
        ///  This does the real work of the plugin - uploading to imgur.com.
        /// </summary>
        ///
        /// <remarks>
        ///   First upload the main image, and then place the raw
        ///   image URL onto the clipboard for easy reference/paste.
        /// </remarks>
        private void UploadImage()
        {
            Tracing.Trace("TwitPic::UploadImage");

            try
            {
                string relativeUrl = PluginSettings.Tweet ? "uploadAndPost" : "upload";
                var http = new HttpClient(_baseUri);
                var form = new HttpMultipartMimeForm();
                using (var fs = File.Open(this._fileName, FileMode.Open, FileAccess.Read))
                {
                    form.Add("media",
                             this._fileName,
                             HttpContent.Create(fs, "application/octet-stream", fs.Length));
                    form.Add("username", PluginSettings.Username);
                    form.Add("password", PluginSettings.Password);
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
                                form.Add("message", txt.Text);
                        }
                        else
                        {
                            Tracing.Trace("cancelled.");
                            Tracing.Trace("---------------------------------");
                            return;
                        }
                    }

                    var response = http.Post(relativeUrl, form.CreateHttpContent());
                    response.EnsureStatusIsSuccessful();
                    var foo = response.Content.ReadAsXmlSerializable<UploadResponse>();
                    if (foo == null)
                        throw new InvalidOperationException("Successful response, but cannot deserialize xml.");
                    if ((foo.status != "ok") || (foo.error != null) ||
                        (foo.stat != null && foo.stat != "ok"))
                    {
                        if (foo.error != null)
                            throw new InvalidOperationException(String.Format("Error, code = {0}}, message = {1}.", foo.error.code, foo.error.message));


                        throw new InvalidOperationException(String.Format("Successful response, but status = {0}.", foo.status));

                    }

                    string rawImageUri = foo.mediaurl;
                    System.Diagnostics.Process.Start(rawImageUri);

                    Clipboard.SetDataObject(rawImageUri, true);

                    Tracing.Trace("all done.");
                    Tracing.Trace("---------------------------------");
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
                                this._fileName);
            }
            return ;
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


        private TwitPicSettings _settings;
        private TwitPicOptionsForm _configForm;
        private static readonly string _baseUri= "http://twitpic.com/api/";
        private string _fileName;
    }


    // see http://twitpic.com/api.do#upload
    //
    // Example responses:
    //
    //   <rsp status="ok">
    //     <statusid>1111</statusid>
    //     <userid>11111</userid>
    //     <mediaid>abc123</mediaid>
    //     <mediaurl>http://twitpic.com/abc123</mediaurl>
    //   </rsp>
    //
    //
    //   <rsp stat="fail">
    //     <err code="2" msg="Image type not supported. GIF, JPG, & PNG only" />
    //   </rsp>
    //
    // Yes, the status attribute is "stat" in the failure case and "status" in
    // the success case. Really.
    //

    [XmlType("rsp", Namespace="")]
    [XmlRoot("rsp", Namespace="")]
    public partial class UploadResponse
    {
        [XmlAttribute("stat")]
        public string stat       { get;set; }
        [XmlAttribute("status")]
        public string status     { get;set; }
        public string statusid   { get;set; }
        public string userid     { get;set; }
        public string mediaid    { get;set; }
        public string mediaurl   { get;set; }
        [XmlElement("err")]
        public ResponseError error  { get;set; }
    }

    public class ResponseError
    {
        [XmlAttribute("code")]
        public int code     { get;set; }
        [XmlAttribute("msg")]
        public string message     { get;set; }
    }

    public class TwitPicSettings
    {
        string _format;
        public TwitPicSettings()
        {
            JpgImageQuality= 80; // default
            ImageFormat = "jpg"; // default
        }

        /// <summary>
        ///   The username known to Twitter.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///   The password for authenticating to Twitter.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///   True: send a text tweet along with the message (the user
        ///   will  be prompted. False: just up-load the image.
        /// </summary>
        public bool Tweet { get; set; }

        /// <summary>
        ///   Quality level to use when saving an image in JPG format.
        /// </summary>
        public int JpgImageQuality
        {
            get;
            set;
        }

        /// <summary>
        ///   The Image format; one of Jpeg, Png.
        /// </summary>
        public string ImageFormat
        {
            get { return _format; }

            set
            {
                var v = value.ToLower();
                if (v == "png" || v == "jpg")
                    _format = v;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool Completed
        {
            get
            {
                return !(System.String.IsNullOrEmpty(Username) ||
                         System.String.IsNullOrEmpty(Password));
            }
        }
    }
}

