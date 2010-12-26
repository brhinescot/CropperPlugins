namespace Cropper.SendToS3
{
    public class S3Settings
    {
        public string AccessKeyId     { get; set; }
        public string SecretAccessKey { get; set; }
        public string BucketName      { get; set; }
        public string BaseKey         { get; set; }

        public S3Settings()
        {
            BaseKey = "ScreenShot-";
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool Completed
        {
            get
            {
                return !(System.String.IsNullOrEmpty(AccessKeyId) ||
                         System.String.IsNullOrEmpty(SecretAccessKey) ||
                         System.String.IsNullOrEmpty(BucketName));
            }
        }
    }
}
