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
            this.qualitySlider = new System.Windows.Forms.TrackBar();
            this.cmbWorkItemType = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblTeamServer = new System.Windows.Forms.Label();
            this.lblTeamProject = new System.Windows.Forms.Label();
            this.btnSelectTeamProject = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDefaultImageName = new System.Windows.Forms.TextBox();
            this.cmbDefaultImageFormat = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDefaultAttachmentComment = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbOpenImageInEditor = new System.Windows.Forms.CheckBox();
            this.txtImageEditor = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnBrowseImageEditor = new System.Windows.Forms.Button();
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
            this.tabPage1.Controls.Add(this.btnBrowseImageEditor);
            this.tabPage1.Controls.Add(this.txtImageEditor);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.cbOpenImageInEditor);
            this.tabPage1.Controls.Add(this.txtDefaultAttachmentComment);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.cmbDefaultImageFormat);
            this.tabPage1.Controls.Add(this.qualitySlider);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.txtDefaultImageName);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.lblTeamProject);
            this.tabPage1.Controls.Add(this.lblTeamServer);
            this.tabPage1.Controls.Add(this.btnOK);
            this.tabPage1.Controls.Add(this.btnCancel);
            this.tabPage1.Controls.Add(this.cmbWorkItemType);
            this.tabPage1.Controls.Add(this.btnSelectTeamProject);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Size = new System.Drawing.Size(316, 365);
            this.tabPage1.Text = "Settings";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Team Server:";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Team Project:";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Work Item Type:";
            //
            // lblTeamServer
            //
            this.lblTeamServer.AutoSize = true;
            this.lblTeamServer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblTeamServer.ForeColor = System.Drawing.Color.Blue;
            this.lblTeamServer.Location = new System.Drawing.Point(94, 16);
            this.lblTeamServer.Name = "lblTeamServer";
            this.lblTeamServer.Size = new System.Drawing.Size(156, 13);
            this.lblTeamServer.TabIndex = 11;
            this.lblTeamServer.Text = "(not set)";
            //
            // lblTeamProject
            //
            this.lblTeamProject.AutoSize = true;
            this.lblTeamProject.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblTeamProject.ForeColor = System.Drawing.Color.Blue;
            this.lblTeamProject.Location = new System.Drawing.Point(94, 40);
            this.lblTeamProject.Name = "lblTeamProject";
            this.lblTeamProject.Size = new System.Drawing.Size(159, 13);
            this.lblTeamProject.TabIndex = 12;
            this.lblTeamProject.Text = "(not set)";
            //
            // btnSelectTeamProject
            //
            this.btnSelectTeamProject.ImageIndex = 0;
            this.btnSelectTeamProject.Location = new System.Drawing.Point(270, 16);
            this.btnSelectTeamProject.Name = "btnSelectTeamProject";
            this.btnSelectTeamProject.Size = new System.Drawing.Size(38, 23);
            this.btnSelectTeamProject.TabIndex = 0;
            this.btnSelectTeamProject.Text = "...";
            this.btnSelectTeamProject.UseVisualStyleBackColor = true;
            this.btnSelectTeamProject.Click += new System.EventHandler(this.btnSelectTeamProject_Click);
            //
            // cmbWorkItemType
            //
            this.cmbWorkItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWorkItemType.Enabled = false;
            this.cmbWorkItemType.FormattingEnabled = false;
            this.cmbWorkItemType.Location = new System.Drawing.Point(94, 64);
            this.cmbWorkItemType.Name = "cmbWorkItemType";
            this.cmbWorkItemType.Size = new System.Drawing.Size(214, 21);
            this.cmbWorkItemType.TabIndex = 1;
            this.cmbWorkItemType.SelectionChangeCommitted += new System.EventHandler(this.cmbWorkItemType_SelectedIndexChanged);
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Image Name:";
            //
            // txtDefaultImageName
            //
            this.txtDefaultImageName.Location = new System.Drawing.Point(94, 88);
            this.txtDefaultImageName.Name = "txtDefaultImageName";
            this.txtDefaultImageName.Size = new System.Drawing.Size(214, 21);
            this.txtDefaultImageName.TabIndex = 2;
            //
            // cmbDefaultImageFormat
            //
            this.cmbDefaultImageFormat.FormattingEnabled = false;
            this.cmbDefaultImageFormat.AllowDrop = false;
            this.cmbDefaultImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultImageFormat.Items.AddRange(new object[] {
            "png",
            "jpg",
            "bmp"});
            this.cmbDefaultImageFormat.Location = new System.Drawing.Point(94, 112);
            this.cmbDefaultImageFormat.Name = "cmbDefaultImageFormat";
            this.cmbDefaultImageFormat.Size = new System.Drawing.Size(72, 21);
            this.cmbDefaultImageFormat.TabIndex = 3;
            this.cmbDefaultImageFormat.SelectionChangeCommitted += new System.EventHandler(this.SelectedImageFormatChanged);
            //
            // label6
            //
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Output Format:";
            //
            // qualitySlider
            //
            this.qualitySlider.LargeChange = 10;
            this.qualitySlider.Location = new System.Drawing.Point(94, 136);
            this.qualitySlider.Maximum = 100;
            this.qualitySlider.Minimum = 10;
            this.qualitySlider.Name = "qualitySlider";
            this.qualitySlider.Size = new System.Drawing.Size(214, 34);
            this.qualitySlider.TabIndex = 7;
            this.qualitySlider.TickFrequency = 10;
            this.qualitySlider.Value = 80;
            this.qualitySlider.ValueChanged += new System.EventHandler(this.HandleQualitySliderValueChanged);
            //
            // txtDefaultAttachmentComment
            //
            this.txtDefaultAttachmentComment.Location = new System.Drawing.Point(94, 180);
            this.txtDefaultAttachmentComment.Name = "txtDefaultAttachmentComment";
            this.txtDefaultAttachmentComment.Size = new System.Drawing.Size(214, 21);
            this.txtDefaultAttachmentComment.TabIndex = 12;
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 182);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(153, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Comment:";
            //
            // btnBrowseImageEditor
            //
            this.btnBrowseImageEditor.Image = global::Cropper.TFSWorkItem.Properties.Resources.folder_image;
            this.btnBrowseImageEditor.Location = new System.Drawing.Point(202, 210);
            this.btnBrowseImageEditor.Name = "btnBrowseImageEditor";
            this.btnBrowseImageEditor.Size = new System.Drawing.Size(21, 21);
            this.btnBrowseImageEditor.TabIndex = 24;
            this.btnBrowseImageEditor.UseVisualStyleBackColor = true;
            this.btnBrowseImageEditor.Click += new System.EventHandler(this.btnBrowseImageEditor_Click);
            //
            // cbOpenImageInEditor
            //
            this.cbOpenImageInEditor.AutoSize = true;
            this.cbOpenImageInEditor.Location = new System.Drawing.Point(6, 210);
            this.cbOpenImageInEditor.Name = "cbOpenImageInEditor";
            this.cbOpenImageInEditor.Size = new System.Drawing.Size(213, 17);
            this.cbOpenImageInEditor.TabIndex = 21;
            this.cbOpenImageInEditor.Text = "Edit image before attaching";
            this.cbOpenImageInEditor.UseVisualStyleBackColor = true;
            //
            // txtImageEditor
            //
            this.txtImageEditor.Location = new System.Drawing.Point(94, 232);
            this.txtImageEditor.Name = "txtImageEditor";
            this.txtImageEditor.Size = new System.Drawing.Size(214, 21);
            this.txtImageEditor.TabIndex = 28;
            //
            // label7
            //
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 234);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Image Editor:";
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(232, 274);
            this.btnCancel.Visible = false;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 23);
            this.btnCancel.TabIndex = 40;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // btnOK
            //
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Visible = false;
            this.btnOK.Location = new System.Drawing.Point(152, 274);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 30;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // OptionsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 391);
            this.Name = "OptionsForm";
            this.Text = "Cropper - Configure TFS Work Item Capture";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip tooltip;
        private System.Windows.Forms.Button btnSelectTeamProject;
        private System.Windows.Forms.ComboBox cmbWorkItemType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTeamServer;
        private System.Windows.Forms.Label lblTeamProject;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDefaultImageName;
        private System.Windows.Forms.ComboBox cmbDefaultImageFormat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDefaultAttachmentComment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbOpenImageInEditor;
        private System.Windows.Forms.TextBox txtImageEditor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnBrowseImageEditor;
        private System.Windows.Forms.TrackBar qualitySlider;
    }
}