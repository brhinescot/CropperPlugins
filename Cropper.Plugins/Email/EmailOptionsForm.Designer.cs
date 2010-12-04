#region License Information

/**********************************************************************************
Shared Source License for Cropper
Copyright 9/07/2004 Brian Scott
http://blogs.geekdojo.net/brian

This license governs use of the accompanying software ('Software'), and your
use of the Software constitutes acceptance of this license.

You may use the Software for any commercial or noncommercial purpose,
including distributing derivative works.

In return, we simply require that you agree:
1. Not to remove any copyright or other notices from the Software.
2. That if you distribute the Software in source code form you do so only
   under this license (i.e. you must include a complete copy of this license
   with your distribution), and if you distribute the Software solely in
   object form you only do so under a license that complies with this
   license.
3. That the Software comes "as is", with no warranties. None whatsoever.
   This means no express, implied or statutory warranty, including without
   limitation, warranties of merchantability or fitness for a particular
   purpose or any warranty of title or non-infringement. Also, you must pass
   this disclaimer on whenever you distribute the Software or derivative
   works.
4. That no contributor to the Software will be liable for any of those types
   of damages known as indirect, special, consequential, or incidental
   related to the Software or this license, to the maximum extent the law
   permits, no matter what legal theory it’s based on. Also, you must pass
   this limitation of liability on whenever you distribute the Software or
   derivative works.
5. That if you sue anyone over patents that you think may apply to the
   Software for a person's use of the Software, your license to the Software
   ends automatically.
6. That the patent rights, if any, granted in this license only apply to the
   Software, not to any derivative works you make.
7. That the Software is subject to U.S. export jurisdiction at the time it
   is licensed to you, and it may be subject to additional export or import
   laws in other places.  You agree to comply with all such laws and
   regulations that may apply to the Software after delivery of the software
   to you.
8. That if you are an agency of the U.S. Government, (i) Software provided
   pursuant to a solicitation issued on or after December 1, 1995, is
   provided with the commercial license rights set forth in this license,
   and (ii) Software provided pursuant to a solicitation issued prior to
   December 1, 1995, is provided with “Restricted Rights” as set forth in
   FAR, 48 C.F.R. 52.227-14 (June 1987) or DFAR, 48 C.F.R. 252.227-7013
   (Oct 1988), as applicable.
9. That your rights under this License end automatically if you breach it in
   any way.
10.That all rights not expressly granted to you in this license are reserved.

**********************************************************************************/

#endregion

