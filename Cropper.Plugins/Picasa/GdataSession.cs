// GdataSession.cs
// ------------------------------------------------------------------
//
// A class to store session information for GData service. Primarily
// this is the authentication header.
//
// Author     : Dino
// Created    : Sat Dec 11 11:02:23 2010
// Last Saved : <2010-December-23 18:17:41>
//
// ------------------------------------------------------------------
//
// Copyright (c) 2010 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Http;
using RE=System.Text.RegularExpressions;
using CropperPlugins.Utils;


/// <summary>
///   A Singleton that holds an authentication token (in the form of a
///   request header) for a Google Gdata service.
/// </summary>
///
/// <remarks>
///   <para>
///     It's necessary to obtain this auth token just once per process,
///     using GData's ClientLogin protocol.  Because getting the token
///     requires a prompt to the user for username and password, we want
///     to make the result a process-wide singleton and re-use it as
///     necessary.
///   </para>
///   <para>
///     There are two cases where we need it: first, to upload a photo,
///     the user needs to be authenticated.  Second, to populate the
///     list of albums to upload to, the user needs to be
///     authenticated. This latter bit can happen when displaying the
///     options form for this plugin. That there are two situations
///     where authentication will happen is why we want to make this a
///     singleton.
///   </para>
/// </remarks>
namespace Cropper.SendToPicasa
{
    public sealed class GdataSession
    {
        static readonly GdataSession instance= new GdataSession();

        /// <summary>
        /// Explicit static constructor to tell C# compiler
        /// not to mark type as beforefieldinit
        /// </summary>
        static GdataSession() { }

        /// <summary>
        ///   non-public default ctor
        /// </summary>
        GdataSession()
        {
            Auth = new Dictionary<String,Dictionary<String,String>>();
        }


        private Dictionary<String,Dictionary<String,String>> Auth;


        private void SetAuth(string user, string service, string auth)
        {
            var serviceString = LookupService(service);

            if (!Auth.ContainsKey(user))
                Auth[user] = new Dictionary<String,String>();

            var userEntry = Auth[user];

            userEntry[serviceString] = auth;
            userEntry[service] = auth;
        }


        /// <summary>
        ///   Generate a new instance of headers every request.
        ///   I found that the Microsoft.Http assembly modifies the
        ///   headers object when it is used, causing problems on
        ///   the 2nd call. Content-Length can be set, for example.
        /// </summary>
        public Microsoft.Http.Headers.RequestHeaders GetHeaders(string user, string service)
        {
            try
            {
                var userEntry = Auth[user];

                var headers = Microsoft.Http.Headers.RequestHeaders.Parse
                    ("GData-Version: 2.0\r\n" +
                     "Authorization: GoogleLogin auth=" +
                     userEntry[service] +
                     "\r\n");

                return headers;
            }
            catch
            {
                // occurs if the key is not found, or is null
                return null;
            }
        }

        /// <summary>
        ///   The instance - for the singleton.
        /// </summary>
        public static GdataSession Instance
        {
            get { return instance; }
        }

        /// <summary>
        ///   map from the common service name to the google-required service string.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     See http://code.google.com/apis/accounts/docs/AuthForInstalledApps.html.
        ///     For example, the name associated with Google Calendar is 'cl'. This
        ///     parameter is required when authenticating access to services based on
        ///     Google Data APIs. For specific service names, refer to the service
        ///     documentation.
        ///   </para>
        /// </remarks>
        private static string LookupService (string service)
        {
            if (service.ToLower() == "picasa")
                return "lh2";
            return service;
        }


        class GoogleAuthInfo
        {
            public string email;
            public string password;
        }


