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
        public AuthorizeDialog(string path)
        {
            this.components = null;
            this.InitializeComponent();
            this._settings = new Settings(path);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new Label();
            this.label2 = new Label();
            this.linkLabel1 = new LinkLabel();
            this.linkLabel2 = new LinkLabel();
            base.SuspendLayout();
            this.label1.Font = new Font("Microsoft Sans Serif", 11.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label1.Location = new Point(0x10, 0x10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0x100, 80);
            this.label1.TabIndex = 0;
            this.label1.Text = "Before you can use this plugin to send your photos to your Flickr account, you must first authorize it for use with Flickr services.  ";
            this.label2.Location = new Point(0x10, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0x100, 0x20);
            this.label2.TabIndex = 1;
            this.label2.Text = "You can revoke this authorization at anytime through your Flickr account settings.";
            this.linkLabel1.Location = new Point(0x10, 0x60);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(0x40, 0x17);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Authorize...";
            this.linkLabel1.TextAlign = ContentAlignment.MiddleLeft;
            this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            this.linkLabel2.ImageAlign = ContentAlignment.MiddleRight;
            this.linkLabel2.Location = new Point(0x88, 0x60);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(0x80, 0x17);
            this.linkLabel2.TabIndex = 3;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Complete Authorization";
            this.linkLabel2.TextAlign = ContentAlignment.MiddleRight;
            this.linkLabel2.Visible = false;
            this.linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            base.ClientSize = new System.Drawing.Size(0x124, 0xa8);
            base.Controls.Add(this.linkLabel2);
            base.Controls.Add(this.linkLabel1);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Name = "AuthorizeDialog";
            this.Text = "Authorize";
            base.ResumeLayout(false);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FlickrNet.Flickr flickr1 = new FlickrNet.Flickr("ab782e182b4eb406d285211811d625ff", "b080496c05335c3d");
            this._frob = flickr1.AuthGetFrob();
            string text1 = flickr1.AuthCalcUrl(this._frob, AuthLevel.Delete);
            Process.Start(text1);
            this.linkLabel1.Visible = false;
            this.linkLabel2.Visible = true;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Auth auth1 = new FlickrNet.Flickr("ab782e182b4eb406d285211811d625ff", "b080496c05335c3d").AuthGetToken(this._frob);
            this._settings.Token = auth1.Token;
            this.linkLabel2.Visible = false;
            MessageBox.Show("Authorization complete!");
        }


        private string _frob;
        private Settings _settings;
        private Container components;
        private Label label1;
        private Label label2;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
    }
}

