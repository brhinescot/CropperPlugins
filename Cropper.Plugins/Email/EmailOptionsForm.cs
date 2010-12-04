using Fusion8.Cropper.Extensibility;

namespace Cropper.Email
{
    public partial class EmailOptionsForm : BaseConfigurationForm
    {
        private OutputImageFormat format = OutputImageFormat.Bmp; // default


        public EmailOptionsForm(EmailSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            this.Format = _settings.Format;
            this.JpgImageQuality = _settings.JpgImageQuality;

            JpgButtonCheckedChanged(null, null);
        }

        public OutputImageFormat Format
        {
            get
            {
                if (radioPng.Checked)
                    format = OutputImageFormat.Png;
                else if (radioJpg.Checked)
                    format = OutputImageFormat.Jpeg;
                else
                    format = OutputImageFormat.Bmp;

                return format;
            }
            set
            {
                format = value;
                if (format == OutputImageFormat.Png)
                    radioPng.Checked = true;
                else if (format == OutputImageFormat.Jpeg)
                    radioJpg.Checked = true;
                else
                    radioBitmap.Checked = true;
            }
        }

        public int JpgImageQuality
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


        private void btnOK_Click(object sender, EventArgs e)
        {
            this.ApplySettings();
        }

        public void ApplySettings()
        {
            //System.Diagnostics.Debugger.Break();
            if (this.TFS != null)
            {
                settings.TeamServer           = this.TFS.Uri.AbsoluteUri;
                settings.TeamProject          = this.lblTeamProject.Text;
            }
            settings.WorkItemType             = this.cmbWorkItemType.Text;
            settings.DefaultImageName         = this.txtDefaultImageName.Text;
            settings.DefaultImageFormat       = this.cmbDefaultImageFormat.Text;
            settings.DefaultAttachmentComment = this.txtDefaultAttachmentComment.Text;
            settings.ImageEditor              = this.txtImageEditor.Text;
            settings.OpenImageInEditor        = this.cbOpenImageInEditor.Checked;
            settings.JpgImageQuality          = this.JpgImageQuality;

        }

        private void JpgButtonCheckedChanged(object sender, System.EventArgs e)
        {
            qualitySlider.Enabled = radioJpg.Checked;
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
        }
    }
}