namespace Cropper.SendToS3
{
    partial class Options
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
            this._txtAccessKeyID = new System.Windows.Forms.TextBox();
            this.lblSubjectLine = new System.Windows.Forms.Label();
            this._txtSecretAccessKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._cmbBucket = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this._linkRefreshBucketList = new System.Windows.Forms.LinkLabel();
            this.themedTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._linkRefreshBucketList);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this._cmbBucket);
            this.tabPage1.Controls.Add(this._txtSecretAccessKey);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this._txtAccessKeyID);
            this.tabPage1.Controls.Add(this.lblSubjectLine);
            // 
            // _txtAccessKeyID
            // 
            this._txtAccessKeyID.AutoCompleteCustomSource.AddRange(new string[] {
            "$NAME$",
            "$SIZE$",
            "$OPERATINGSYSTEM$",
            "$COMPUTERNAME$"});
            this._txtAccessKeyID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this._txtAccessKeyID.Location = new System.Drawing.Point(25, 19);
            this._txtAccessKeyID.Name = "_txtAccessKeyID";
            this._txtAccessKeyID.Size = new System.Drawing.Size(259, 20);
            this._txtAccessKeyID.TabIndex = 11;
            // 
            // lblSubjectLine
            // 
            this.lblSubjectLine.AutoSize = true;
            this.lblSubjectLine.Location = new System.Drawing.Point(8, 3);
            this.lblSubjectLine.Name = "lblSubjectLine";
            this.lblSubjectLine.Size = new System.Drawing.Size(77, 13);
            this.lblSubjectLine.TabIndex = 10;
            this.lblSubjectLine.Text = "Access Key ID";
            // 
            // _txtSecretAccessKey
            // 
            this._txtSecretAccessKey.AutoCompleteCustomSource.AddRange(new string[] {
            "$NAME$",
            "$SIZE$",
            "$OPERATINGSYSTEM$",
            "$COMPUTERNAME$"});
            this._txtSecretAccessKey.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this._txtSecretAccessKey.Location = new System.Drawing.Point(25, 68);
            this._txtSecretAccessKey.Name = "_txtSecretAccessKey";
            this._txtSecretAccessKey.Size = new System.Drawing.Size(259, 20);
            this._txtSecretAccessKey.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Secret Access Key";
            // 
            // _cmbBucket
            // 
            this._cmbBucket.FormattingEnabled = true;
            this._cmbBucket.Location = new System.Drawing.Point(25, 115);
            this._cmbBucket.Name = "_cmbBucket";
            this._cmbBucket.Size = new System.Drawing.Size(259, 21);
            this._cmbBucket.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Bucket";
            // 
            // _linkRefreshBucketList
            // 
            this._linkRefreshBucketList.ActiveLinkColor = System.Drawing.Color.DarkOrange;
            this._linkRefreshBucketList.AutoSize = true;
            this._linkRefreshBucketList.LinkColor = System.Drawing.Color.Blue;
            this._linkRefreshBucketList.Location = new System.Drawing.Point(55, 99);
            this._linkRefreshBucketList.Name = "_linkRefreshBucketList";
            this._linkRefreshBucketList.Size = new System.Drawing.Size(44, 13);
            this._linkRefreshBucketList.TabIndex = 16;
            this._linkRefreshBucketList.TabStop = true;
            this._linkRefreshBucketList.Text = "Refresh";
            this._linkRefreshBucketList.VisitedLinkColor = System.Drawing.Color.Blue;
            this._linkRefreshBucketList.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._linkRefreshBucketList_LinkClicked);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 276);
            this.Name = "Options";
            this.Text = "Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox _txtAccessKeyID;
        private System.Windows.Forms.Label lblSubjectLine;
        private System.Windows.Forms.TextBox _txtSecretAccessKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _cmbBucket;
        private System.Windows.Forms.LinkLabel _linkRefreshBucketList;
    }
}