namespace Cropper.Email
{
    using System;

    public class EmailSettings
    {
        public static readonly String[] DEFAULT_MESSAGE = new String[] {
            "Here's a screenshot for you.",
            "Name: $NAME$",
            "Size: $SIZE$",
            "Taken with Cropper - http://cropper.codeplex.com",
            " "
        };
        string _format;

        /// <summary>
        ///   User-accessible settings for the SendtoEmail plugin.
        /// </summary>
        public EmailSettings()
        {
            JpgImageQuality= 80; // default
            _format = "jpg";
            Message = String.Join( "\r\n", DEFAULT_MESSAGE );
            Subject = "Screen shot: $NAME$";
        }

        /// <summary>
        ///   Quality level to use when saving an image in JPG format.
        /// </summary>
        public int JpgImageQuality
        {
            get;
            set;
        }

        /// <summary>
        ///   The Image format; one of Jpeg, Png, Bmp.
        /// </summary>
        public string ImageFormat
        {
            get { return _format; }

            set
            {
                var v = value.ToLower();
                if (v == "bmp" || value == "png" || value == "jpg")
                    _format = v;
            }
        }

        /// <summary>
        ///   The subject to use in the generated email.
        /// </summary>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        ///   The text message to append to the attached image.
        /// </summary>
        public string Message
        {
            get;
            set;
        }
    }
}