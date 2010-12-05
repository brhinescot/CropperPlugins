using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fusion8.Cropper.Extensibility;

namespace Cropper.SendToTwitPic
{
    public partial class TwitPicOptionsForm : BaseConfigurationForm
    {
        private TwitPicSettings _settings;

        public TwitPicOptionsForm(TwitPicSettings settings)
        {
            InitializeComponent();

            _settings                        = settings;
            this.txtUsername.Text            = _settings.Username;
            this.txtPassword.Text            = _settings.Password;
            this.cmbImageFormat.SelectedItem = settings.ImageFormat;
            this.qualitySlider.Value         = _settings.JpgImageQuality;
            this.chkTweet.Checked            = _settings.Tweet;
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
            _settings.Username = this.txtUsername.Text.Trim();
            _settings.Password = this.txtPassword.Text.Trim();
            _settings.Tweet = this.chkTweet.Checked;
            _settings.JpgImageQuality =  this.qualitySlider.Value;
            _settings.ImageFormat = this.cmbImageFormat.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void SelectedImageFormatChanged(object sender, EventArgs e)
        {
            qualitySlider.Enabled = (this.cmbImageFormat.Text == "jpg");
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
            this.tooltip.SetToolTip(qualitySlider,
                                    "quality=" + qualitySlider.Value.ToString());
        }

    }
}
