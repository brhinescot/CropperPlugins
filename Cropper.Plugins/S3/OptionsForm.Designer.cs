namespace Cropper.SendToS3
{
    partial class OptionsForm
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
            this.btnRefreshBucketList = new System.Windows.Forms.Button();
            this._txtAccessKeyID = new System.Windows.Forms.TextBox();
            this.lblAccessKeyId = new System.Windows.Forms.Label();
            this.txtSecretAccessKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._cmbBucket = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this._txtBaseKey = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
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
            this.tabPage1.Controls.Add(this._txtBaseKey);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this._cmbBucket);
            this.tabPage1.Controls.Add(this.btnRefreshBucketList);
            this.tabPage1.Controls.Add(this.txtSecretAccessKey);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this._txtAccessKeyID);
            this.tabPage1.Controls.Add(this.lblAccessKeyId);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.btnOK);
            //
            // _txtAccessKeyID
            //
            this._txtAccessKeyID.AutoCompleteCustomSource.AddRange(new string[] {
            "$NAME$",
            "$SIZE$",
            "$OPERATINGSYSTEM$",
            "$COMPUTERNAME$"});
            this._txtAccessKeyID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this._txtAccessKeyID.Location = new System.Drawing.Point(94, 6);
            this._txtAccessKeyID.Name = "_txtAccessKeyID";
            this._txtAccessKeyID.Size = new System.Drawing.Size(214, 20);
            this._txtAccessKeyID.TabIndex = 11;
            //
            // lblAccessKeyId
            //
            this.lblAccessKeyId.AutoSize = true;
            this.lblAccessKeyId.Location = new System.Drawing.Point(4, 8);
            this.lblAccessKeyId.Name = "lblAccessKeyId";
            this.lblAccessKeyId.Size = new System.Drawing.Size(88, 13);
            this.lblAccessKeyId.TabIndex = 10;
            this.lblAccessKeyId.Text = "Access Key ID";
            //
            // txtSecretAccessKey
            //
            this.txtSecretAccessKey.AutoCompleteCustomSource.AddRange(new string[] {
            "$NAME$",
            "$SIZE$",
            "$OPERATINGSYSTEM$",
            "$COMPUTERNAME$"});
            this.txtSecretAccessKey.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtSecretAccessKey.Location = new System.Drawing.Point(94, 30);
            this.txtSecretAccessKey.Name = "txtSecretAccessKey";
            this.txtSecretAccessKey.Size = new System.Drawing.Size(214, 20);
            this.txtSecretAccessKey.TabIndex = 21;
            this.tooltip.SetToolTip(txtSecretAccessKey, "(Secret)");
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Access Key";
            //
            // _cmbBucket
            //
            this._cmbBucket.FormattingEnabled = true;
            this._cmbBucket.Location = new System.Drawing.Point(94, 54);
            this._cmbBucket.Name = "_cmbBucket";
            this._cmbBucket.Size = new System.Drawing.Size(214, 21);
            this._cmbBucket.TabIndex = 31;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Bucket";
            //
            // btnRefreshBucketList
            //
            this.btnRefreshBucketList.Image = global::Cropper.SendToS3.Properties.Resources.refresh;
            this.btnRefreshBucketList.Location = new System.Drawing.Point(58, 56);
            this.btnRefreshBucketList.Name = "btnRefreshBucketList";
            this.btnRefreshBucketList.Size = new System.Drawing.Size(20, 20);
            this.btnRefreshBucketList.TabIndex = 24;
            this.btnRefreshBucketList.UseVisualStyleBackColor = true;
            this.btnRefreshBucketList.Click += new System.EventHandler(this.btnRefreshBucketList_Click);
            this.tooltip.SetToolTip(btnRefreshBucketList, "Refresh");
            //
            // _txtBaseKey
            //
            //this._txtBaseKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._txtBaseKey.AutoCompleteCustomSource.AddRange(new string[] {
            "$NAME$",
            "$SIZE$",
            "$OPERATINGSYSTEM$",
            "$COMPUTERNAME$"});
            this._txtBaseKey.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this._txtBaseKey.Location = new System.Drawing.Point(94, 78);
            this._txtBaseKey.Name = "_txtBaseKey";
            this._txtBaseKey.Size = new System.Drawing.Size(214, 20);
            this._txtBaseKey.TabIndex = 41;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "Base Key";
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(212, 204);
            this.btnCancel.Visible = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 23);
            this.btnCancel.TabIndex = 60;
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
            this.btnOK.TabIndex = 50;
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
            this.Text = "Configure S3 Connection Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRefreshBucketList;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.TextBox _txtAccessKeyID;
        private System.Windows.Forms.Label lblAccessKeyId;
        private System.Windows.Forms.TextBox txtSecretAccessKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _cmbBucket;
        private System.Windows.Forms.TextBox _txtBaseKey;
        private System.Windows.Forms.Label label3;
    }
}