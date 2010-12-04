namespace Cropper.Email
{
    public class EmailSettings
    {
        string _format;
        public EmailSettings()
        {
            JpgImageQuality= 80; // default
            Format = "bmp";
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
        public string Format
        {
            get { return _format; }

            set
            {
                var v = value.ToLower();
                if (v == "bmp" || value == "png" || value == "jpg")
                    _format = v;
            }
        }

        public string Subject
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }
    }
}