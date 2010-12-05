// ImageShack/Plugin.cs
//
// Code for a cropper plugin that sends a screen snap to
// ImageShack.us .
//
// To enable tracing for this DLL, build like so:
//    msbuild /p:Platform=x86 /p:DefineConstants=Trace
//
//
// Dino Chiesa
// Sat, 04 Dec 2010  20:58
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
using Microsoft.Http;             // HttpClient

namespace Cropper.SendToImageShack
{
    public class Plugin : DesignablePluginThatUsesFetchOutputStream, IConfigurablePlugin
    {
        public override string Description
        {
            get
            {
                return "Send to ImageShack";
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
            return "Send to ImageShack [Dino Chiesa]";
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
                Tracing.Trace("ImageShack::SaveImage ({0})", _fileName);

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
        ///   ImageShack supports only PNG and JPG and GIF. This plugin does
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
            Tracing.Trace("ImageShack::SaveImageInDesiredFormat");
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
            Tracing.Trace("ImageShack::UploadImage");

            try
            {
                string relativeUrl = "upload_api.php";
                var http = new HttpClient(_baseUri);
                var form = new HttpMultipartMimeForm();
                using (var fs = File.Open(this._fileName, FileMode.Open, FileAccess.Read))
                {
                    string mimetype = GetMimeType(this._fileName);
                    form.Add("fileupload",
                             this._fileName,
                             HttpContent.Create(fs, mimetype, fs.Length));
                    form.Add("key", _developerKey);
                    form.Add("rembar", "1");

                    if (PluginSettings.UseCookie)
                    {
                        // TODO: Apply the cookie to the transaction
                    }

                    if (PluginSettings.UseCustomTags)
                    {
                        // prompt for the custom tags here
                        var f = new System.Windows.Forms.Form();
                        var btnOK = new System.Windows.Forms.Button();
                        var label = new System.Windows.Forms.Label();
                        var txt = new System.Windows.Forms.TextBox();
                        label.Text = "Tags:";
                        label.AutoSize = true;
                        label.Location = new System.Drawing.Point(4, 8);
                        txt.Text = "";
                        txt.TabIndex = 11;
                        txt.Location = new System.Drawing.Point(48, 6);
                        txt.Size = new System.Drawing.Size(274, 26);
                        btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
                        btnOK.Location = new System.Drawing.Point(254, 32);
                        btnOK.Name = "btnOK";
                        btnOK.Size = new System.Drawing.Size(68, 23);
                        btnOK.TabIndex = 61;
                        btnOK.Text = "&OK";
                        btnOK.UseVisualStyleBackColor = true;
                        f.Controls.Add(label);
                        f.Controls.Add(txt);
                        f.Controls.Add(btnOK);
                        f.Name = "Tags";
                        f.Text = "Tags for this image?";
                        //f.ClientSize = new System.Drawing.Size(324, 118);
                        f.MinimumSize = new System.Drawing.Size(342, 96);
                        f.MaximumSize = new System.Drawing.Size(342, 96);
                        var result = f.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            if (!String.IsNullOrEmpty(txt.Text))
                                form.Add("tags", txt.Text);
                        }
                        else
                        {
                            Tracing.Trace("cancelled.");
                            return;
                        }
                    }
                    else if (!String.IsNullOrEmpty(PluginSettings.FixedTags))
                        form.Add("tags", PluginSettings.FixedTags);


                    var response = http.Post(relativeUrl, form.CreateHttpContent());
                    response.EnsureStatusIsSuccessful();
                    var foo = response.Content.ReadAsXmlSerializable<UploadResponseSuccess>();
                    if ((foo == null) || foo.links == null)
                    {
                        var foo2 = response.Content.ReadAsXmlSerializable<UploadResponseError>();
                        if ((foo2 != null) && (foo2.error != null))
                            throw new InvalidOperationException(String.Format("Error on upload, id = {0}}, message = {1}.", foo2.error.id, foo2.error.message));


                        throw new InvalidOperationException("Successful response, but cannot deserialize xml.");

                    }

                    string rawImageUri = foo.links.imageuri;

                    if (PluginSettings.PopBrowser)
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
                                this._fileName );
            }
            return ;
        }


#if Trace
        // these methods are needed only for diagnostic purposes.
        public override void Connect(IPersistableOutput persistableOutput)
        {
            Tracing.Trace("ImageShack::Connect");
            base.Connect(persistableOutput);
        }

        public override void Disconnect()
        {
            Tracing.Trace("ImageShack::Disconnect");
            base.Disconnect();
        }

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("ImageShack::MenuClick");
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
                    _configForm = new ImageShackOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            ImageShackOptionsForm form = sender as ImageShackOptionsForm;
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
            set { PluginSettings = value as ImageShackSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private ImageShackSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new ImageShackSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        #endregion


        private ImageShackSettings _settings;
        private ImageShackOptionsForm _configForm;
        private static readonly string _baseUri= "http://www.imageshack.us/";
        private static readonly string _developerKey= "T39OZMFC7b60153bbc4341b959be614bc37f3278";
        private string _fileName;
    }


