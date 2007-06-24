namespace Cropper.TFSWorkItem
{
    partial class CaptureOptionsForm
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
            this.rbCreateNewWorkItem = new System.Windows.Forms.RadioButton();
            this.rbAddToExistingWorkItem = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtAttachmentComment = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtImageName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtWorkItemId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectWorkItem = new System.Windows.Forms.Button();
            this.cmbWorkItemType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTeamProject = new System.Windows.Forms.Label();
            this.lblTeamServer = new System.Windows.Forms.Label();
            this.btnSelectTeamProject = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowseImageEditor = new System.Windows.Forms.Button();
            this.txtImageEditor = new System.Windows.Forms.TextBox();
            this.cbOpenImageInEditor = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbCreateNewWorkItem
            // 
            this.rbCreateNewWorkItem.AutoSize = true;
            this.rbCreateNewWorkItem.Checked = true;
            this.rbCreateNewWorkItem.Location = new System.Drawing.Point(16, 72);
            this.rbCreateNewWorkItem.Name = "rbCreateNewWorkItem";
            this.rbCreateNewWorkItem.Size = new System.Drawing.Size(130, 17);
            this.rbCreateNewWorkItem.TabIndex = 0;
            this.rbCreateNewWorkItem.TabStop = true;
            this.rbCreateNewWorkItem.Text = "Create new work item";
            this.rbCreateNewWorkItem.UseVisualStyleBackColor = true;
            this.rbCreateNewWorkItem.CheckedChanged += new System.EventHandler(this.rbCreateNewWorkItem_CheckedChanged);
            // 
            // rbAddToExistingWorkItem
            // 
            this.rbAddToExistingWorkItem.AutoSize = true;
            this.rbAddToExistingWorkItem.Location = new System.Drawing.Point(16, 128);
            this.rbAddToExistingWorkItem.Name = "rbAddToExistingWorkItem";
            this.rbAddToExistingWorkItem.Size = new System.Drawing.Size(161, 17);
            this.rbAddToExistingWorkItem.TabIndex = 1;
            this.rbAddToExistingWorkItem.Text = "Add to an existing work item";
            this.rbAddToExistingWorkItem.UseVisualStyleBackColor = true;
            this.rbAddToExistingWorkItem.CheckedChanged += new System.EventHandler(this.rbAddToExistingWorkItem_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(299, 319);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(384, 319);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtAttachmentComment
            // 
            this.txtAttachmentComment.Location = new System.Drawing.Point(136, 48);
            this.txtAttachmentComment.Name = "txtAttachmentComment";
            this.txtAttachmentComment.Size = new System.Drawing.Size(320, 21);
            this.txtAttachmentComment.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Attachment Comment:";
            // 
            // txtImageName
            // 
            this.txtImageName.Location = new System.Drawing.Point(136, 24);
            this.txtImageName.Name = "txtImageName";
            this.txtImageName.Size = new System.Drawing.Size(152, 21);
            this.txtImageName.TabIndex = 26;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Image Name:";
            // 
            // txtWorkItemId
            // 
            this.txtWorkItemId.Enabled = false;
            this.txtWorkItemId.Location = new System.Drawing.Point(104, 152);
            this.txtWorkItemId.Name = "txtWorkItemId";
            this.txtWorkItemId.Size = new System.Drawing.Size(176, 21);
            this.txtWorkItemId.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Work Item Id:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSelectWorkItem);
            this.groupBox1.Controls.Add(this.cmbWorkItemType);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblTeamProject);
            this.groupBox1.Controls.Add(this.lblTeamServer);
            this.groupBox1.Controls.Add(this.btnSelectTeamProject);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.rbCreateNewWorkItem);
            this.groupBox1.Controls.Add(this.txtWorkItemId);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.rbAddToExistingWorkItem);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 192);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Work Item";
            // 
            // btnSelectWorkItem
            // 
            this.btnSelectWorkItem.Enabled = false;
            this.btnSelectWorkItem.Image = global::Cropper.TFSWorkItem.Properties.Resources.folder_brick;
            this.btnSelectWorkItem.Location = new System.Drawing.Point(280, 152);
            this.btnSelectWorkItem.Name = "btnSelectWorkItem";
            this.btnSelectWorkItem.Size = new System.Drawing.Size(21, 21);
            this.btnSelectWorkItem.TabIndex = 40;
            this.btnSelectWorkItem.UseVisualStyleBackColor = true;
            this.btnSelectWorkItem.Click += new System.EventHandler(this.btnSelectWorkItem_Click);
            // 
            // cmbWorkItemType
            // 
            this.cmbWorkItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkItemType.FormattingEnabled = true;
            this.cmbWorkItemType.Location = new System.Drawing.Point(104, 96);
            this.cmbWorkItemType.Name = "cmbWorkItemType";
            this.cmbWorkItemType.Size = new System.Drawing.Size(176, 21);
            this.cmbWorkItemType.TabIndex = 38;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 39;
            this.label6.Text = "Work Item Type:";
            // 
            // lblTeamProject
            // 
            this.lblTeamProject.AutoSize = true;
            this.lblTeamProject.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblTeamProject.ForeColor = System.Drawing.Color.Blue;
            this.lblTeamProject.Location = new System.Drawing.Point(104, 48);
            this.lblTeamProject.Name = "lblTeamProject";
            this.lblTeamProject.Size = new System.Drawing.Size(159, 13);
            this.lblTeamProject.TabIndex = 37;
            this.lblTeamProject.Text = "{Press button to select project}";
            // 
            // lblTeamServer
            // 
            this.lblTeamServer.AutoSize = true;
            this.lblTeamServer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblTeamServer.ForeColor = System.Drawing.Color.Blue;
            this.lblTeamServer.Location = new System.Drawing.Point(104, 24);
            this.lblTeamServer.Name = "lblTeamServer";
            this.lblTeamServer.Size = new System.Drawing.Size(156, 13);
            this.lblTeamServer.TabIndex = 36;
            this.lblTeamServer.Text = "{Press button to select server}";
            // 
            // btnSelectTeamProject
            // 
            this.btnSelectTeamProject.ImageIndex = 0;
            this.btnSelectTeamProject.Location = new System.Drawing.Point(272, 24);
            this.btnSelectTeamProject.Name = "btnSelectTeamProject";
            this.btnSelectTeamProject.Size = new System.Drawing.Size(88, 23);
            this.btnSelectTeamProject.TabIndex = 33;
            this.btnSelectTeamProject.Text = "Select Project";
            this.btnSelectTeamProject.UseVisualStyleBackColor = true;
            this.btnSelectTeamProject.Click += new System.EventHandler(this.btnSelectTeamProject_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Team Project:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "Team Server:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBrowseImageEditor);
            this.groupBox2.Controls.Add(this.txtImageEditor);
            this.groupBox2.Controls.Add(this.cbOpenImageInEditor);
            this.groupBox2.Controls.Add(this.txtImageName);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtAttachmentComment);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 192);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(478, 112);
            this.groupBox2.TabIndex = 34;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Image";
            // 
            // btnBrowseImageEditor
            // 
            this.btnBrowseImageEditor.Image = global::Cropper.TFSWorkItem.Properties.Resources.folder_image;
            this.btnBrowseImageEditor.Location = new System.Drawing.Point(434, 72);
            this.btnBrowseImageEditor.Name = "btnBrowseImageEditor";
            this.btnBrowseImageEditor.Size = new System.Drawing.Size(21, 21);
            this.btnBrowseImageEditor.TabIndex = 33;
            this.btnBrowseImageEditor.UseVisualStyleBackColor = true;
            // 
            // txtImageEditor
            // 
            this.txtImageEditor.Enabled = false;
            this.txtImageEditor.Location = new System.Drawing.Point(136, 72);
            this.txtImageEditor.Name = "txtImageEditor";
            this.txtImageEditor.Size = new System.Drawing.Size(296, 21);
            this.txtImageEditor.TabIndex = 31;
            // 
            // cbOpenImageInEditor
            // 
            this.cbOpenImageInEditor.AutoSize = true;
            this.cbOpenImageInEditor.Location = new System.Drawing.Point(14, 72);
            this.cbOpenImageInEditor.Name = "cbOpenImageInEditor";
            this.cbOpenImageInEditor.Size = new System.Drawing.Size(100, 17);
            this.cbOpenImageInEditor.TabIndex = 30;
            this.cbOpenImageInEditor.Text = "Open In Editor:";
            this.cbOpenImageInEditor.UseVisualStyleBackColor = true;
            this.cbOpenImageInEditor.CheckedChanged += new System.EventHandler(this.cbOpenImageInEditor_CheckedChanged);
            // 
            // CaptureOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 355);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CaptureOptionsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Capture Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rbCreateNewWorkItem;
        private System.Windows.Forms.RadioButton rbAddToExistingWorkItem;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtAttachmentComment;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtImageName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtWorkItemId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnBrowseImageEditor;
        private System.Windows.Forms.TextBox txtImageEditor;
        private System.Windows.Forms.CheckBox cbOpenImageInEditor;
        private System.Windows.Forms.Label lblTeamProject;
        private System.Windows.Forms.Label lblTeamServer;
        private System.Windows.Forms.Button btnSelectTeamProject;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbWorkItemType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSelectWorkItem;
    }
}