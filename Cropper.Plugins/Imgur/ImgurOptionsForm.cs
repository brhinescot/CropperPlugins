using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Common;

namespace Cropper.SendToImgur
{
    public partial class ImgurOptionsForm : BaseConfigurationForm
    {
        private ImgurSettings _settings;

        public ImgurOptionsForm(ImgurSettings settings)
        {
            InitializeComponent();

            _settings                        = settings;
            this.txtKey.Text                 = _settings.Key;
            this.chkPopBrowser.Checked       = _settings.PopBrowser;
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
            Tracing.Trace("ImgurOptionsForm::{0:X8}::ApplySettings key({1})",
                          this.GetHashCode(), this.txtKey.Text.Trim());
            _settings.Key = this.txtKey.Text.Trim();
            _settings.JpgImageQuality =  this.qualitySlider.Value;
            _settings.ImageFormat = this.cmbImageFormat.Text;
            _settings.PopBrowser = this.chkPopBrowser.Checked;
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
                this.chkPopBrowser.Location = new System.Drawing.Point(94, 92);
                this.lblPopBrowser.Location = new System.Drawing.Point(4, 96);
            }
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
            this.tooltip.SetToolTip(qualitySlider,
                                    "quality=" + qualitySlider.Value.ToString());
        }

        protected void LinkClicked(object sender, System.EventArgs e)
        {
            // Change the color of the link text by setting LinkVisited
            // to True.
            linkLabel1.LinkVisited = true;
            string message =
                "You'll now be taken to the Imgur.com site to " +
                "register for an access key. Enter the required " +
                "information on that webpage, and key in the Captcha. " +
                "You'll then be taken, instantly, to a new page showing your new " +
                "\"Developer Key\". Select the key and ctrl-C to " +
                "copy it to the clipboard.  Then return to the prior dialog and use " +
                "ctrl-v to paste the key into the textbox. ";
            string dialogTitle = "Get a key for Imgur.com";
            if (MessageBox.Show(message, dialogTitle,
                               MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {
                System.Diagnostics.Process.Start(this.tooltip.GetToolTip(linkLabel1));
            }
        }

    }
}