    // see http://twitpic.com/api.do#upload
    //
    // Example responses:
    //
    // <?xml version="1.0" encoding="iso-8859-1"?>
    // <imginfo xmlns="http://ns.imageshack.us/imginfo/7/" version="7" timestamp="1291548263">
    //   <rating>
    //     <ratings>0</ratings>
    //     <avg>0.0</avg>
    //   </rating>
    //   <files server="80" bucket="5474">
    //     <image size="14457" content-type="image/jpeg">croppercapture70.jpg</image>
    //     <thumb size="5970" content-type="image/jpeg">croppercapture70.th.jpg</thumb>
    //   </files>
    //   <resolution>
    //     <width>257</width>
    //     <height>184</height>
    //   </resolution>
    //   <class>r</class>
    //   <visibility>no</visibility>
    //   <uploader>
    //     <ip>166.137.141.250</ip>
    //   </uploader>
    //   <links>
    //     <image_link>http://img80.imageshack.us/img80/5474/croppercapture70.jpg</image_link>
    //     <image_html>&lt;a href=&quot;http://img80.imageshack.us/my.php?image=croppercapture70.jpg&quot; target=&quot;_blank&quot;&gt;&lt;img src=&quot;http://img80.imageshack.us/img80/5474/croppercapture70.jpg&quot; alt=&quot;Free Image Hosting at www.ImageShack.us&quot; border=&quot;0&quot;/&gt;&lt;/a&gt;</image_html>
    //     <image_bb>[URL=http://img80.imageshack.us/my.php?image=croppercapture70.jpg][IMG]http://img80.imageshack.us/img80/5474/croppercapture70.jpg[/IMG][/URL]</image_bb>
    //     <image_bb2>[url=http://img80.imageshack.us/my.php?image=croppercapture70.jpg][img=http://img80.imageshack.us/img80/5474/croppercapture70.jpg][/url]</image_bb2>
    //     <thumb_link>http://img80.imageshack.us/img80/5474/croppercapture70.th.jpg</thumb_link>
    //     <thumb_html>&lt;a href=&quot;http://img80.imageshack.us/my.php?image=croppercapture70.jpg&quot; target=&quot;_blank&quot;&gt;&lt;img src=&quot;http://img80.imageshack.us/img80/5474/croppercapture70.th.jpg&quot; alt=&quot;Free Image Hosting at www.ImageShack.us&quot; border=&quot;0&quot;/&gt;&lt;/a&gt;</thumb_html>
    //     <thumb_bb>[URL=http://img80.imageshack.us/my.php?image=croppercapture70.jpg][IMG]http://img80.imageshack.us/img80/5474/croppercapture70.th.jpg[/IMG][/URL]</thumb_bb>
    //     <thumb_bb2>[url=http://img80.imageshack.us/my.php?image=croppercapture70.jpg][img=http://img80.imageshack.us/img80/5474/croppercapture70.th.jpg][/url]</thumb_bb2>
    //     <yfrog_link>http://yfrog.com/28croppercapture70j</yfrog_link>
    //     <yfrog_thumb>http://yfrog.com/28croppercapture70j.th.jpg</yfrog_thumb>
    //     <ad_link>http://img80.imageshack.us/my.php?image=croppercapture70.jpg</ad_link>
    //     <done_page>http://img80.imageshack.us/content.php?page=done&amp;l=img80/5474/croppercapture70.jpg</done_page>
    //   </links>
    // </imginfo>
    //
    //
    // Error:
    //
    //   <links>
    //     <error id="something">This is the error message</error>
    //   </links>
    //
    // Yes, the root element is different for the error and the success case.

    [XmlType("imginfo", Namespace="http://ns.imageshack.us/imginfo/7/")]
    [XmlRoot("imginfo", Namespace="http://ns.imageshack.us/imginfo/7/")]
    public partial class UploadResponseSuccess
    {
        [XmlAttribute]
        public string version { get;set; }

        [XmlAttribute]
        public string timestamp { get;set; }

        public ImageLinks links { get;set; }
    }

    public class ImageLinks
    {
        [XmlElement("image_link")]
        public string imageuri { get;set; }
        [XmlElement("image_html")]
        public string html { get;set; }
    }

    [XmlType("links", Namespace="")]
    [XmlRoot("links", Namespace="")]
    public class UploadResponseError
    {
        public ResponseError error  { get;set; }

    }
    public class ResponseError
    {
        [XmlAttribute("id")]
        public string id          { get;set; }
        [XmlText]
        public string message     { get;set; }
    }

    public class ImageShackSettings
    {
        string _format;
        public ImageShackSettings()
        {
            // defaults
            JpgImageQuality= 80;
            ImageFormat = "jpg";
            FixedTags = "Cropper, screenshot";
            PopBrowser = true;
        }

        /// <summary>
        ///   True: use the ImageShack.us cookie, if present on the computer,
        ///   when uploading. This means uploads will not be anonymous.
        /// </summary>
        public bool UseCookie { get; set; }

        /// <summary>
        ///   True: pop the browser after upload.
        ///   False: don't.
        /// </summary>
        public bool PopBrowser { get; set; }

        /// <summary>
        ///   True: use different tags for each upload.  Will prompt the user.
        ///   False: don't.
        /// </summary>
        public bool UseCustomTags { get; set; }

        /// <summary>
        ///   Quality level to use when saving an image in JPG format.
        /// </summary>
        public int JpgImageQuality
        {
            get;
            set;
        }

        /// <summary>
        ///   A fixed, comma-separated list of tabs to apply to each uploaded
        ///   image, if the "use custom tags" option is off.
        /// </summary>
        public string FixedTags { get; set; }

        /// <summary>
        ///   The Image format; one of Jpeg, Png.
        /// </summary>
        public string ImageFormat
        {
            get { return _format; }

            set
            {
                var v = value.ToLower();
                if (v == "png" || v == "jpg" || v == "bmp")
                    _format = v;
            }
        }
    }
}

