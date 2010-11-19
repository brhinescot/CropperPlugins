namespace Cropper.TFSWorkItem
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbWorkItemType = new System.Windows.Forms.ComboBox();
            this.cbDoNotShowAgain = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblTeamServer = new System.Windows.Forms.Label();
            this.lblTeamProject = new System.Windows.Forms.Label();
            this.btnSelectTeamProject = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDefaultImageName = new System.Windows.Forms.TextBox();
            this.cmbDefaultOutputExtension = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDefaultAttachmentComment = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbOpenImageInEditor = new System.Windows.Forms.CheckBox();
            this.txtImageEditor = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnBrowseImageEditor = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Team Server:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Team Project:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Work Item Type:";
            // 
            // cmbWorkItemType
            // 
            this.cmbWorkItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkItemType.Enabled = false;
            this.cmbWorkItemType.FormattingEnabled = true;
            this.cmbWorkItemType.Location = new System.Drawing.Point(168, 64);
            this.cmbWorkItemType.Name = "cmbWorkItemType";
            this.cmbWorkItemType.Size = new System.Drawing.Size(256, 21);
            this.cmbWorkItemType.TabIndex = 1;
            this.cmbWorkItemType.SelectedIndexChanged += new System.EventHandler(this.cmbWorkItemType_SelectedIndexChanged);
            // 
            // cbDoNotShowAgain
            // 
            this.cbDoNotShowAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDoNotShowAgain.AutoSize = true;
            this.cbDoNotShowAgain.Location = new System.Drawing.Point(259, 265);
            this.cbDoNotShowAgain.Name = "cbDoNotShowAgain";
            this.cbDoNotShowAgain.Size = new System.Drawing.Size(166, 17);
            this.cbDoNotShowAgain.TabIndex = 8;
            this.cbDoNotShowAgain.Text = "Do not show this dialog again";
            this.cbDoNotShowAgain.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(347, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(267, 228);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblTeamServer
            // 
            this.lblTeamServer.AutoSize = true;
            this.lblTeamServer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblTeamServer.ForeColor = System.Drawing.Color.Blue;
            this.lblTeamServer.Location = new System.Drawing.Point(168, 16);
            this.lblTeamServer.Name = "lblTeamServer";
            this.lblTeamServer.Size = new System.Drawing.Size(156, 13);
            this.lblTeamServer.TabIndex = 11;
            this.lblTeamServer.Text = "{Press button to select server}";
            // 
            // lblTeamProject
            // 
            this.lblTeamProject.AutoSize = true;
            this.lblTeamProject.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblTeamProject.ForeColor = System.Drawing.Color.Blue;
            this.lblTeamProject.Location = new System.Drawing.Point(168, 40);
            this.lblTeamProject.Name = "lblTeamProject";
            this.lblTeamProject.Size = new System.Drawing.Size(159, 13);
            this.lblTeamProject.TabIndex = 12;
            this.lblTeamProject.Text = "{Press button to select project}";
            // 
            // btnSelectTeamProject
            // 
            this.btnSelectTeamProject.ImageIndex = 0;
            this.btnSelectTeamProject.Location = new System.Drawing.Point(336, 16);
            this.btnSelectTeamProject.Name = "btnSelectTeamProject";
            this.btnSelectTeamProject.Size = new System.Drawing.Size(88, 23);
            this.btnSelectTeamProject.TabIndex = 0;
            this.btnSelectTeamProject.Text = "Select Project";
            this.btnSelectTeamProject.UseVisualStyleBackColor = true;
            this.btnSelectTeamProject.Click += new System.EventHandler(this.btnSelectTeamProject_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Default Image Name:";
            // 
            // txtDefaultImageName
            // 
            this.txtDefaultImageName.Location = new System.Drawing.Point(168, 88);
            this.txtDefaultImageName.Name = "txtDefaultImageName";
            this.txtDefaultImageName.Size = new System.Drawing.Size(256, 21);
            this.txtDefaultImageName.TabIndex = 2;
            // 
            // cmbDefaultOutputExtension
            // 
            this.cmbDefaultOutputExtension.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultOutputExtension.FormattingEnabled = true;
            this.cmbDefaultOutputExtension.Items.AddRange(new object[] {
            "png",
            "jpg",
            "bmp"});
            this.cmbDefaultOutputExtension.Location = new System.Drawing.Point(168, 112);
            this.cmbDefaultOutputExtension.Name = "cmbDefaultOutputExtension";
            this.cmbDefaultOutputExtension.Size = new System.Drawing.Size(72, 21);
            this.cmbDefaultOutputExtension.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Default Output Extension:";
            // 
            // txtDefaultAttachmentComment
            // 
            this.txtDefaultAttachmentComment.Location = new System.Drawing.Point(168, 136);
            this.txtDefaultAttachmentComment.Name = "txtDefaultAttachmentComment";
            this.txtDefaultAttachmentComment.Size = new System.Drawing.Size(256, 21);
            this.txtDefaultAttachmentComment.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Default Attachment Comment:";
            // 
            // cbOpenImageInEditor
            // 
            this.cbOpenImageInEditor.AutoSize = true;
            this.cbOpenImageInEditor.Location = new System.Drawing.Point(16, 168);
            this.cbOpenImageInEditor.Name = "cbOpenImageInEditor";
            this.cbOpenImageInEditor.Size = new System.Drawing.Size(213, 17);
            this.cbOpenImageInEditor.TabIndex = 21;
            this.cbOpenImageInEditor.Text = "Open Image In Editor Before Attaching";
            this.cbOpenImageInEditor.UseVisualStyleBackColor = true;
            // 
            // txtImageEditor
            // 
            this.txtImageEditor.Location = new System.Drawing.Point(168, 192);
            this.txtImageEditor.Name = "txtImageEditor";
            this.txtImageEditor.Size = new System.Drawing.Size(232, 21);
            this.txtImageEditor.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 192);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Image Editor:";
            // 
            // btnBrowseImageEditor
            // 
            this.btnBrowseImageEditor.Image = global::Cropper.TFSWorkItem.Properties.Resources.folder_image;
            this.btnBrowseImageEditor.Location = new System.Drawing.Point(402, 192);
            this.btnBrowseImageEditor.Name = "btnBrowseImageEditor";
            this.btnBrowseImageEditor.Size = new System.Drawing.Size(21, 21);
            this.btnBrowseImageEditor.TabIndex = 24;
            this.btnBrowseImageEditor.UseVisualStyleBackColor = true;
            this.btnBrowseImageEditor.Click += new System.EventHandler(this.btnBrowseImageEditor_Click);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 297);
            this.Controls.Add(this.btnBrowseImageEditor);
            this.Controls.Add(this.txtImageEditor);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbOpenImageInEditor);
            this.Controls.Add(this.txtDefaultAttachmentComment);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbDefaultOutputExtension);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtDefaultImageName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTeamProject);
            this.Controls.Add(this.lblTeamServer);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cbDoNotShowAgain);
            this.Controls.Add(this.cmbWorkItemType);
            this.Controls.Add(this.btnSelectTeamProject);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cropper Plugin - TFS Work Item";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSelectTeamProject;
        private System.Windows.Forms.ComboBox cmbWorkItemType;
        private System.Windows.Forms.CheckBox cbDoNotShowAgain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblTeamServer;
        private System.Windows.Forms.Label lblTeamProject;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDefaultImageName;
        private System.Windows.Forms.ComboBox cmbDefaultOutputExtension;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDefaultAttachmentComment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbOpenImageInEditor;
        private System.Windows.Forms.TextBox txtImageEditor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnBrowseImageEditor;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}