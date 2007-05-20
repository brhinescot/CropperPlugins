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
            _cmbBucket.Items.Clear();

            Service az = new Service(_txtAccessKeyID.Text.Trim(), _txtSecretAccessKey.Text.Trim());

            //AWSAuthConnection conn = new AWSAuthConnection(_txtAccessKeyID.Text.Trim(), _txtSecretAccessKey.Text.Trim());

            ListAllMyBucketsResponse buckets = az.ListAllMyBuckets(null); 

            foreach (string bucket in buckets.Buckets)
            {
                _cmbBucket.Items.Add(bucket);
            }
        }
    }
}