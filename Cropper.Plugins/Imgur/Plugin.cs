// Imgur/Plugin.cs
//
// Code for a cropper plugin that sends a screen snap to
// Imgur.com
//
// To enable tracing for this DLL, build like so:
//    msbuild /p:Platform=x86 /p:DefineConstants=Trace
//
//
// Dino Chiesa
// 2010 Nov 9
//
// -------------------------------------------------------
// flymake: msbuild Imgur.csproj /t:CheckSyntax /property:FlymakeCheck=@@SRC@@ /property:FlymakeExclude=@@ORIG@@
// compile: msbuild
//

// #define Trace

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Common;       // for Tracing
using Microsoft.Http;              // HttpClient

namespace Cropper.SendToImgur
{
    public class Plugin :
        DesignablePluginThatUsesFetchOutputStream,
        IConfigurablePlugin
    {
    // FIXME CropperPlugins.Common.IUpload
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool Beep(UInt32 frequency, UInt32 duration);

        public Plugin()
        {
            Tracing.Trace("+--------------------------------");
            Tracing.Trace("Imgur::ctor ({0:X8})", this.GetHashCode());
        }

        public override string Description
        {
            get { return "Send to Imgur"; }
        }

        public override string Extension
        {
            get { return PluginSettings.ImageFormat; }
        }

        public override string ToString()
        {
            return "Send to Imgur.com [Dino Chiesa]";
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            if (PluginSettings.PlaySound)
                Beep(4000,30);

            if (PluginSettings.WantLogging)
                this._logger = new ImgurLogWriter(new FileInfo(e.ImageNames.FullSize).DirectoryName);
            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
        }


        private bool VerifyBasicSettings()
        {
            if (!PluginSettings.Completed)
            {
                var dlg = new ImgurOptionsForm(PluginSettings);
                dlg.MakeButtonsVisible();
                dlg.ShowDialog();
            }

            if (!PluginSettings.Completed)
            {
                MessageBox.Show("You must configure Imgur settings before " +
                                "uploading an image to the service.\n\n",
                                "Missing Settings for Imgur plugin",
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
                Tracing.Trace("Imgur::SaveImage ({0})", _fileName);

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
                                "Upload to Imgur did not happen",
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
            Tracing.Trace("Imgur::SaveImageInDesiredFormat");

            // optionally Add watermark text on image
            int height = image.Height;
            int width = image.Width;
            var bitmap = new System.Drawing.Bitmap(image, width, height);
            if (PluginSettings.AddWatermark)
            {
                var g = System.Drawing.Graphics.FromImage(bitmap);
                var brush = new SolidBrush(Color.FromArgb(113, 255, 255, 255));
                g.TranslateTransform(50, 50);
                g.RotateTransform(-27);
                g.DrawString("CROPPER",
                             new Font("Arial", 18, System.Drawing.FontStyle.Bold),
                             brush, 0, 100);
            }

            var ms = new MemoryStream();
            bitmap.Save(ms, DesiredImageFormat);
            ms.Seek(0, SeekOrigin.Begin);
            image = Image.FromStream(ms);

            if (String.Compare(Extension, "jpg", true) == 0)
            {
                SaveImage_Jpg(stream, bitmap);  // image
            }
            else
            {
                image.Save(stream, DesiredImageFormat);
            }

            ////     Response.ContentType = "image/jpeg";
            // bitmapimage.Save(Response.OutputStream, ImageFormat.Jpeg);
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
            Tracing.Trace("Imgur::UploadImage");

            if (!VerifyBasicSettings()) return;

            Hacks.BootstrapSettings(PluginSettings);

            try
            {
                var http = new HttpClient(Plugin._baseUri);
                var form = new HttpMultipartMimeForm();
                using (var fs = File.Open(this._fileName, FileMode.Open, FileAccess.Read))
                {
                    form.Add("key", PluginSettings.Key);
                    form.Add("image",
                             this._fileName,
                             HttpContent.Create(fs, "application/octet-stream", fs.Length));
                    form.Add("type", "file");
                    form.Add("title", "uploaded by Cropper SendToImgur plugin"); // optional
                    form.Add("caption", "http://cropper.codeplex.com"); // optional
                    var response = http.Post("upload.xml", form.CreateHttpContent());
                    response.EnsureStatusIsSuccessful();
                    var foo = response.Content.ReadAsXmlSerializable<UploadResponse>();
                    if (foo.links == null)
                        throw new InvalidOperationException("Successful response, but link is empty.");

                    string rawImageUri = foo.links.original;

                    if (PluginSettings.PopBrowser)
                        System.Diagnostics.Process.Start(rawImageUri);

                    Clipboard.SetDataObject(rawImageUri, true);
                    if (this._logger != null)
                    {
                        try
                        {
                            this._logger.Log(rawImageUri);
                        }
                        catch (Exception ex2)
                        {
                            MessageBox.Show("There's been an exception writing the ImgUr log?" +
                                            Environment.NewLine +
                                            ex2.Message +
                                            Environment.NewLine,
                                            "This isn't serious",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception exception2)
            {
                Tracing.Trace("Exception: {0}", exception2.StackTrace);
                Tracing.Trace("---------------------------------");
                MessageBox.Show("There's been an exception uploading your image:" +
                                Environment.NewLine +
                                exception2.Message +
                                Environment.NewLine +
                                Environment.NewLine +
                                "You will have to upload this file manually: " +
                                Environment.NewLine +
                                this._fileName,
                                "Upload to Imgur failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            return ;
        }

        public void UploadFile(string fileName)
        {
            this._fileName = fileName;
            UploadImage();
        }


#if Trace
        // these methods are needed only for diagnostic purposes.
        public override void Connect(IPersistableOutput persistableOutput)
        {
            Tracing.Trace("Imgur::Connect");
            base.Connect(persistableOutput);
        }

        public override void Disconnect()
        {
            Tracing.Trace("Imgur::Disconnect");
            base.Disconnect();
        }

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("Imgur::MenuClick");
            base.OnImageFormatClick(sender, e);
        }
#endif


        #region IConfigurablePlugin Implementation

        /// <summary>
        /// Gets the plug-ins impementation of the <see
        /// cref="BaseConfigurationForm"/> used for setting plug-in
        /// specific options.
        /// </summary>
        BaseConfigurationForm IConfigurablePlugin.ConfigurationForm
        {
            get
            {
                if (this._configForm == null)
                {
                    this._configForm = new ImgurOptionsForm(PluginSettings);
                    this._configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            ImgurOptionsForm form = sender as ImgurOptionsForm;
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
            get
            {
                Tracing.Trace("Imgur::get_Settings");
                return PluginSettings;
            }
            set { PluginSettings = value as ImgurSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private ImgurSettings PluginSettings
        {
            get
            {
                if (this._settings == null)
                    this._settings = new ImgurSettings();
                return this._settings;
            }
            set { this._settings = value; }
        }

        #endregion


        private ImgurSettings _settings;
        private ImgurOptionsForm _configForm;
        private static readonly string _baseUri= "http://api.imgur.com/2/";

        // TODO: allow someone to use their Imgur account (requires a different key)
        //private static readonly string _ImgurDevKey = "e2d95a325f87bdc89fae26ad69cb3c49";

        private string _fileName;

        private ImgurLogWriter _logger;
    }



    [XmlType("upload", Namespace="")]
    [XmlRoot("upload", Namespace="")]
    public partial class UploadResponse {
        public UploadResponseImage image;
        public UploadResponseLinks links;
    }

    [XmlType("image")]
    public class UploadResponseImage
    {
        public string name       { get;set; }
        public string title      { get;set; }
        public string caption    { get;set; }
        public string hash       { get;set; }
        public string deletehash { get;set; }
        public string datetime   { get;set; }
        public string type       { get;set; }
        public string animated   { get;set; }
        public string width      { get;set; }
        public string height     { get;set; }
        public string size       { get;set; }
        public string views      { get;set; }
        public string bandwidth  { get;set; }
    }

    [XmlType("links")]
    public class UploadResponseLinks
    {
        public string original        { get;set; }
        public string imgur_page      { get;set; }
        public string delete_page     { get;set; }
        public string small_square    { get;set; }
        public string large_thumbnail { get;set; }
    }


    internal sealed class CachedSettings
    {
        static readonly CachedSettings instance= new CachedSettings();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CachedSettings() { }

        public string Key { get; set; }

        // explicit nonpublic default constructor
        CachedSettings() { Key= ""; }

        public static CachedSettings Instance
        {
            get { return instance; }
        }
    }


    public class ImgurSettings
    {
        //private string _Key;
        private string _format;

        public ImgurSettings()
        {
            Tracing.Trace("+--------------------------------");
            Tracing.Trace("ImgurSettings::ctor ({0:X8})", this.GetHashCode());
            JpgImageQuality= 80; // default
            ImageFormat = "png"; // default
            PopBrowser = true;
            PlaySound = true;
            AddWatermark = false;
            WantLogging = true;
            Key="";
        }

        /// <summary>
        ///   The key to use for anonymous uploads to Imgur.com.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     For an explanation of what this is, see
        ///     http://imgur.com/register/api_anon .
        ///   </para>
        ///   <para>
        ///     There's a singleton used as a backing store for this.
        ///     This is because it can change outside the scope of an
        ///     options form, and we want to retain any non-null value
        ///     across instances of the Plugin (and the Settings class).
        ///   </para>
        /// </remarks>
        public string Key { get; set; }


#if NOT
        {
            get
            {
                Tracing.Trace("ImgurSettings::{0:X8}::get_Key", this.GetHashCode());
                if (!String.IsNullOrEmpty(CachedSettings.Instance.Key)
                    && String.IsNullOrEmpty(_Key))
                    _Key= CachedSettings.Instance.Key;

                Tracing.Trace("ImgurSettings::{0:X8}::get_Key value({1})",
                              this.GetHashCode(), _Key);
                return _Key;
            }
            set
            {
                Tracing.Trace("ImgurSettings::{0:X8}::set_Key ({1})",
                              this.GetHashCode(), value);
                _Key = value;
                if (!String.IsNullOrEmpty(value))
                    CachedSettings.Instance.Key = value;
            }
        }
#endif

        /// <summary>
        ///   Quality level to use when saving an image in JPG format.
        /// </summary>
        public int JpgImageQuality { get; set; }

        /// <summary>
        ///   True: pop a browser after upload. False: Don't.
        /// </summary>
        public bool PopBrowser { get; set; }

        /// <summary>
        ///   True: play a short beep after upload. False: Don't.
        /// </summary>
        public bool PlaySound { get; set; }

        /// <summary>
        ///   True: add a watermark to each uploaded image.
        /// </summary>
        public bool AddWatermark { get; set; }

        /// <summary>
        ///   True: log each uploaded image into an XML file in the Cropper Captures dir
        /// </summary>
        public bool WantLogging { get; set; }

        /// <summary>
        ///   The Image format; one of Jpeg, Png, Bmp.
        /// </summary>
        public string ImageFormat
        {
            get { return _format; }

            set
            {
                var v = value.ToLower();
                if (v == "bmp" || v == "png" || v == "jpg")
                    _format = v;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool Completed
        {
            get
            {
                return !System.String.IsNullOrEmpty(Key);
            }
        }
    }
}



