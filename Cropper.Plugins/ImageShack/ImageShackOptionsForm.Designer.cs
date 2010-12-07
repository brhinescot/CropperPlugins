namespace Cropper.SendToImageShack
{
    partial class ImageShackOptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.qualitySlider = new System.Windows.Forms.TrackBar();
            this.cmbImageFormat = new System.Windows.Forms.ComboBox();
            this.txtFixedTags = new System.Windows.Forms.TextBox();
            this.lblFixedTags = new System.Windows.Forms.Label();
            this.lblNotImplemented = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.lblCookie = new System.Windows.Forms.Label();
            this.chkCookie = new System.Windows.Forms.CheckBox();
            this.chkPopBrowser = new System.Windows.Forms.CheckBox();
            this.lblPopBrowser = new System.Windows.Forms.Label();
            this.chkCustomTags = new System.Windows.Forms.CheckBox();
            this.lblCustomTags = new System.Windows.Forms.Label();
            this.themedTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            //
            // themedTabControl1
            //
            this.themedTabControl1.Size = new System.Drawing.Size(324, 391);
            //
            // tooltip
            //
            this.tooltip = new System.Windows.Forms.ToolTip();
            this.tooltip.AutoPopDelay = 2400;
            this.tooltip.InitialDelay = 500;
            this.tooltip.ReshowDelay = 500;
            //
            // tabPage1
            //
            this.tabPage1.Controls.Add(this.lblCookie);
            this.tabPage1.Controls.Add(this.chkCookie);
            this.tabPage1.Controls.Add(this.lblPopBrowser);
            this.tabPage1.Controls.Add(this.chkPopBrowser);
            this.tabPage1.Controls.Add(this.lblCustomTags);
            this.tabPage1.Controls.Add(this.chkCustomTags);
            this.tabPage1.Controls.Add(this.qualitySlider);
            this.tabPage1.Controls.Add(this.lblFormat);
            this.tabPage1.Controls.Add(this.cmbImageFormat);
            this.tabPage1.Controls.Add(this.txtFixedTags);
            this.tabPage1.Controls.Add(this.lblFixedTags);
            this.tabPage1.Controls.Add(this.lblNotImplemented);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.btnOK);
            //
            // chkCustomTags
            //
            this.chkCustomTags.Location = new System.Drawing.Point(94, 5);
            this.chkCustomTags.Text = "";
            this.chkCustomTags.Name = "chkCustomTags";
            this.chkCustomTags.TabIndex = 1;
            this.chkCustomTags.CheckedChanged += CustomTagsCheckedChanged;
            this.tooltip.SetToolTip(chkCustomTags, "check to use custom tags for each upload.");
            //
            // lblCustomTags
            //
            this.lblCustomTags.AutoSize = true;
            this.lblCustomTags.Location = new System.Drawing.Point(4, 8);
            this.lblCustomTags.Name = "lblCustomTags";
            this.lblCustomTags.Size = new System.Drawing.Size(68, 13);
            this.lblCustomTags.TabIndex = 2;
            this.lblCustomTags.Text = "Custom Tags?";
            //
            // txtFixedTags
            //
            this.txtFixedTags.Location = new System.Drawing.Point(94, 34);
            this.txtFixedTags.Name = "txtFixedTags";
            this.txtFixedTags.Size = new System.Drawing.Size(214, 20);
            this.txtFixedTags.TabIndex = 11;
            this.tooltip.SetToolTip(txtFixedTags, "a fixed comma-separated list of tags to use\r\nfor every uploaded image");
            //
            // lblFixedTags
            //
            this.lblFixedTags.AutoSize = true;
            this.lblFixedTags.Location = new System.Drawing.Point(4, 36);
            this.lblFixedTags.Name = "lblFixedTags";
            this.lblFixedTags.Size = new System.Drawing.Size(66, 13);
            this.lblFixedTags.TabIndex = 10;
            this.lblFixedTags.Text = "Fixed Tags:";
            //
            // cmbImageFormat
            //
            this.cmbImageFormat.FormattingEnabled = false;
            this.cmbImageFormat.AllowDrop = false;
            this.cmbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImageFormat.Items.AddRange(new object[] {
                    "png",
                    "jpg",
                    "bmp"});
            this.cmbImageFormat.Location = new System.Drawing.Point(94, 62);
            this.cmbImageFormat.Name = "cmbImageFormat";
            this.cmbImageFormat.Size = new System.Drawing.Size(72, 21);
            this.cmbImageFormat.TabIndex = 31;
            this.cmbImageFormat.SelectionChangeCommitted += new System.EventHandler(this.SelectedImageFormatChanged);
            //
            // lblFormat
            //
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(4, 64);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(50, 13);
            this.lblFormat.TabIndex = 30;
            this.lblFormat.Text = "Output Format:";
            //
            // qualitySlider
            //
            this.qualitySlider.LargeChange = 10;
            this.qualitySlider.Location = new System.Drawing.Point(94, 92);
            this.qualitySlider.Maximum = 100;
            this.qualitySlider.Minimum = 10;
            this.qualitySlider.Name = "qualitySlider";
            this.qualitySlider.Size = new System.Drawing.Size(214, 34);
            this.qualitySlider.TabIndex = 41;
            this.qualitySlider.TickFrequency = 10;
            this.qualitySlider.Value = 80;
            this.qualitySlider.ValueChanged += new System.EventHandler(this.HandleQualitySliderValueChanged);
            //
            // chkCookie
            //
            this.chkCookie.Location = new System.Drawing.Point(94, 129);
            this.chkCookie.Size = new System.Drawing.Size(32, 32);
            this.chkCookie.Text = "";
            this.chkCookie.Name = "chkCookie";
            this.chkCookie.TabIndex = 51;
            this.chkCookie.CheckedChanged += UseCookieCheckedChanged;
            this.tooltip.SetToolTip(chkCookie, "check to use the ImageShack.us cookie, if one exists.");
            //
            // lblCookie
            //
            this.lblCookie.AutoSize = true;
            this.lblCookie.Location = new System.Drawing.Point(4, 137);
            this.lblCookie.Name = "lblCookie";
            this.lblCookie.Size = new System.Drawing.Size(68, 13);
            this.lblCookie.TabIndex = 50;
            this.lblCookie.Text = "Use Cookie?";
            //
            // lblNotImplemented
            //
            this.lblNotImplemented.AutoSize = true;
            this.lblNotImplemented.Location = new System.Drawing.Point(124, 137);
            this.lblNotImplemented.Name = "lblNotImplemented";
            this.lblNotImplemented.Size = new System.Drawing.Size(68, 13);
            this.lblNotImplemented.ForeColor = System.Drawing.Color.Red;
            this.lblNotImplemented.TabIndex = 53;
            //this.lblNotImplemented.Visible = false;
            this.lblNotImplemented.Text = "(not implemented)";
            //
            // chkPopBrowser
            //
            this.chkPopBrowser.Location = new System.Drawing.Point(94, 157);
            this.chkPopBrowser.Text = "";
            this.chkPopBrowser.Name = "chkPopBrowser";
            this.chkPopBrowser.TabIndex = 61;
            this.tooltip.SetToolTip(chkPopBrowser, "check to pop the browser with the newly uploaded image.");
            //
            // lblPopBrowser
            //
            this.lblPopBrowser.AutoSize = true;
            this.lblPopBrowser.Location = new System.Drawing.Point(4, 163);
            this.lblPopBrowser.Name = "lblPopBrowser";
            this.lblPopBrowser.Size = new System.Drawing.Size(68, 13);
            this.lblPopBrowser.TabIndex = 60;
            this.lblPopBrowser.Text = "Pop Browser?";
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(212, 204);
            this.btnCancel.Visible = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 23);
            this.btnCancel.TabIndex = 81;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // btnOK
            //
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Visible = false;
            this.btnOK.Location = new System.Drawing.Point(132, 204);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 71;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // Options
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 391);
            this.Name = "Options";
            this.Text = "Configure ImageShack Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkCookie;
        private System.Windows.Forms.Label lblCookie;
        private System.Windows.Forms.CheckBox chkPopBrowser;
        private System.Windows.Forms.Label lblPopBrowser;
        private System.Windows.Forms.Label lblNotImplemented;
        private System.Windows.Forms.CheckBox chkCustomTags;
        private System.Windows.Forms.Label lblCustomTags;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.TextBox txtFixedTags;
        private System.Windows.Forms.Label lblFixedTags;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbImageFormat;
        private System.Windows.Forms.TrackBar qualitySlider;
    }
}