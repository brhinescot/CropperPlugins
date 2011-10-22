
namespace Cropper.SendToTwitter
{
    using System;
    using System.Collections.Generic;
    using CropperPlugins.Common;

    /// <summary>
    ///   A singleton that stores the access token and secret for OAuth.
    /// </summary>
    ///
    /// <remarks>
    ///   <para>
    ///     The Plugin class implements IConfigurableSettings, to let
    ///     Cropper core handle the configuration settings. But, the
    ///     problem is that cropper invokes the form multiple times. When
    ///     Cropper retrieves the settings to store them, it could use
    ///     one of any number of instances?  Why does it do this?
    ///     I don't know.
    ///   </para>
    ///   <para>
    ///     This class caches the oauth token values across
    ///     instances. Token and token_secret are the only field in the Settings
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
        public string AccessSecret;

        // explicit nonpublic default constructor
        CachedSettings() { AccessToken = ""; AccessSecret = ""; }

        public static CachedSettings Instance
        {
            get { return instance; }
        }
    }



    public class TwitterSettings : CropperPlugins.OAuth.IOAuthSettings
    {
        public static readonly string
            TWITTER_CONSUMER_KEY    = "CVzLPskR3ll4OEwyNGvXBw",
            TWITTER_CONSUMER_SECRET = "HFLdkTg1cjriPwaSJ9zwc93HLox6n4YTwoXqfvaMwk",
            TWITPIC_API_KEY         = "490795eada0ecab994a9ee8aa9d7821e",
            URL_UPLOAD              = "https://upload.twitter.com/1/statuses/update_with_media.xml";

        private string _format;
        private string _AccessToken;
        private string _AccessSecret;

        public TwitterSettings()
        {
            JpgImageQuality= 80; // default
            ImageFormat = "jpg"; // default
            AccessToken = CachedSettings.Instance.AccessToken;
            AccessSecret = CachedSettings.Instance.AccessSecret;
        }

        /// <summary>
        ///   The oauth_token (access token) for authenticating to Twitter.
        /// </summary>
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
        ///   The oauth_token_secret for authenticating to Twitter.
        /// </summary>
        public string AccessSecret
        {
            get
            {
                if (!String.IsNullOrEmpty(CachedSettings.Instance.AccessSecret)
                    && String.IsNullOrEmpty(_AccessSecret))
                    _AccessSecret= CachedSettings.Instance.AccessSecret;

                return _AccessSecret;
            }
            set
            {
                _AccessSecret = value;
                if (!String.IsNullOrEmpty(value))
                    CachedSettings.Instance.AccessSecret = value;
            }
        }

        /// <summary>
        ///   True: send a text tweet along with the message (the user
        ///   will  be prompted. False: just up-load the image.
        /// </summary>
        public bool Tweet { get; set; }

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
                bool r = (!System.String.IsNullOrEmpty(AccessToken) &&
                          !System.String.IsNullOrEmpty(AccessSecret));
                Tracing.Trace("Settings.Completed? token({0}) secret({1}): {2}",
                              AccessToken, AccessSecret, r);
                return r;
            }
        }
    }
}