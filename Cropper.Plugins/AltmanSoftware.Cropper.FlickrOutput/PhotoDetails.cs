namespace AltmanSoftware.Cropper.FlickrOutput
{
    using FlickrNet;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    public class PhotoDetails : Form
    {
        public PhotoDetails(string path)
        {
            this.components = null;
            this.InitializeComponent();
            if (!File.Exists(path))
            {
                throw new Exception("Invalid ConfigFilePath: " + path);
            }
            this._settings = new Settings(path);
            this.txtTags.Text = this._settings.Tags;
            FlickrNet.Flickr flickr1 = new FlickrNet.Flickr("ab782e182b4eb406d285211811d625ff", "b080496c05335c3d", this._settings.Token);
            try
            {
                Photosets photosets1 = flickr1.PhotosetsGetList();
                foreach (Photoset photoset1 in photosets1.PhotosetCollection)
                {
                    PhotoSetItem item1 = new PhotoSetItem(photoset1.PhotosetId, photoset1.Title);
                    this.cboPhotoSets.Items.Add(item1);
                    if (photoset1.PhotosetId == this._settings.PhotoSet)
                    {
                        this.cboPhotoSets.SelectedItem = item1;
                    }
                }
            }
            catch (FlickrException exception1)
            {
                MessageBox.Show("There was a problem in communicating with Flickr via the Flickr API.  " + exception1.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Title = this.txtTitle.Text;
            this.Description = this.txtDescription.Text;
            this.Tags = this.txtTags.Text;
            if (this.cboPhotoSets.SelectedIndex > 0)
            {
                this.PhotoSetId = ((PhotoSetItem) this.cboPhotoSets.SelectedItem).Id;
            }
            this.IsPublic = this.chkPublic.Checked;
            this.IsFamily = this.chkFamily.Checked;
            this.IsFriend = this.chkFriends.Checked;
            if (this._settings != null)
            {
                this._settings.Tags = this.txtTags.Text;
                this._settings.PhotoSet = this.PhotoSetId;
            }
            base.Close();
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
            this.cboPhotoSets = new ComboBox();
            this.txtTags = new TextBox();
            this.label3 = new Label();
            this.label4 = new Label();
            this.button1 = new Button();
            base.SuspendLayout();
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
            this.cboPhotoSets.Location = new Point(0x10, 0x110);
            this.cboPhotoSets.Name = "cboPhotoSets";
            this.cboPhotoSets.Size = new System.Drawing.Size(240, 0x15);
            this.cboPhotoSets.TabIndex = 13;
            this.cboPhotoSets.Text = "Select a Photo Set...";
            this.txtTags.Location = new Point(0x10, 0xb0);
            this.txtTags.Multiline = true;
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(240, 0x40);
            this.txtTags.TabIndex = 12;
            this.txtTags.Text = "";
            this.label3.Location = new Point(0x10, 0x100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 0x17);
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
            this.button1.Text = "Save";
            this.button1.Click += new EventHandler(this.button1_Click);
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            base.ClientSize = new System.Drawing.Size(0x162, 0x138);
            base.Controls.Add(this.button1);
            base.Controls.Add(this.cboPhotoSets);
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
            this.Text = "Photo Details";
            base.ResumeLayout(false);
        }


        private Settings _settings;
        private Button button1;
        private ComboBox cboPhotoSets;
        private CheckBox chkFamily;
        private CheckBox chkFriends;
        private CheckBox chkPublic;
        private Container components;
        public string Description;
        public bool IsFamily;
        public bool IsFriend;
        public bool IsPublic;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        public string PhotoSetId;
        public string Tags;
        public string Title;
        private TextBox txtDescription;
        private TextBox txtTags;
        private TextBox txtTitle;
    }
}

