using Fusion8.Cropper.Extensibility;

namespace Cropper.Email
{
    public partial class EmailOptionsForm : BaseConfigurationForm
    {

        private EmailSettings _settings;
        public EmailOptionsForm(EmailSettings settings)
        {
            InitializeComponent();

            _settings = settings;
            this.txtSubjectLine.Text         = _settings.Subject;
            this.txtMessage.Lines            = _settings.Message.Split("\r\n".ToCharArray());
            this.qualitySlider.Value         = _settings.JpgImageQuality;
            this.cmbImageFormat.SelectedItem = settings.ImageFormat;
            HandleQualitySliderValueChanged(null,null);
            SelectedImageFormatChanged(null,null);
        }


        /// <summary>
        ///   Show the OK and Cancel buttons.
        /// </summary>
        ///
        /// <remarks>
        ///   This form can be shown in two ways: as a standalone
        ///   dialog, and hosted within the tabbed "Options" UI provided
        ///   by the Cropper Core.  By default, the OK and Cancel
        ///   buttons are not visible.  When used as a standalone dialog
        ///   the caller should invoke this method before calling
        ///   ShowDialog().
        /// </remarks>
        public void MakeButtonsVisible()
        {
            this.btnOK.Visible = true;
            this.btnCancel.Visible = true;
            this.btnOK.Enabled = true;
            this.btnCancel.Enabled = true;
            this.MinimumSize = new System.Drawing.Size(340, 326);
            this.MaximumSize = new System.Drawing.Size(340, 326);
        }

        public void ApplySettings()
        {
            _settings.Subject = this.txtSubjectLine.Text.Trim();
            _settings.Message = this.txtMessage.Text.Trim();
            _settings.JpgImageQuality =  this.qualitySlider.Value;
            _settings.ImageFormat = this.cmbImageFormat.Text;
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            this.ApplySettings();
        }

        private void SelectedImageFormatChanged(object sender, System.EventArgs e)
        {
            qualitySlider.Enabled =
            qualitySlider.Visible = (this.cmbImageFormat.Text == "jpg");
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
            this.tooltip.SetToolTip(qualitySlider,
                                    "quality=" + qualitySlider.Value.ToString());
        }
    }
}