namespace CropperPlugins
{
    public class EmailOutputSettings
    {
        private int imageQuality = 80;
        private EmailOutputFormat format;
        string subject;
        string message;

        public int ImageQuality
        {
            get { return imageQuality; }
            set { imageQuality = value; }
        }

        public EmailOutputFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        public string Subject
        {
            get { return subject; }
            set { subject = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}