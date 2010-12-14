namespace Cropper.SendToTwitPic
{
    partial class TwitPicOptionsForm
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
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.qualitySlider = new System.Windows.Forms.TrackBar();
            this.cmbImageFormat = new System.Windows.Forms.ComboBox();
            this.txtAccessToken = new System.Windows.Forms.TextBox();
            this.lblAccessToken = new System.Windows.Forms.Label();
            this.txtAccessSecret = new System.Windows.Forms.TextBox();
            this.lblAccessSecret = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.lblTweet = new System.Windows.Forms.Label();
            this.chkTweet = new System.Windows.Forms.CheckBox();
            this.lblPopBrowser = new System.Windows.Forms.Label();
            this.chkPopBrowser = new System.Windows.Forms.CheckBox();
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
            this.tabPage1.Controls.Add(this.lblPopBrowser);
            this.tabPage1.Controls.Add(this.chkPopBrowser);
            this.tabPage1.Controls.Add(this.lblTweet);
            this.tabPage1.Controls.Add(this.chkTweet);
            this.tabPage1.Controls.Add(this.qualitySlider);
            this.tabPage1.Controls.Add(this.lblFormat);
            this.tabPage1.Controls.Add(this.cmbImageFormat);
            this.tabPage1.Controls.Add(this.txtAccessSecret);
            this.tabPage1.Controls.Add(this.lblAccessSecret);
            this.tabPage1.Controls.Add(this.txtAccessToken);
            this.tabPage1.Controls.Add(this.lblAccessToken);
            this.tabPage1.Controls.Add(this.btnClear);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.btnOK);
            //
            // txtAccessToken
            //
            this.txtAccessToken.Location = new System.Drawing.Point(94, 6);
            this.txtAccessToken.Name = "txtAccessToken";
            this.txtAccessToken.Size = new System.Drawing.Size(214, 20);
            this.txtAccessToken.TabIndex = 11;
            this.txtAccessToken.Enabled = false;
            this.tooltip.SetToolTip(txtAccessToken, "Your Twitter OAuth access token");
            //
            // lblAccessToken
            //
            this.lblAccessToken.AutoSize = true;
            this.lblAccessToken.Location = new System.Drawing.Point(4, 8);
            this.lblAccessToken.Name = "lblAccessToken";
            this.lblAccessToken.Size = new System.Drawing.Size(66, 13);
            this.lblAccessToken.TabIndex = 10;
            this.lblAccessToken.Text = "Access Token:";
            //
            // txtAccessSecret
            //
            this.txtAccessSecret.Location = new System.Drawing.Point(94, 34);
            this.txtAccessSecret.Name = "txtAccessSecret";
            this.txtAccessSecret.Size = new System.Drawing.Size(214, 20);
            this.txtAccessSecret.TabIndex = 21;
            this.txtAccessSecret.Enabled = false;
            this.tooltip.SetToolTip(txtAccessSecret, "The Twitter access secret.");
            //
            // lblAccessSecret
            //
            this.lblAccessSecret.AutoSize = true;
            this.lblAccessSecret.Location = new System.Drawing.Point(4, 36);
            this.lblAccessSecret.Name = "lblAccessSecret";
            this.lblAccessSecret.Size = new System.Drawing.Size(66, 13);
            this.lblAccessSecret.TabIndex = 10;
            this.lblAccessSecret.Text = "Access Secret:";
            //
            // btnClear
            //
            this.btnClear.Location = new System.Drawing.Point(94, 62);
            this.btnClear.Visible = true;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(136, 23);
            this.btnClear.TabIndex = 71;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.tooltip.SetToolTip(btnClear,
                                    "Erase the Twitter access token and secret.\n" +
                                    "This will require you to re-authorize the plugin.");
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // cmbImageFormat
            //
            this.cmbImageFormat.FormattingEnabled = false;
            this.cmbImageFormat.AllowDrop = false;
            this.cmbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImageFormat.Items.AddRange(new object[] {
            "png",
            "jpg" });
            this.cmbImageFormat.Location = new System.Drawing.Point(94, 90);
            this.cmbImageFormat.Name = "cmbImageFormat";
            this.cmbImageFormat.Size = new System.Drawing.Size(72, 21);
            this.cmbImageFormat.TabIndex = 31;
            this.cmbImageFormat.SelectionChangeCommitted += new System.EventHandler(this.SelectedImageFormatChanged);
            //
            // lblFormat
            //
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(4, 90);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(50, 13);
            this.lblFormat.TabIndex = 30;
            this.lblFormat.Text = "Output Format:";
            //
            // qualitySlider
            //
            this.qualitySlider.LargeChange = 10;
            this.qualitySlider.Location = new System.Drawing.Point(94, 120);
            this.qualitySlider.Maximum = 100;
            this.qualitySlider.Minimum = 10;
            this.qualitySlider.Name = "qualitySlider";
            this.qualitySlider.Size = new System.Drawing.Size(214, 34);
            this.qualitySlider.TabIndex = 41;
            this.qualitySlider.TickFrequency = 10;
            this.qualitySlider.Value = 80;
            this.qualitySlider.ValueChanged += new System.EventHandler(this.HandleQualitySliderValueChanged);
            //
            // chkTweet
            //
            this.chkTweet.Location = new System.Drawing.Point(94, 158);
            this.chkTweet.Text = "";
            this.chkTweet.Name = "chkTweet";
            this.chkTweet.TabIndex = 51;
            this.tooltip.SetToolTip(chkTweet, "Check to be asked for a message to tweet for each upload");
            //
            // lblTweet
            //
            this.lblTweet.AutoSize = true;
            this.lblTweet.Location = new System.Drawing.Point(4, 158);
            this.lblTweet.Name = "lblTweet";
            this.lblTweet.Size = new System.Drawing.Size(68, 13);
            this.lblTweet.TabIndex = 50;
            this.lblTweet.Text = "Tweet uploads?";
            //
            // chkPopBrowser
            //
            this.chkPopBrowser.Location = new System.Drawing.Point(94, 186);
            this.chkPopBrowser.Text = "";
            this.chkPopBrowser.Name = "chkPopBrowser";
            this.chkPopBrowser.TabIndex = 51;
            this.tooltip.SetToolTip(chkPopBrowser, "Check to view the just-uploaded image in the browser.");
            //
            // lblPopBrowser
            //
            this.lblPopBrowser.AutoSize = true;
            this.lblPopBrowser.Location = new System.Drawing.Point(4, 186);
            this.lblPopBrowser.Name = "lblPopBrowser";
            this.lblPopBrowser.Size = new System.Drawing.Size(68, 13);
            this.lblPopBrowser.TabIndex = 50;
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
            this.btnCancel.TabIndex = 71;
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
            this.btnOK.TabIndex = 61;
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
            this.Text = "Configure TwitPic Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblPopBrowser;
        private System.Windows.Forms.CheckBox chkPopBrowser;
        private System.Windows.Forms.CheckBox chkTweet;
        private System.Windows.Forms.Label lblTweet;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.TextBox txtAccessToken;
        private System.Windows.Forms.Label lblAccessToken;
        private System.Windows.Forms.TextBox txtAccessSecret;
        private System.Windows.Forms.Label lblAccessSecret;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbImageFormat;
        private System.Windows.Forms.TrackBar qualitySlider;
    }
}