using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using FlickrNet;
using CropperPlugins.Utils;       // for Tracing

namespace Cropper.SendToFlickr
{
    public class PhotoDetails : Form
    {
        private FlickrNet.Flickr _flickr;
        private FlickrSettings _settings;

        private Container components;
        private Button button1;
        private Button btnRefresh;
        private ComboBox cboPhotosets;
        private CheckBox chkFamily;
        private CheckBox chkFriends;
        private CheckBox chkPublic;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txtDescription;
        private TextBox txtTags;
        private TextBox txtTitle;
        private System.Windows.Forms.ToolTip tooltip;

        #region public properties
        public string Description
        {
            get { return this.txtDescription.Text; }
        }
        public bool IsFamily
        {
            get { return this.chkFamily.Checked; }
        }
        public bool IsFriend
        {
            get { return this.chkFriends.Checked; }
        }
        public bool IsPublic
        {
            get { return this.chkPublic.Checked; }
        }
        public string Tags
        {
            get { return this.txtTags.Text; }
        }
        public string Title
        {
            get { return this.txtTitle.Text; }
        }
        public string PhotosetId
        {
            get
            {
                return (this.cboPhotosets.SelectedIndex > 0)
                    ? ((PhotoSetItem) this.cboPhotosets.SelectedItem).Id
                    : null;
            }
        }
        #endregion

        public PhotoDetails(FlickrNet.Flickr flickr, FlickrSettings settings)
        {
            Tracing.Trace("PhotoDetails::ctor");
            this.components = null;
            this.InitializeComponent();
            this._settings = settings;

            this.txtTags.Text = settings.MostRecentTags;
            this._flickr = flickr;
            LoadPhotosets(false);
        }


        /// <summary>
        ///   Refresh the list of photosets from the Flickr service.
        /// </summary>
        private void LoadPhotosets(bool circumventCache)
        {
            var c = this.Cursor;
            try
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

                this.cboPhotosets.Items.Clear();
                var item = new PhotoSetItem("-none-", "-none-");
                this.cboPhotosets.Items.Add(item);
                this.cboPhotosets.SelectedItem = item;

                // add in the explicit photosets
                if (circumventCache)
                {
                    _flickr.PhotosetsGetList();
                    FlickrNet.Flickr.FlushCache(_flickr.LastRequest);
                }
                var psc = _flickr.PhotosetsGetList();
                Tracing.Trace("PhotoDetails::ctor psc {0} items", psc.Count);
                foreach (var photoset in psc)
                {
                    Tracing.Trace("PhotoDetails::ctor item {0}", photoset.PhotosetId);
                    item = new PhotoSetItem(photoset.PhotosetId, photoset.Title);
                    this.cboPhotosets.Items.Add(item);
                    if (photoset.PhotosetId == this._settings.MostRecentPhotosetId)
                    {
                        this.cboPhotosets.SelectedItem = item;
                    }
                }
            }
            catch (FlickrException exception1)
            {
                MessageBox.Show("There was a problem in communicating with Flickr via the Flickr API.  " + exception1.Message);
            }

             this.Cursor = c;
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
            this.txtTitle = new TextBox();
            this.txtDescription = new TextBox();
            this.chkPublic = new CheckBox();
            this.chkFamily = new CheckBox();
            this.chkFriends = new CheckBox();
            this.cboPhotosets = new ComboBox();
            this.txtTags = new TextBox();
            this.label3 = new Label();
            this.label4 = new Label();
            this.button1 = new Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.tooltip = new System.Windows.Forms.ToolTip();
            this.SuspendLayout();
            this.tooltip.AutoPopDelay = 2400;
            this.tooltip.InitialDelay = 500;
            this.tooltip.ReshowDelay = 500;
            this.label1.Location = new Point(0x10, 8);
            this.label1.Name = "label1";
            this.label1.TabIndex = 0;
            this.label1.Text = "Title";
            this.label2.Location = new Point(0x10, 0x40);
            this.label2.Name = "label2";
            this.label2.TabIndex = 1;
            this.label2.Text = "Description";
            this.txtTitle.Location = new Point(0x10, 0x18);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(320, 20);
            this.txtTitle.TabIndex = 5;
            this.txtTitle.Text = "";
            this.txtDescription.Location = new Point(0x10, 80);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(320, 0x40);
            this.txtDescription.TabIndex = 6;
            this.txtDescription.Text = "";
            this.chkPublic.Checked = true;
            this.chkPublic.CheckState = CheckState.Checked;
            this.chkPublic.Location = new Point(0x110, 0xa8);
            this.chkPublic.Name = "chkPublic";
            this.chkPublic.Size = new System.Drawing.Size(0x40, 0x18);
            this.chkPublic.TabIndex = 7;
            this.chkPublic.Text = "Public";
            this.chkFamily.Location = new Point(0x110, 0xc0);
            this.chkFamily.Name = "chkFamily";
            this.chkFamily.Size = new System.Drawing.Size(0x40, 0x18);
            this.chkFamily.TabIndex = 8;
            this.chkFamily.Text = "Family";
            this.chkFriends.Location = new Point(0x110, 0xd8);
            this.chkFriends.Name = "chkFriends";
            this.chkFriends.Size = new System.Drawing.Size(0x40, 0x18);
            this.chkFriends.TabIndex = 9;
            this.chkFriends.Text = "Friends";
            this.cboPhotosets.Location = new Point(16, 272);
            this.cboPhotosets.Name = "cboPhotoSets";
            this.cboPhotosets.Size = new System.Drawing.Size(240, 0x15);
            this.cboPhotosets.TabIndex = 13;
            this.cboPhotosets.Text = "Select a Photo Set...";
            this.cboPhotosets.FormattingEnabled = false;
            this.cboPhotosets.AllowDrop = false;
            this.cboPhotosets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            //
            // btnRefresh
            //
            this.btnRefresh.Image = global::Cropper.SendToFlickr.Properties.Resources.refresh;
            this.btnRefresh.Location = new System.Drawing.Point(88, 252);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(20, 20);
            this.btnRefresh.TabIndex = 24;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            this.tooltip.SetToolTip(btnRefresh, "Refresh");

            this.txtTags.Location = new Point(0x10, 0xb0);
            this.txtTags.Multiline = true;
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(240, 0x40);
            this.txtTags.TabIndex = 12;
            this.txtTags.Text = "";
            this.label3.Location = new Point(16, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 23);
            this.label3.TabIndex = 11;
            this.label3.Text = "Photo Set";
            this.label4.Location = new Point(0x10, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 0x17);
            this.label4.TabIndex = 10;
            this.label4.Text = "Tags";
            this.button1.DialogResult = DialogResult.OK;
            this.button1.Location = new Point(0x110, 0x110);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(0x40, 0x17);
            this.button1.TabIndex = 14;
            this.button1.Text = "Upload";
            this.tooltip.SetToolTip(button1, "upload the image to Flickr");
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            base.ClientSize = new System.Drawing.Size(0x162, 0x138);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.btnRefresh);
            base.Controls.Add(this.cboPhotosets);
            base.Controls.Add(this.txtTags);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.chkFriends);
            base.Controls.Add(this.chkFamily);
            base.Controls.Add(this.chkPublic);
            base.Controls.Add(this.txtDescription);
            base.Controls.Add(this.txtTitle);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Name = "PhotoDetails";
            base.ShowInTaskbar = false;
            this.Text = "Specify details for this image...";
            base.ResumeLayout(false);
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPhotosets(true);
        }

    }
}

