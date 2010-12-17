using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fusion8.Cropper.Extensibility;

namespace Cropper.SendToImageShack
{
    public partial class ImageShackOptionsForm : BaseConfigurationForm
    {
        private ImageShackSettings _settings;

        public ImageShackOptionsForm(ImageShackSettings settings)
        {
            InitializeComponent();

            _settings                        = settings;
            this.chkCustomTags.Checked       = _settings.UseCustomTags;
            this.txtFixedTags.Text           = _settings.FixedTags;
            this.cmbImageFormat.SelectedItem = _settings.ImageFormat;
            this.qualitySlider.Value         = _settings.JpgImageQuality;
            //this.chkCookie.Checked           = _settings.UseCookie;
            this.chkPopBrowser.Checked       = _settings.PopBrowser;
            HandleQualitySliderValueChanged(null,null);
            SelectedImageFormatChanged(null,null);
            CustomTagsCheckedChanged(null,null);
            UseCookieCheckedChanged(null,null);
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
            _settings.UseCustomTags = this.chkCustomTags.Checked;
            _settings.FixedTags = this.txtFixedTags.Text.Trim();
            //_settings.UseCookie = this.chkCookie.Checked;
            _settings.PopBrowser = this.chkPopBrowser.Checked;
            _settings.JpgImageQuality =  this.qualitySlider.Value;
            _settings.ImageFormat = this.cmbImageFormat.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void SelectedImageFormatChanged(object sender, EventArgs e)
        {
            if (this.cmbImageFormat.Text == "jpg")
            {
                qualitySlider.Visible = true;
                qualitySlider.Enabled = true;
                this.chkPopBrowser.Location = new System.Drawing.Point(94, 130);
                this.lblPopBrowser.Location = new System.Drawing.Point(4, 134);
            }
            else
            {
                qualitySlider.Visible = false;
                qualitySlider.Enabled = false;
                this.chkPopBrowser.Location = new System.Drawing.Point(94, 90);
                this.lblPopBrowser.Location = new System.Drawing.Point(4, 94);
            }
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
            this.tooltip.SetToolTip(qualitySlider,
                                    "quality=" + qualitySlider.Value.ToString());
        }

        private void CustomTagsCheckedChanged(object sender, System.EventArgs e)
        {
            this.lblFixedTags.Enabled = !this.chkCustomTags.Checked;
            this.txtFixedTags.Enabled = !this.chkCustomTags.Checked;
        }

        private void UseCookieCheckedChanged(object sender, System.EventArgs e)
        {
            //this.lblNotImplemented.Visible = this.chkCookie.Checked ;
        }
    }
}
