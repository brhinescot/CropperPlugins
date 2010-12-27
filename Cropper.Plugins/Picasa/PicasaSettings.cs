// PicasaSettings.cs
// ------------------------------------------------------------------
//
// Description goes here....
//
// Author     : Dino
// Created    : Sat Dec 11 11:51:10 2010
// Last Saved : <2010-December-26 21:42:46>
//
// ------------------------------------------------------------------
//
// Copyright (c) 2010 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

using System;

using CropperPlugins.Common;

namespace Cropper.SendToPicasa
{
    public class AlbumItem
    {
        // these must be properties to be displayed properly by ComboBox
        public String Name { get;set; }
        public String Id  { get;set; }  // default = drop box
    }

    /// <summary>
    ///   A singleton that stores an email address
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
    ///     This class caches the email address across
    ///     instances. EmailAddress is the only field in the Settings
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

        private string _emailAddress;
        public string EmailAddress
        {
            get
            {
                Tracing.Trace("Picasa.CachedSettings::{0:X8}::get_Email '{1}'",
                              this.GetHashCode(), _emailAddress);
                return _emailAddress;
            }

            set
            {
                Tracing.Trace("Picasa.CachedSettings::{0:X8}::set_Email '{1}'",
                              this.GetHashCode(), value);
                _emailAddress = value;
            }
        }

        // explicit nonpublic default constructor
        CachedSettings() { EmailAddress= ""; }

        public static CachedSettings Instance
        {
            get { return instance; }
        }
    }


    public class PicasaSettings
    {
        private string _EmailAddress;
        private string _format;

        public PicasaSettings()
        {
            Tracing.Trace("PicasaSettings::{0:X8}::ctor", this.GetHashCode());

            // defaults
            JpgImageQuality= 80;
            ImageFormat = "jpg";
            AllPhotosComment = "Uploaded by Cropper";
            PopBrowser = true;
            EmailAddress = CachedSettings.Instance.EmailAddress;
            UseFixedComment = false;
            Album = new AlbumItem { Name= "Drop Box", Id = "default" };
        }


        /// <summary>
        ///   The email address known to Google/Picasa.
        /// </summary>
        public string EmailAddress
        {
            get
            {
                Tracing.Trace("PicasaSettings::{0:X8}::get_Email", this.GetHashCode());
                if (!String.IsNullOrEmpty(CachedSettings.Instance.EmailAddress)
                    && String.IsNullOrEmpty(_EmailAddress))
                    _EmailAddress= CachedSettings.Instance.EmailAddress;

                return _EmailAddress;
            }
            set
            {
                Tracing.Trace("PicasaSettings::{0:X8}::set_Email", this.GetHashCode());
                _EmailAddress = value;
                if (!String.IsNullOrEmpty(value))
                    CachedSettings.Instance.EmailAddress = value;
            }
        }

        /// <summary>
        ///   True: use the same summary (or comment) for every uploaded
        ///   photo.
        ///   False: the user will be asked to specify a different
        ///   summary for each uploaded photo.
        /// </summary>
        public bool UseFixedComment { get; set; }

        /// <summary>
        ///   The comment to use for all uploaded photos, when
        ///   CommentEachTime is false.
        /// </summary>
        public string AllPhotosComment { get; set; }

        /// <summary>
        ///   The picasa album to upload to.
        /// </summary>
        public AlbumItem Album { get; set; }

        /// <summary>
        ///   True: view the uploaded image in the browser after upload.
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
        ///   The original Image format; one of Jpeg, Bmp, Png.
        ///   Picasa converts them all to jpg after upload, I think.
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