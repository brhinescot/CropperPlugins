
namespace Cropper.SendToFacebook
{
    using System;
    using System.Collections.Generic;

    using CropperPlugins.Common;

    /// <summary>
    ///   A singleton that the access token and secret for OAuth.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>
    ///     The Plugin class implements IConfigurableSettings, to let
    ///     Cropper core handle the configuration settings. But, the
    ///     problem is that Cropper invokes the form multiple times. When
    ///     Cropper retrieves the settings to store them, it could use
    ///     one of any number of instances?  Why does it do this?
    ///     I don't know.
    ///   </para>
    ///   <para>
    ///     This class caches the access token value across
    ///     instances. AccessToken is the only field in the Settings
    ///     class that can change outside the config form, so, it's
    ///     the only one that needs to be cached in this way.
    ///   </para>
    /// </remarks>
    internal sealed class CachedSettings
    {
        static readonly CachedSettings instance= new CachedSettings();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static CachedSettings() { }

        public string AccessToken;

        // explicit nonpublic default constructor
        CachedSettings() { AccessToken = ""; }

        public static CachedSettings Instance
        {
            get { return instance; }
        }
    }



    public class FacebookSettings
    {
        public static readonly string
            AUTHORIZE_URL_FORMAT = "https://graph.facebook.com/oauth/authorize?client_id={0}&" +
                              "redirect_uri=http://www.facebook.com/connect/login_success.html&" +
                              "type=user_agent&scope=user_photos,publish_stream",
            APP_ID = "119755188091651", // ID For Cropper Plugin
            URL_UPLOAD_STUB = "https://graph.facebook.com/me/photos?access_token=" ;

        private string _format;
        private string _AccessToken;

        public FacebookSettings()
        {
            // default
            JpgImageQuality= 80;
            ImageFormat = "jpg";
            AccessToken = CachedSettings.Instance.AccessToken;
            PopBrowser = true;
        }

        /// <summary>
        ///   The access token for authenticating to Facebook.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This needs to use a singleton as the backing store,
        ///     because it gets set outside of the options form.
        ///     See the comment on the CachedSettings class above
        ///     for further explanation.
        ///   </para>
        /// </remarks>
        public string AccessToken
        {
            get
            {
                if (!String.IsNullOrEmpty(CachedSettings.Instance.AccessToken)
                    && String.IsNullOrEmpty(_AccessToken))
                    _AccessToken= CachedSettings.Instance.AccessToken;

                return _AccessToken;
            }
            set
            {
                _AccessToken = value;
                if (!String.IsNullOrEmpty(value))
                    CachedSettings.Instance.AccessToken = value;
            }
        }

        /// <summary>
        ///   True: attach a custom caption to the message - the user
        ///   will  be prompted. False: just up-load the image, and
        ///   apply a "standard" caption.
        /// </summary>
        public bool Caption { get; set; }

        /// <summary>
        ///   True: open a browser window with the just-uploaded image.
        ///   False: don't.
        /// </summary>
        public bool PopBrowser { get; set; }

        /// <summary>
        ///   Quality level to use when saving an image in JPG format.
        /// </summary>
        public int JpgImageQuality
        {
            get;
            set;
        }

        /// <summary>
        ///   The Image format; one of bmp, jpg, png.
        /// </summary>
        public string ImageFormat
        {
            get { return _format; }

            set
            {
                var v = value.ToLower();
                if (v == "png" || v == "bmp" || v == "jpg")
                    _format = v;
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool HaveAccess
        {
            get
            {
                if (System.String.IsNullOrEmpty(AccessToken))
                    return false;

                // tokens can expire. Need to renew someitmes?
                try
                {
                    // try fetching something
                    var src = "https://graph.facebook.com/me?access_token=" +
                              AccessToken;
                    var json = Cropper.SendToFacebook.Plugin.FbFetch(src);
                }
                catch (System.Exception)
                {
                    return false;
                }
                return true;
            }
        }
    }
}