        /// <summary>
        ///   Prompt for google user id (email) and password.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     It pops up a simple form, with a slot for username and
        ///     another for password. Two buttons - OK and Cancel.
        ///   </para>
        ///   <para>p
        ///     The username may be blank or null on entry.
        ///   </para>
        /// </remarks>
        private static GoogleAuthInfo PromptForAuthInfo(string username)
        {
            var f = new System.Windows.Forms.Form();
            var btnOK = new System.Windows.Forms.Button();
            var btnCancel = new System.Windows.Forms.Button();
            var label1 = new System.Windows.Forms.Label();
            var label2 = new System.Windows.Forms.Label();
            var txtUser = new System.Windows.Forms.TextBox();
            var txtPass = new System.Windows.Forms.TextBox();
            var tooltip = new System.Windows.Forms.ToolTip();
            f.SuspendLayout();
            tooltip.AutoPopDelay = 2400;
            tooltip.InitialDelay = 500;
            tooltip.ReshowDelay = 500;

            label1.Text = "email:";
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(4, 8);
            txtUser.Text = username;
            txtUser.TabIndex = 11;
            txtUser.Location = new System.Drawing.Point(64, 6);
            txtUser.Size = new System.Drawing.Size(200, 24);
            tooltip.SetToolTip(txtUser, "Your Google username");

            label2.Text = "password:";
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(4, 36);
            txtPass.Text = "";
            txtPass.TabIndex = 21;
            txtPass.PasswordChar = '*';
            txtPass.Location = new System.Drawing.Point(64, 34);
            txtPass.Size = new System.Drawing.Size(200, 24);
            tooltip.SetToolTip(txtUser, "Your Google password");

            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(238, 64);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(64, 23);
            btnCancel.TabIndex = 71;
            btnCancel.Text = "&Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            btnOK.Location = new System.Drawing.Point(166, 64);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(64, 23);
            btnOK.TabIndex = 61;
            btnOK.Text = "&OK";
            btnOK.UseVisualStyleBackColor = true;
            f.Controls.Add(label1);
            f.Controls.Add(txtUser);
            f.Controls.Add(label2);
            f.Controls.Add(txtPass);
            f.Controls.Add(btnOK);
            f.Controls.Add(btnCancel);
            f.Name = "GoogleAuthenticate";
            f.Text = "Authenticate to Picasa";
            f.Icon = global::Cropper.SendToPicasa.Properties.Resources.icon;
            f.MinimumSize = new System.Drawing.Size(320, 128);
            f.MaximumSize = new System.Drawing.Size(320, 128);
            if (String.IsNullOrEmpty(txtUser.Text))
                f.ActiveControl = txtUser;
            else
                f.ActiveControl = txtPass;
            f.ResumeLayout(false);

            var result = f.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // save (cache) the user name for next time.
                return new GoogleAuthInfo
                {
                    email = txtUser.Text,
                    password = txtPass.Text
                };
            }

            return null;
        }



        /// <summary>
        ///   Authenticate to the named Google/GData service, eg picasa,
        ///   with the given user name.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     If the provided username is blank, then this method will
        ///     prompt the user to provide it, by displaying a form.  It
        ///     then will send an authentication message to Google.
        ///     Upon receipt of a successful authentication message, the
        ///     Google authentication token will be stored into this
        ///     (singleton) instance.
        ///   </para>
        /// </remarks>
        /// <returns>
        //    The user name (email addy) which is authenticated.
        /// </returns>
        public static string Authenticate(string username, string service)
        {
            Tracing.Trace("GDataSession::Authenticate user({0}), svc({1})",
                          username, service);
            var serviceString = LookupService(service);
            if (instance.GetHeaders(username, service) != null) return username;
            var info = PromptForAuthInfo(username);
            if (info == null) return null;

            var http = new HttpClient(_baseLoginUrl);
            var form = new HttpUrlEncodedForm();
            form.Add("accountType", "GOOGLE");
            form.Add("Email", info.email);
            form.Add("Passwd", info.password);
            form.Add("service", serviceString);
            form.Add("source", _appName);

            var response = http.Post(_relativeLoginUrl, form.CreateHttpContent());
            response.EnsureStatusIsSuccessful();

            var result = response.Content.ReadAsString();
            string auth = RE.Regex.Replace(result, "(?s).*Auth=(.*)", "$1");

            // After a successful authentication request, use the Auth
            // value to create an Authorization header for each request:
            //
            // Authorization: GoogleLogin auth=yourAuthValue
            //
            // Hereafter, we need to use a HttpClient.Send() overload, in
            // order to be able to specify headers.

            instance.SetAuth(info.email, service, auth);
            Tracing.Trace("GDataSession::Authenticate return '{0}'",
                          info.email);

            return info.email;
        }

        private static readonly string _baseLoginUrl = "https://www.google.com";
        private static readonly string _relativeLoginUrl = "/accounts/ClientLogin";
        private static readonly string _appName = "DinoChiesa-CropperPicasaPlugin-1.0";
    }
}

