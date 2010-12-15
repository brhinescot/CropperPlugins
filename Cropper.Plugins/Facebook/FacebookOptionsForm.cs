using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fusion8.Cropper.Extensibility;

namespace Cropper.SendToFacebook
{
    public partial class FacebookOptionsForm : BaseConfigurationForm
    {
        private FacebookSettings _settings;

        public FacebookOptionsForm(FacebookSettings settings)
        {
            InitializeComponent();

            _settings                        = settings;
            this.txtAccessToken.Text         = _settings.AccessToken;
            this.cmbImageFormat.SelectedItem = settings.ImageFormat;
            this.qualitySlider.Value         = _settings.JpgImageQuality;
            this.chkCaption.Checked          = _settings.Caption;
            this.chkPopBrowser.Checked       = _settings.PopBrowser;
            HandleQualitySliderValueChanged(null,null);
            SelectedImageFormatChanged(null,null);

            // only makes sense to clear settings if they are not clear yet.
            this.btnClear.Enabled = !String.IsNullOrEmpty(_settings.AccessToken);
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
            _settings.AccessToken = this.txtAccessToken.Text.Trim();
            _settings.Caption = this.chkCaption.Checked;
            _settings.PopBrowser = this.chkPopBrowser.Checked;
            _settings.JpgImageQuality =  this.qualitySlider.Value;
            _settings.ImageFormat = this.cmbImageFormat.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // erase whatever settings there are
            CachedSettings.Instance.AccessToken =
                _settings.AccessToken = null;
            this.txtAccessToken.Text = "";
            this.btnClear.Enabled = false; // can do only once
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
