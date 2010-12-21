// Flickr.cs
// ------------------------------------------------------------------
//
// Implements the SendToFlickr plugin for Cropper.
//
// This is a basic plugin, with a few small twists. It saves a cropper
// screenshot, and also uploads the shot to the Flickr photo-sharing
// service.
//
// This plugin implements IConfigurablePlugin, which means Cropper
// (Core) will invoke methods on this class to provide it with settings
// values read from the cropper.config file, and will also invoke
// methods on this class to get and then open (present to the user) the
// Windows Form that allows the user to configure settings
// interactively. This is nothing special, really. It's similar in this
// respect to other plugins, like ImageShack, Imgur, S3, and TFS. The
// settings that can be configured include: ikmage format (jpg, png,
// bmp); jpg quality; default tags, and a boolean that tells the plugin
// whether or not to pop the browser window with the URL of the
// just-uploaded image.  The Windows Form that allows specification of
// all this is the first user interaction.
//
// The Flickr service encourages apps development; each app must be
// authorized to tickle Flickr, and the proof of authorization is an
// authz token that the user must explicitly acquire via a browser
// interaction.
//
// Therefore this plugin is special in that respect - it prompts the
// user to open a browser window, to get that authorization.  This is
// done in AuthorizationDialog.cs , the second user interaction form.
// AuthorizationDialog is implicitly presented whenever the plugin
// references PluginSettings.Token, and Token is null or blank. This
// should happen only once, the first time the plugin is ever
// run. Thereafter, the Token is stored in the cropper.config file, and
// there's no need for the user to go to the web to approve
// authorization again.
//
// There is a third user interaction form, PhotoDetails.cs , which
// collects details for the about-to-be-uploaded photo (Screenshot).
// The user keys in tags, a description, ticks a couple boxes for
// properties on the photo, and selects a photoset, before clicking
// a button to do the upload.
//
// Optionally the plugin will pop a browser window with the
// just-uploaded photo in it. In any case the URL of that uploaded photo
// gets placed on the Clipboard for easy paste into a text editor.
//
//
// ------------------------------------------------------------------


using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using FlickrNet;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Utils;       // for Tracing


