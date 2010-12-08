using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using FlickrNet;

namespace Cropper.SendToFlickr
{
    public class AuthorizeDialog : Form
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


        public AuthorizeDialog()
        {
            InitializeComponent();
        }


        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            //this.label4 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            //this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            //this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            base.SuspendLayout();
            //
            // label1
            //
            this.label1.Font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 124);
            this.label1.TabIndex = 0;
            this.label1.Text = "Before you can use this plugin to send your photos to your Flickr account, you must first visit the Flickr.com website to authorize the application for use with Flickr services.";
            //
            // label2
            //
            this.label2.Location = new Point(16, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(300, 48);
            this.label2.TabIndex = 1;
            this.label2.Text = "You need to do this only once. You can revoke this authorization at any time through your Flickr account settings.";
            //
            // label3
            //
            this.label3.Location = new Point(16, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(300, 48);
            this.label3.TabIndex = 1;
            this.label3.Text = "Click the button to open a browser window, which will allow you to approve the authorization. Return to this window when you finish.";
            //
            // btnOK
            //
            this.btnOK.Location = new Point(164, 234);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(76, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.TabStop = true;
            this.btnOK.Text = "Authorize...";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            //
            // button2
            //
            this.button2.Location = new System.Drawing.Point(244, 234);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 23);
            this.button2.TabIndex = 81;
            this.button2.Text = "&Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // //
            // // linkLabel1
            // //
            // this.linkLabel1.Location = new Point(16, 224);
            // this.linkLabel1.Name = "linkLabel1";
            // this.linkLabel1.Size = new System.Drawing.Size(66, 23);
            // this.linkLabel1.TabIndex = 2;
            // this.linkLabel1.TabStop = true;
            // this.linkLabel1.Text = "Authorize...";
            // //this.linkLabel1.TextAlign = ContentAlignment.MiddleLeft;
            // this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // //
            // // linkLabel2
            // //
            // //this.linkLabel2.ImageAlign = ContentAlignment.MiddleRight;
            // this.linkLabel2.Location = new Point(16, 224);
            // this.linkLabel2.Name = "linkLabel2";
            // this.linkLabel2.Size = new System.Drawing.Size(66, 23);
            // this.linkLabel2.TabIndex = 3;
            // this.linkLabel2.TabStop = true;
            // this.linkLabel2.Text = "Complete Authorization";
            // //this.linkLabel2.TextAlign = ContentAlignment.MiddleRight;
            // this.linkLabel2.Visible = false;
            // this.linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            //
            // options
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 340);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Name = "AuthorizeDialog";
            this.Text = "Authorize the Flickr Plugin for Cropper";
            this.ResumeLayout(false);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Text == "Close")
            {
                try
                {
                    var auth1 = flickr1.AuthGetToken(this._frob);
                    this._appAuthorizationToken = auth1.Token;
                    this.DialogResult = DialogResult.OK;
                }
                catch (Exception e1)
                {
                    this._Message = "Exception: " + e1.Message ;
                    this.DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            var c = this.Cursor;
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.btnOK.Enabled = false;
            this.label3.Visible = false;
            this.Update();

            this._frob = flickr1.AuthGetFrob();
            string text1 = flickr1.AuthCalcUrl(this._frob, AuthLevel.Delete);
            if (text1.StartsWith("http://"))
                Process.Start(text1);
            else
                MessageBox.Show("Unexpected auth url: " + text1);

            this.label3.Visible = true;
            this.label3.Text = "Click the close button to proceed.";
            this.button2.Text = "Close";

            this.Cursor = c;
        }


        private FlickrNet.Flickr _flickr1;
        private FlickrNet.Flickr flickr1
        {
            get {
                if (_flickr1 == null)
                    _flickr1 = new FlickrNet.Flickr(FlickrSettings.FLICKR_KEY,
                                                    FlickrSettings.FLICKR_SHAREDSECRET);
                return _flickr1;
            }
        }

        internal string AppAuthorizationToken
        {
            get
            {
                return _appAuthorizationToken;
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
        }

        private string _frob;
        private string _appAuthorizationToken;
        //private Container components;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btnOK;
        private Button button2;
        //private LinkLabel linkLabel1;
        //private LinkLabel linkLabel2;
    }
}

