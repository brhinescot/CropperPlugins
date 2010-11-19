using Fusion8.Cropper.Extensibility;

namespace CropperPlugins
{
    public partial class Options : BaseConfigurationForm
    {
        private EmailOutputFormat format = EmailOutputFormat.Bitmap;

        public EmailOutputFormat Format
        {
            get
            {
                if (radioPng.Checked)
                    format = EmailOutputFormat.Png;
                else if (radioJpg.Checked)
                    format = EmailOutputFormat.Jpg;
                else
                    format = EmailOutputFormat.Bitmap;

                return format;
            }
            set
            {
                format = value;
                if (format == EmailOutputFormat.Png)
                    radioPng.Checked = true;
                else if (format == EmailOutputFormat.Jpg)
                    radioJpg.Checked = true;
                else
                    radioBitmap.Checked = true;
            }
        }

        public int ImageQuality
        {
            get { return qualitySlider.Value; }
            set { qualitySlider.Value = value; }
        }

        public string Subject
        {
            get { return txtSubjectLine.Text; }
            set { txtSubjectLine.Text = value; }
        }

        public string Message
        {
            get { return txtMessage.Text; }
            set { txtMessage.Text = value; }
        }

        public Options()
        {
            InitializeComponent();
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
        }
    }
}