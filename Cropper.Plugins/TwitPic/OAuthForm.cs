// OAuthForm.cs
// ------------------------------------------------------------------
//
// Presents a form to walk the user through the OAuth Approval process:
// get a Request Token, visit the website, click the approve button, get
// a PIN, enter the PIN, get an Access Token.
//
// Author     : Dino
// Created    : Tue Dec 14 15:21:59 2010
// Last Saved : <2010-December-14 17:02:30>
//
// ------------------------------------------------------------------
//
// Copyright (c) 2010 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

namespace Cropper.SendToTwitPic
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;
    using CropperPlugins.Utils;

    class TwitPicOauthForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button btnAction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtPin;
        private System.Windows.Forms.TextBox txtAccessToken;
        private System.Windows.Forms.TextBox txtAccessSecret;
        private System.Windows.Forms.Label lblPin;
        private System.Windows.Forms.Label lblAccessToken;
        private System.Windows.Forms.Label lblAccessSecret;
        private System.Windows.Forms.Label lblInstructions;

        private string _instructions1 =
              "Click the Next button to open a web page on Twitter.com. It will allow you to approve\n" +
              "this Cropper plugin as a Twitter app. This is the only time you will have\n" +
              "to do this.";

        private string _instructions2 =
              "Now a webpage will open. Approve the plugin, then copy/paste the\n" +
              "PIN you receive, into the textbox below. Click the Next " +
              "button to continue.";

        private string _instructions3 =
              "You have granted authorization for this plugin to upload images to TwitPic.\n" +
              "You won't have to do this again. Click the Done button to continue uploading.";

        private string _instructions4 =
              "Something has gone wrong while trying to approve this Plugin.\n" +
              "You'll need to try again.";

        private int _stage;
        private OAuth.Manager _oauth;

        public TwitPicOauthForm(OAuth.Manager oauth)
        {
            InitializeComponent();
            _oauth= oauth;
            _stage = 0;
        }

        public void StoreTokens(TwitPicSettings s)
        {
            s.AccessToken = this._oauth["token"];
            s.AccessSecret = this._oauth["token_secret"];
        }

        private void InitializeComponent()
        {
            // guide the user through OAuth authentication here
            var f = this;

            btnAction = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            txtPin = new System.Windows.Forms.TextBox();
            txtAccessToken = new System.Windows.Forms.TextBox();
            txtAccessSecret = new System.Windows.Forms.TextBox();
            lblInstructions = new System.Windows.Forms.Label();
            lblPin = new System.Windows.Forms.Label();
            lblAccessToken = new System.Windows.Forms.Label();
            lblAccessSecret = new System.Windows.Forms.Label();
            //
            // lblInstructions
            //
            lblInstructions.Text = _instructions1;
            lblInstructions.ForeColor = System.Drawing.Color.Red;
            lblInstructions.AutoSize = true;
            lblInstructions.Size = new System.Drawing.Size(576, 46);
            lblInstructions.Location = new System.Drawing.Point(4, 6);
            //
            // lblPin
            //
            lblPin.Text = "PIN:";
            lblPin.AutoSize = true;
            lblPin.Visible = false;
            lblPin.Location = new System.Drawing.Point(4, 44);
            //
            // txtPin
            //
            txtPin.Text = "";
            txtPin.Enabled = false;
            txtPin.Visible = false;
            txtPin.Location = new System.Drawing.Point(100, 44);
            txtPin.Size = new System.Drawing.Size(320, 82);
            //
            // lblAccessToken
            //
            lblAccessToken.Text = "Access Token:";
            lblAccessToken.Visible = false;
            lblAccessToken.AutoSize = true;
            lblAccessToken.Location = new System.Drawing.Point(4, 38);
            //
            // txtAccessToken
            //
            txtAccessToken.Text = "";
            txtAccessToken.Enabled = false;
            txtAccessToken.Visible = false;
            txtAccessToken.Location = new System.Drawing.Point(100, 38);
            txtAccessToken.Size = new System.Drawing.Size(320, 82);
            //
            // lblAccessSecret
            //
            lblAccessSecret.Text = "Token Secret:";
            lblAccessSecret.Visible = false;
            lblAccessSecret.AutoSize = true;
            lblAccessSecret.Location = new System.Drawing.Point(4, 64);
            //
            // txtAccessSecret
            //
            txtAccessSecret.Text = "";
            txtAccessSecret.Enabled = false;
            txtAccessSecret.Visible = false;
            txtAccessSecret.Location = new System.Drawing.Point(100, 64);
            txtAccessSecret.Size = new System.Drawing.Size(320, 82);
            //
            // btnCancel
            //
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(368, 94);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(68, 23);
            btnCancel.TabIndex = 71;
            btnCancel.Text = "&Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            //
            // btnAction
            //
            btnAction.Location = new System.Drawing.Point(294, 94);
            btnAction.Name = "btnAction";
            btnAction.Size = new System.Drawing.Size(68, 23);
            btnAction.TabIndex = 61;
            btnAction.Text = "&Next";
            btnAction.UseVisualStyleBackColor = true;
            //this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
            //
            // Form
            //
            f.Controls.Add(lblPin);
            f.Controls.Add(lblAccessToken);
            f.Controls.Add(lblAccessSecret);
            f.Controls.Add(lblInstructions);
            f.Controls.Add(txtPin);
            f.Controls.Add(txtAccessToken);
            f.Controls.Add(txtAccessSecret);
            f.Controls.Add(btnAction);
            f.Controls.Add(btnCancel);
            f.Name = "Authorize";
            f.Text = "Authorize the Cropper Plugin";
            f.MinimumSize = new System.Drawing.Size(460, 158);
            f.MaximumSize = new System.Drawing.Size(460, 158);
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            switch (_stage)
            {
                case 0:
                    GetRequestToken();
                    break;
                case 1:
                    GetAccessToken();
                    break;
                case 2:
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                    break;

                default:
                    // not sure how this would ever happen
                    DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    this.Close();
                    break;
            }
            _stage++;
        }


        private void GetRequestToken()
        {
            Cursor cursor = this.Cursor;
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.btnAction.Enabled = false;
            this.btnCancel.Enabled = false;
            this.lblInstructions.Text = _instructions2;
            this.lblPin.Visible = true;
            this.txtPin.Visible = true;
            this.txtPin.Enabled = true;
            this.Update();
            System.Threading.Thread.Sleep(2100);

            var response =
                this._oauth.AcquireRequestToken(TwitPicSettings.URL_REQUEST_TOKEN, "POST");

            this.btnCancel.Enabled = true;
            Tracing.Trace("Request token response: {0}", response.AllText);
            if (!String.IsNullOrEmpty(response["oauth_token"]))
            {
                var uriString = TwitPicSettings.URL_AUTHORIZE + response["oauth_token"];
                System.Diagnostics.Process.Start(uriString);
                this.btnAction.Enabled = true;
            }
            else
            {
                this.lblInstructions.Text = _instructions4;
                _stage= 99;
            }

            this.Cursor = cursor;
        }

        private void GetAccessToken()
        {
            Cursor cursor = this.Cursor;
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.btnAction.Enabled = false;
            this.btnCancel.Enabled = false;
            var pin = this.txtPin.Text.Trim();
            var response =
                this._oauth.AcquireAccessToken(TwitPicSettings.URL_ACCESS_TOKEN, "POST", pin);

            Tracing.Trace("Access token response: {0}", response.AllText);
            if (!String.IsNullOrEmpty(response["oauth_token"]))
            {
                this.lblInstructions.Text = _instructions3;
                SetAccessFields(response["oauth_token"], response["oauth_token_secret"]);
                this.btnAction.Enabled = true;
                btnAction.Text = "&Done";
                btnCancel.Enabled = false;
            }
            else
            {
                this.lblInstructions.Text = _instructions4;
                _stage= 99;
            }

            this.Cursor = cursor;
        }

        private void SetAccessFields(string accessToken, string tokenSecret)
        {
            lblPin.Visible = false;
            txtPin.Visible = false;
            lblAccessToken.Visible = true;
            txtAccessToken.Visible = true;
            lblAccessSecret.Visible = true;
            txtAccessSecret.Visible = true;

            txtAccessToken.Text = accessToken;
            txtAccessSecret.Text = tokenSecret;
        }
    }
}