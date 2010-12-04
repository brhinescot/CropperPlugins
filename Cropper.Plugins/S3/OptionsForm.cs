using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Fusion8.Cropper.Extensibility;
using Cropper.SendToS3.S3;

namespace Cropper.SendToS3
{
    public partial class OptionsForm : BaseConfigurationForm
    {
        private string _bucketName;
        private S3Settings _settings;
        private bool _firstActivation = true;

        public OptionsForm(S3Settings settings)
        {
            InitializeComponent();

            _settings = settings;
            this.AccessKeyId     = _settings.AccessKeyId;
            this.SecretAccessKey = _settings.SecretAccessKey;
            this.BucketName      = _settings.BucketName;
            this.BaseKey         = _settings.BaseKey;
        }

        private void Form1_Activated(object sender, System.EventArgs e)
        {
            if (_firstActivation)
            {
                this.LoadList();
                _firstActivation = false;
            }
        }

        /// <summary>
        ///   Load the list of known S3 buckets into the combobox.
        ///   This is possible only if the authentication for S3 is
        ///   successful.
        /// </summary>
        public void LoadList()
        {
            if (string.IsNullOrEmpty(AccessKeyId) ||
                string.IsNullOrEmpty(SecretAccessKey))
                return;

            _cmbBucket.Items.Clear();

            Service az = new Service(AccessKeyId, SecretAccessKey);
            try
            {
                ListAllMyBucketsResponse buckets = az.ListAllMyBuckets(null);
                foreach (Bucket bucket in buckets.Buckets)
                    _cmbBucket.Items.Add(bucket.Name);

                if (!string.IsNullOrEmpty(_bucketName))
                    _cmbBucket.SelectedItem = _bucketName;
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
        }


        /// <summary>
        ///   Show the OK and Cancel buttons.
        /// </summary>
        ///
        /// <remarks>
        ///   This form can be shown in two ways: as a standalone
        ///   dialog, and hosted within the tabbed "Options" UI provided
        ///   by the Cropper Core.  By default, the OK and Cancel
        ///   buttons are not visible.  When used as a standalone dialog
        ///   the caller should invoke this method before calling
        ///   ShowDialog().
        /// </remarks>
        public void MakeButtonsVisible()
        {
            this.btnOK.Visible = true;
            this.btnCancel.Visible = true;
            this.btnOK.Enabled = true;
            this.btnCancel.Enabled = true;
            this.MinimumSize = new System.Drawing.Size(340, 326);
            this.MaximumSize = new System.Drawing.Size(340, 326);
        }

        public void ApplySettings()
        {
            _settings.AccessKeyId = this.AccessKeyId;
            _settings.SecretAccessKey =  this.SecretAccessKey;
            _settings.BucketName = this.BucketName;
            _settings.BaseKey = this.BaseKey;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private string AccessKeyId
        {
            get { return _txtAccessKeyID.Text.Trim(); }
            set { _txtAccessKeyID.Text = value; }
        }

        private string SecretAccessKey
        {
            get { return txtSecretAccessKey.Text.Trim(); }
            set { txtSecretAccessKey.Text = value; }
        }


        private string BucketName
        {
            get
            {
                if (_cmbBucket.SelectedItem != null)
                {
                    return _cmbBucket.SelectedItem.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                // TODO: actually select the item with the string value
                // matching the value passed in.  If there is no such
                // item, then add an item, and select it.
                _bucketName = value;
            }
        }


        private string BaseKey
        {
            get
            {
                string key = _txtBaseKey.Text.Trim();

                if (!string.IsNullOrEmpty(key))
                {
                    key.Replace('\\', '/');
                    if (key[key.Length - 1] != '/')
                    {
                        key += "/";
                    }
                }

                return key;
            }
            set
            {
                _txtBaseKey.Text = value;
            }
        }

        private void btnRefreshBucketList_Click(object sender, EventArgs e)
        {
            LoadList();
        }
    }
}
