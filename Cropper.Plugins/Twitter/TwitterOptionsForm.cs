using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fusion8.Cropper.Extensibility;

namespace Cropper.SendToTwitter
{
    public partial class TwitterOptionsForm : BaseConfigurationForm
    {
        private TwitterSettings _settings;

        public TwitterOptionsForm(TwitterSettings settings)
        {
            InitializeComponent();

            _settings                        = settings;
            this.txtAccessToken.Text         = _settings.AccessToken;
            this.txtAccessSecret.Text        = _settings.AccessSecret;
            this.cmbImageFormat.SelectedItem = settings.ImageFormat;
            this.qualitySlider.Value         = _settings.JpgImageQuality;
            this.chkTweet.Checked            = _settings.Tweet;
            this.chkPopBrowser.Checked       = _settings.PopBrowser;
            HandleQualitySliderValueChanged(null,null);
            SelectedImageFormatChanged(null,null);

            // only makes sense to clear settings if they are not clear yet.
            this.btnClear.Enabled = (!String.IsNullOrEmpty(_settings.AccessToken) ||
                                       !String.IsNullOrEmpty(_settings.AccessSecret));
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
            _settings.AccessSecret = this.txtAccessSecret.Text.Trim();
            _settings.Tweet = this.chkTweet.Checked;
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
            CachedSettings.Instance.AccessSecret =
                _settings.AccessToken =
                _settings.AccessSecret = null;
            this.txtAccessToken.Text = "";
            this.txtAccessSecret.Text = "";
            this.btnClear.Enabled = false; // can do only once
        }

        private void SelectedImageFormatChanged(object sender, EventArgs e)
        {
            if (this.cmbImageFormat.Text == "jpg")
            {
                qualitySlider.Visible = true;
                qualitySlider.Enabled = true;
                this.chkTweet.Location = new System.Drawing.Point(94, 158);
                this.lblTweet.Location = new System.Drawing.Point(4, 162);
                this.chkPopBrowser.Location = new System.Drawing.Point(94, 180);
                this.lblPopBrowser.Location = new System.Drawing.Point(4, 184);
            }
            else
            {
                qualitySlider.Visible = false;
                qualitySlider.Enabled = false;
                this.chkTweet.Location = new System.Drawing.Point(94, 118);
                this.lblTweet.Location = new System.Drawing.Point(4, 122);
                this.chkPopBrowser.Location = new System.Drawing.Point(94, 140);
                this.lblPopBrowser.Location = new System.Drawing.Point(4, 144);
            }
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
            this.tooltip.SetToolTip(qualitySlider,
                                    "quality=" + qualitySlider.Value.ToString());
        }
    }
}