namespace Cropper.SendToFlickr
{
    public class Plugin : DesignablePluginThatUsesFetchOutputStream,
        IConfigurablePlugin
    {

        public override string Description
        {
            get { return "Send to Flickr"; }
        }

        public override string Extension
        {
            get { return PluginSettings.ImageFormat; }
        }

        public override string ToString()
        {
            return "Send to Flickr";
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage),
                                     this._fileName,
                                     e.FullSizeImage);
        }

        private bool VerifyMinimumSettings()
        {
            Tracing.Trace("Flickr::VerifyMinimumSettings");
            if (String.IsNullOrEmpty(PluginSettings.Token))
                PluginSettings.AcquireToken();
            Tracing.Trace("Flickr::VerifyMinimumSettings tok({0})", PluginSettings.Token);
            if (System.String.IsNullOrEmpty(PluginSettings.Token))
            {
                MessageBox.Show("Authorization is not complete. \n\n" +
                                ((PluginSettings.AuthorizationMessage!= null)
                                 ? PluginSettings.AuthorizationMessage + "\n\n"
                                 :"") +
                                "You must authorize Cropper as a Flickr application\n" +
                                "before using it to upload an image to Flickr.",
                                "SendToFlickr: No Authorization");
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
                Tracing.Trace("Flickr::SaveImage ({0})", _fileName);

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
                                "Cropper Plugin for Flickr: bad save");
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
                    Tracing.Trace("Flickr::get_DesiredImageFormat jpg");
                    return ImageFormat.Jpeg;
                }
                else if (String.Compare(Extension, "bmp", true) == 0)
                {
                    Tracing.Trace("Flickr::get_DesiredImageFormat bmp");
                    return ImageFormat.Bmp;
                }
                else
                {
                    Tracing.Trace("Flickr::get_DesiredImageFormat png");
                    return ImageFormat.Png;
                }
            }
        }


        private void SaveImageInDesiredFormat(Stream stream, System.Drawing.Image image)
        {
            Tracing.Trace("Flickr::SaveImageInDesiredFormat");
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


        private void UploadImage()
        {
            Tracing.Trace("Flickr::UploadImage");
            if (!VerifyMinimumSettings()) return;
            var details1 = new PhotoDetails(flickr, PluginSettings);
            if (details1.ShowDialog() == DialogResult.OK)
            {
                string photoId = flickr.UploadPicture(_fileName,
                                                    details1.Title,
                                                    details1.Description,
                                                    details1.Tags,
                                                    details1.IsPublic,
                                                    details1.IsFamily,
                                                    details1.IsFriend);

                Tracing.Trace("Flickr::UploadImage id({0})", photoId);

                PluginSettings.MostRecentTags = details1.Tags;

                if (!String.IsNullOrEmpty(details1.PhotosetId))
                {
                    if (details1.PhotosetId != "-none-")
                        flickr.PhotosetsAddPhoto(details1.PhotosetId, photoId);

                    PluginSettings.MostRecentPhotosetId = details1.PhotosetId;
                }

                var info = flickr.PhotosGetInfo(photoId);
                Tracing.Trace("Flickr::UploadImage LG url({0})", info.LargeUrl);
                if (PluginSettings.PopBrowser)
                {
                    System.Diagnostics.Process.Start(info.LargeUrl);
                }

                Clipboard.SetDataObject(info.LargeUrl, true);
            }
        }


        private FlickrNet.Flickr _flickr;
        private FlickrNet.Flickr flickr
        {
            get
            {
                if (PluginSettings.Token == null)
                    PluginSettings.AcquireToken();
                _flickr = new FlickrNet.Flickr(FlickrSettings.FLICKR_KEY,
                                               FlickrSettings.FLICKR_SHAREDSECRET,
                                               PluginSettings.Token);
                return _flickr;
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
                    _configForm = new FlickrOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            FlickrOptionsForm form = sender as FlickrOptionsForm;
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
        ///   The settings for this plugin. Required by IConfigurablePlugin.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This property is set during startup with the settings contained in the
        ///   application configuration file (cropper.config).
        /// </para>
        /// <para>
        ///   The object returned by this property is serialized into the application
        ///   configuration file on shutdown.
        /// </para>
        /// </remarks>
        public object Settings
        {
            get {
                Tracing.Trace("Flickr::get_Settings tags({0}) fmt({1})...",
                              PluginSettings.MostRecentTags,
                              PluginSettings.ImageFormat);
                return PluginSettings;
            }
            set {
                Tracing.Trace("Flickr::set_Settings");
                PluginSettings = value as FlickrSettings;
            }
        }

        // Helper property for IConfigurablePlugin Implementation
        private FlickrSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new FlickrSettings();
                return _settings;
            }
            set { _settings = value; }
        }
        #endregion


        private FlickrSettings _settings;
        private FlickrOptionsForm _configForm;
        private string _fileName;
    }



    /// <summary>
    ///   A Data Transfer Object - DTO. This is really a glorified structure.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This is not actually a pure data structure. This class knows how
    ///     to do one thing - retrieve a Token from the Flickr web service,
    ///     when no token is yet available.
    ///   </para>
    /// </remarks>
    public class FlickrSettings
    {
        private string _format;
        public static readonly string FLICKR_KEY = "ab782e182b4eb406d285211811d625ff";
        public static readonly string FLICKR_SHAREDSECRET = "b080496c05335c3d";

        public FlickrSettings()
        {
            Tracing.Trace("FlickrSettings::ctor");
            JpgImageQuality= 80; // default
            ImageFormat = "jpg"; // default
            PopBrowser = true;
        }


        public void AcquireToken()
        {
            if (_authzToken == null)
            {
                // The first time the token is requested, EVER,
                // it will be blank.  Pop the Authorization dialog and
                // get a token from the web.  Thereafter, the token
                // will be stored in and retrieved from the cropper.config
                // file, and provided to the Flickr plugin via
                // IConfigurablePlugin.
                Tracing.Trace("FlickrSettings::AcquireToken, is null");
                var dlg = new AuthorizeDialog();
                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this._authzToken = dlg.AppAuthorizationToken;
                    Tracing.Trace("FlickrSettings::rec'd token ({0})", _authzToken);
                }
                else
                    this._AuthorizationMessage = dlg.Message;
            }
        }

        /// <summary>
        ///   A cached token generated by Flickr, that authorizes the
        ///   Cropper app to upload to a particular Flickr account.
        /// </summary>
        ///
        /// <remarks>
        ///   <para>
        ///   The model is: get the token once, and then use it every
        ///   time the user runs the app.  the user can later revoke the
        ///   token for the app, via the flickr.com website.
        ///   </para>
        /// </remarks>
        private string _authzToken;
        public string Token
        {
            get
            {
                Tracing.Trace("FlickrSettings::get_Token");
                return _authzToken;
            }
            set
            {
                _authzToken = value;
            }
        }

        /// <summary>
        ///   The tags used on the most recent flickr upload.
        ///   This is null if no prior upload occurred.
        /// </summary>
        public string MostRecentTags { get; set; }

        /// <summary>
        ///   The id of the most recently used flickr photoset.
        /// </summary>
        public string MostRecentPhotosetId { get; set; }

        /// <summary>
        ///   True: pop the browser after upload.
        ///   False: don't.
        /// </summary>
        public bool PopBrowser { get; set; }

        /// <summary>
        ///   Quality level to use when saving an image in JPG format.
        /// </summary>
        public int JpgImageQuality { get; set; }

        /// <summary>
        ///   The Image format; one of jpeg, png, bmp.
        /// </summary>
        public string ImageFormat
        {
            get { return _format; }

            set
            {
                var v = value.ToLower();
                if (v == "png" || v == "jpg" || v == "bmp")
                    _format = v;
                else
                    throw new InvalidOperationException();
            }
        }

        private string _AuthorizationMessage;
        [System.Xml.Serialization.XmlIgnore]
        public string AuthorizationMessage
        {
            get
            {
                return _AuthorizationMessage;
            }
        }
    }

}

