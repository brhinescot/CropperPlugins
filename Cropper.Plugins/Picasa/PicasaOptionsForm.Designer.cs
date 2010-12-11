namespace Cropper.SendToPicasa
{
    partial class PicasaOptionsForm
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
            this.txtEmailAddress = new System.Windows.Forms.TextBox();
            this.lblEmailAddress = new System.Windows.Forms.Label();
            this.txtAllPhotosComment = new System.Windows.Forms.TextBox();
            this.lblAllPhotosComment = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.lblFixedComment = new System.Windows.Forms.Label();
            this.chkUseFixedComment = new System.Windows.Forms.CheckBox();
            this.chkPopBrowser = new System.Windows.Forms.CheckBox();
            this.lblPopBrowser = new System.Windows.Forms.Label();
            this.cmbAlbum = new System.Windows.Forms.ComboBox();
            this.lblAlbum = new System.Windows.Forms.Label();
            this.btnRefreshAlbumList = new System.Windows.Forms.Button();
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
            this.tabPage1.Controls.Add(this.lblAlbum);
            this.tabPage1.Controls.Add(this.btnRefreshAlbumList);
            this.tabPage1.Controls.Add(this.cmbAlbum);
            this.tabPage1.Controls.Add(this.lblPopBrowser);
            this.tabPage1.Controls.Add(this.chkPopBrowser);
            this.tabPage1.Controls.Add(this.lblFixedComment);
            this.tabPage1.Controls.Add(this.chkUseFixedComment);
            this.tabPage1.Controls.Add(this.qualitySlider);
            this.tabPage1.Controls.Add(this.lblFormat);
            this.tabPage1.Controls.Add(this.cmbImageFormat);
            this.tabPage1.Controls.Add(this.txtEmailAddress);
            this.tabPage1.Controls.Add(this.lblEmailAddress);
            this.tabPage1.Controls.Add(this.txtAllPhotosComment);
            this.tabPage1.Controls.Add(this.lblAllPhotosComment);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.btnOK);
            //
            // txtEmailAddress
            //
            this.txtEmailAddress.Location = new System.Drawing.Point(114, 6);
            this.txtEmailAddress.Name = "txtEmailAddress";
            this.txtEmailAddress.Size = new System.Drawing.Size(204, 20);
            this.txtEmailAddress.TabIndex = 11;
            this.tooltip.SetToolTip(txtEmailAddress, "The email address you use to\nauthenticate to Google.");
            //
            // lblEmailAddress
            //
            this.lblEmailAddress.AutoSize = true;
            this.lblEmailAddress.Location = new System.Drawing.Point(4, 8);
            this.lblEmailAddress.Name = "lblEmailAddress";
            this.lblEmailAddress.Size = new System.Drawing.Size(66, 13);
            this.lblEmailAddress.TabIndex = 10;
            this.lblEmailAddress.Text = "email address:";
            //
            // cmbImageFormat
            //
            this.cmbImageFormat.FormattingEnabled = false;
            this.cmbImageFormat.AllowDrop = false;
            this.cmbImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImageFormat.Items.AddRange(new object[] {
            "png",
            "bmp",
            "jpg" });
            this.cmbImageFormat.Location = new System.Drawing.Point(114, 34);
            this.cmbImageFormat.Name = "cmbImageFormat";
            this.cmbImageFormat.Size = new System.Drawing.Size(72, 21);
            this.cmbImageFormat.TabIndex = 21;
            this.cmbImageFormat.SelectionChangeCommitted += new System.EventHandler(this.SelectedImageFormatChanged);
            //
            // lblFormat
            //
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(4, 36);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(50, 13);
            this.lblFormat.TabIndex = 20;
            this.lblFormat.Text = "Output Format:";
            //
            // qualitySlider
            //
            this.qualitySlider.LargeChange = 10;
            this.qualitySlider.Location = new System.Drawing.Point(114, 64);
            this.qualitySlider.Maximum = 100;
            this.qualitySlider.Minimum = 10;
            this.qualitySlider.Name = "qualitySlider";
            this.qualitySlider.Size = new System.Drawing.Size(204, 34);
            this.qualitySlider.TabIndex = 31;
            this.qualitySlider.TickFrequency = 10;
            this.qualitySlider.Value = 80;
            this.qualitySlider.ValueChanged += new System.EventHandler(this.HandleQualitySliderValueChanged);
            //
            // chkUseFixedComment
            //
            this.chkUseFixedComment.Location = new System.Drawing.Point(114, 102);
            this.chkUseFixedComment.Text = "";
            this.chkUseFixedComment.Name = "chkUseFixedComment";
            this.chkUseFixedComment.TabIndex = 41;
            this.chkUseFixedComment.CheckedChanged += UseFixedCommentCheckedChanged;
            this.tooltip.SetToolTip(chkUseFixedComment, "Check to use a standard summary or comment\nfor each image that is uploaded. If unchecked,\nyou will be prompted for a comment for each\nupload.");
            //
            // lblFixedComment
            //
            this.lblFixedComment.AutoSize = true;
            this.lblFixedComment.Location = new System.Drawing.Point(4, 104);
            this.lblFixedComment.Name = "lblFixedComment";
            this.lblFixedComment.Size = new System.Drawing.Size(68, 13);
            this.lblFixedComment.TabIndex = 40;
            this.lblFixedComment.Text = "Standard comment?";
            //
            // txtAllPhotosComment
            //
            this.txtAllPhotosComment.Location = new System.Drawing.Point(114, 130);
            this.txtAllPhotosComment.Name = "txtAllPhotosComment";
            this.txtAllPhotosComment.Size = new System.Drawing.Size(204, 48);
            this.txtAllPhotosComment.Multiline = true;
            this.txtAllPhotosComment.TabIndex = 51;
            this.tooltip.SetToolTip(txtAllPhotosComment, "The summary comment to use for all uploads.");
            //
            // lblAllPhotosComment
            //
            this.lblAllPhotosComment.AutoSize = true;
            this.lblAllPhotosComment.Location = new System.Drawing.Point(38, 132);
            this.lblAllPhotosComment.Name = "lblAllPhotosComment";
            this.lblAllPhotosComment.Size = new System.Drawing.Size(60, 13);
            this.lblAllPhotosComment.TabIndex = 50;
            this.lblAllPhotosComment.Text = "comment:";
            //
            // cmbAlbum
            //
            this.cmbAlbum.FormattingEnabled = false;
            this.cmbAlbum.AllowDrop = false;
            this.cmbAlbum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlbum.Location = new System.Drawing.Point(114, 186);
            this.cmbAlbum.Name = "cmbAlbum";
            this.cmbAlbum.Size = new System.Drawing.Size(122, 21);
            this.cmbAlbum.DisplayMember = "Name";
            this.cmbAlbum.ValueMember = "Id";
            this.cmbAlbum.TabIndex = 61;
            //
            // lblAlbum
            //
            this.lblAlbum.AutoSize = true;
            this.lblAlbum.Location = new System.Drawing.Point(4, 188);
            this.lblAlbum.Name = "lblAlbum";
            this.lblAlbum.Size = new System.Drawing.Size(50, 13);
            this.lblAlbum.TabIndex = 60;
            this.lblAlbum.Text = "Album:";
            //
            // btnRefreshAlbumList
            //
            this.btnRefreshAlbumList.Image = global::Cropper.SendToPicasa.Properties.Resources.refresh;
            this.btnRefreshAlbumList.Location = new System.Drawing.Point(58, 188);
            this.btnRefreshAlbumList.Name = "btnRefreshAlbumList";
            this.btnRefreshAlbumList.Size = new System.Drawing.Size(20, 20);
            this.btnRefreshAlbumList.TabIndex = 64;
            this.btnRefreshAlbumList.UseVisualStyleBackColor = true;
            this.btnRefreshAlbumList.Click += new System.EventHandler(this.btnRefreshAlbumList_Click);
            this.tooltip.SetToolTip(btnRefreshAlbumList, "Refresh");
            //
            // chkPopBrowser
            //
            this.chkPopBrowser.Location = new System.Drawing.Point(114, 214);
            this.chkPopBrowser.Text = "";
            this.chkPopBrowser.Name = "chkPopBrowser";
            this.chkPopBrowser.TabIndex = 71;
            this.tooltip.SetToolTip(chkPopBrowser, "check to pop the browser with each\nnewly uploaded image.");
            //
            // lblPopBrowser
            //
            this.lblPopBrowser.AutoSize = true;
            this.lblPopBrowser.Location = new System.Drawing.Point(4, 219);
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
            this.btnCancel.TabIndex = 91;
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
            this.Text = "Configure Picasa Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnRefreshAlbumList;
        private System.Windows.Forms.ComboBox cmbAlbum;
        private System.Windows.Forms.Label lblAlbum;
        private System.Windows.Forms.TextBox txtAllPhotosComment;
        private System.Windows.Forms.Label lblAllPhotosComment;
        private System.Windows.Forms.CheckBox chkUseFixedComment;
        private System.Windows.Forms.Label lblFixedComment;
        private System.Windows.Forms.CheckBox chkPopBrowser;
        private System.Windows.Forms.Label lblPopBrowser;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.TextBox txtEmailAddress;
        private System.Windows.Forms.Label lblEmailAddress;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbImageFormat;
        private System.Windows.Forms.TrackBar qualitySlider;
    }
}