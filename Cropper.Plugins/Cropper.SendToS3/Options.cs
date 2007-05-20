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
    public partial class Options : BaseConfigurationForm
    {
        public Options()
        {
            InitializeComponent();
        }

        private void _linkRefreshBucketList_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadList();
        }

        public void LoadList()
        {
            if (!string.IsNullOrEmpty(AccessKeyId) && !string.IsNullOrEmpty(SecretAccessKey))
            {
                _cmbBucket.Items.Clear();

                Service az = new Service(AccessKeyId, SecretAccessKey);
                
                try
                {
                    ListAllMyBucketsResponse buckets = az.ListAllMyBuckets(null);
                    
                    foreach (Bucket bucket in buckets.Buckets)
                    {
                        _cmbBucket.Items.Add(bucket.Name);
                    }

                    if (!string.IsNullOrEmpty(_bucketName))
                    {
                        _cmbBucket.SelectedItem = _bucketName;
                    }
                }
                catch (System.Net.WebException e)
                {
                    MessageBox.Show("Error: " + e.Message);
                }

                
            }
        }

        public string AccessKeyId
        {
            get { return _txtAccessKeyID.Text.Trim(); }
            set { _txtAccessKeyID.Text = value; }
        }

        public string SecretAccessKey
        {
            get { return _txtSecretAccessKey.Text.Trim(); }
            set { _txtSecretAccessKey.Text = value; }
        }

        string _bucketName; 

        public string BucketName
        {
            get { return _cmbBucket.SelectedItem.ToString(); }
            set
            {
                _bucketName = value;
            }
        }

        public string BaseKey
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
    }
}