namespace Cropper.Email
{
    partial class EmailOptionsForm
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
            this.radioBitmap = new System.Windows.Forms.RadioButton();
            this.radioJpg = new System.Windows.Forms.RadioButton();
            this.radioPng = new System.Windows.Forms.RadioButton();
            this.qualitySlider = new System.Windows.Forms.TrackBar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lblSubjectLine = new System.Windows.Forms.Label();
            this.txtSubjectLine = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.themedTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qualitySlider)).BeginInit();
            this.SuspendLayout();
            //
            // themedTabControl1
            //
            this.themedTabControl1.Size = new System.Drawing.Size(324, 391);
            //
            // tabPage1
            //
            this.tabPage1.Controls.Add(this.txtMessage);
            this.tabPage1.Controls.Add(this.lblMessage);
            this.tabPage1.Controls.Add(this.txtSubjectLine);
            this.tabPage1.Controls.Add(this.lblSubjectLine);
            this.tabPage1.Controls.Add(this.qualitySlider);
            this.tabPage1.Controls.Add(this.radioBitmap);
            this.tabPage1.Controls.Add(this.radioJpg);
            this.tabPage1.Controls.Add(this.radioPng);
            this.tabPage1.Size = new System.Drawing.Size(316, 365);
            this.tabPage1.Text = "Format";
            //
            // cmbImageFormat
            //
            this.cmbDefaultImageFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultImageFormat.FormattingEnabled = false;
            this.cmbDefaultImageFormat.Items.AddRange(new object[] {
            "png",
            "jpg",
            "bmp"});
            this.cmbDefaultImageFormat.Location = new System.Drawing.Point(94, 112);
            this.cmbDefaultImageFormat.Name = "cmbDefaultOutputExtension";
            this.cmbDefaultImageFormat.Size = new System.Drawing.Size(72, 21);
            this.cmbDefaultImageFormat.TabIndex = 3;

            //
            // radioBitmap
            //
            this.radioBitmap.AutoSize = true;
            this.radioBitmap.Checked = true;
            this.radioBitmap.Location = new System.Drawing.Point(21, 217);
            this.radioBitmap.Name = "radioBitmap";
            this.radioBitmap.Size = new System.Drawing.Size(57, 17);
            this.radioBitmap.TabIndex = 0;
            this.radioBitmap.TabStop = true;
            this.radioBitmap.Text = "&Bmp";
            this.radioBitmap.UseVisualStyleBackColor = true;
            //
            // radioJpg
            //
            this.radioJpg.AutoSize = true;
            this.radioJpg.Location = new System.Drawing.Point(21, 263);
            this.radioJpg.Name = "radioJpg";
            this.radioJpg.Size = new System.Drawing.Size(48, 17);
            this.radioJpg.TabIndex = 4;
            this.radioJpg.Text = "&Jpeg";
            this.radioJpg.UseVisualStyleBackColor = true;
            this.radioJpg.CheckedChanged += new System.EventHandler(this.JpgButtonCheckedChanged);
            //
            // radioPng
            //
            this.radioPng.AutoSize = true;
            this.radioPng.Location = new System.Drawing.Point(21, 240);
            this.radioPng.Name = "radioPng";
            this.radioPng.Size = new System.Drawing.Size(44, 17);
            this.radioPng.TabIndex = 2;
            this.radioPng.Text = "&Png";
            this.radioPng.UseVisualStyleBackColor = true;
            //
            // qualitySlider
            //
            this.qualitySlider.LargeChange = 10;
            this.qualitySlider.Location = new System.Drawing.Point(35, 286);
            this.qualitySlider.Maximum = 100;
            this.qualitySlider.Minimum = 10;
            this.qualitySlider.Name = "qualitySlider";
            this.qualitySlider.Size = new System.Drawing.Size(259, 45);
            this.qualitySlider.TabIndex = 7;
            this.qualitySlider.TickFrequency = 10;
            this.qualitySlider.Value = 80;
            this.qualitySlider.ValueChanged += new System.EventHandler(this.HandleQualitySliderValueChanged);
            //
            // tabPage2
            //
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(341, 315);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            //
            // lblSubjectLine
            //
            this.lblSubjectLine.AutoSize = true;
            this.lblSubjectLine.Location = new System.Drawing.Point(21, 7);
            this.lblSubjectLine.Name = "lblSubjectLine";
            this.lblSubjectLine.Size = new System.Drawing.Size(66, 13);
            this.lblSubjectLine.TabIndex = 8;
            this.lblSubjectLine.Text = "Subject Line";
            //
            // txtSubjectLine
            //
            this.txtSubjectLine.AutoCompleteCustomSource.AddRange(new string[] {
            "$NAME$",
            "$SIZE$",
            "$OPERATINGSYSTEM$",
            "$COMPUTERNAME$"});
            this.txtSubjectLine.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtSubjectLine.Location = new System.Drawing.Point(35, 23);
            this.txtSubjectLine.Name = "txtSubjectLine";
            this.txtSubjectLine.Size = new System.Drawing.Size(259, 20);
            this.txtSubjectLine.TabIndex = 9;
            this.txtSubjectLine.Text = "Screen shot: $NAME$";
            //
            // txtMessage
            //
            this.txtMessage.AcceptsReturn = true;
            this.txtMessage.AutoCompleteCustomSource.AddRange(new string[] {
            "$NAME$",
            "$SIZE$",
            "$OPERATINGSYSTEM$",
            "$COMPUTERNAME$"});
            this.txtMessage.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtMessage.Location = new System.Drawing.Point(35, 63);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMessage.Size = new System.Drawing.Size(259, 148);
            this.txtMessage.TabIndex = 11;
            this.txtMessage.Text = "Here's a screenshot for you.\r\nName: $NAME$\r\nSize: $SIZE$\r\n" +
                "Taken with Cropper - http://cropper.codeplex.com\r\n";

            //
            // lblMessage
            //
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(21, 47);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 10;
            this.lblMessage.Text = "Message";
            //
            // Options
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 391);
            this.Name = "Options";
            this.Text = "Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.qualitySlider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioBitmap;
        private System.Windows.Forms.RadioButton radioJpg;
        private System.Windows.Forms.RadioButton radioPng;
        private System.Windows.Forms.TrackBar qualitySlider;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtSubjectLine;
        private System.Windows.Forms.Label lblSubjectLine;

    }
}