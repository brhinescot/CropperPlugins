namespace Cropper.SendToImgur
{
    partial class ImgurOptionsForm
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
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lblKey = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.lblPopBrowser = new System.Windows.Forms.Label();
            this.chkPopBrowser = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
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
            this.tabPage1.Controls.Add(this.qualitySlider);
            this.tabPage1.Controls.Add(this.lblFormat);
            this.tabPage1.Controls.Add(this.cmbImageFormat);
            this.tabPage1.Controls.Add(this.txtKey);
            this.tabPage1.Controls.Add(this.lblKey);
            this.tabPage1.Controls.Add(this.linkLabel1);
            this.tabPage1.Controls.Add(this.lblPopBrowser);
            this.tabPage1.Controls.Add(this.chkPopBrowser);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.btnOK);
            //
            // txtKey
            //
            this.txtKey.Location = new System.Drawing.Point(94, 6);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(214, 20);
            this.txtKey.TabIndex = 11;
            this.tooltip.SetToolTip(txtKey, "The key for the Imgur.com service.\r\nYou can register to get one, free.");
            //
            // lblKey
            //
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(4, 8);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(88, 13);
            this.lblKey.TabIndex = 10;
            this.lblKey.Text = "Key:";
            //
            // linkLabel1
            //
            this.linkLabel1.Text = "get a new key (free)";
            this.linkLabel1.TabIndex = 21;
            this.linkLabel1.Location = new System.Drawing.Point(94, 34);
            this.linkLabel1.Size = new System.Drawing.Size(214, 20);
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkClicked);
            this.tooltip.SetToolTip(linkLabel1, "http://imgur.com/register/api_anon");
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
            // chkPopBrowser
            //
            this.chkPopBrowser.Location = new System.Drawing.Point(94, 130);
            this.chkPopBrowser.Text = "";
            this.chkPopBrowser.Name = "chkPopBrowser";
            this.chkPopBrowser.TabIndex = 71;
            this.tooltip.SetToolTip(chkPopBrowser, "check to pop the browser with each\nnewly uploaded image.");
            //
            // lblPopBrowser
            //
            this.lblPopBrowser.AutoSize = true;
            this.lblPopBrowser.Location = new System.Drawing.Point(4, 134);
            this.lblPopBrowser.Name = "lblPopBrowser";
            this.lblPopBrowser.Size = new System.Drawing.Size(68, 13);
            this.lblPopBrowser.TabIndex = 70;
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
            this.btnCancel.TabIndex = 61;
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
            this.btnOK.TabIndex = 51;
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
            this.Text = "Configure Imgur Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.CheckBox chkPopBrowser;
        private System.Windows.Forms.Label lblPopBrowser;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbImageFormat;
        private System.Windows.Forms.TrackBar qualitySlider;
    }
}