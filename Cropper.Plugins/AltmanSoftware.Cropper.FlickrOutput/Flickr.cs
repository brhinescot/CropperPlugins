namespace AltmanSoftware.Cropper.FlickrOutput
{
    using FlickrNet;
    using Fusion8.Cropper.Extensibility;
    using System;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class Flickr : IPersistableImageFormat
    {
        public event ImageFormatClickEventHandler ImageFormatClick;

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }
            this._output = persistableOutput;
            this._output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
        }

        public void Disconnect()
        {
            this._output.ImageCaptured -= new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            if (this.ConfigExists)
            {
                ImageFormatEventArgs args1 = new ImageFormatEventArgs();
                args1.ClickedMenuItem = (MenuItem) sender;
                args1.ImageOutputFormat = this;
                this.ImageFormatClick(sender, args1);
            }
            else
            {
                new AuthorizeDialog(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\AltmanSoftware.Cropper.FlickrOutput Plugin\AltmanSoftware.Cropper.FlickrOutput.config").Show();
            }
        }

        private void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            FileStream stream1 = new FileStream(e.ImageNames.FullSize, FileMode.CreateNew);
            e.FullSizeImage.Save(stream1, ImageFormat.Png);
            stream1.Close();
            if (e.IsThumbnailed)
            {
                stream1 = new FileStream(e.ImageNames.Thumbnail, FileMode.CreateNew);
                e.ThumbnailImage.Save(stream1, ImageFormat.Png);
                stream1.Close();
            }
            if (this.ConfigExists)
            {
                Settings settings1 = new Settings(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\AltmanSoftware.Cropper.FlickrOutput Plugin\AltmanSoftware.Cropper.FlickrOutput.config");
                FlickrNet.Flickr flickr1 = new FlickrNet.Flickr("ab782e182b4eb406d285211811d625ff", "b080496c05335c3d", settings1.Token);
                PhotoDetails details1 = new PhotoDetails(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\AltmanSoftware.Cropper.FlickrOutput Plugin\AltmanSoftware.Cropper.FlickrOutput.config");
                if (details1.ShowDialog() == DialogResult.OK)
                {
                    string text1 = flickr1.UploadPicture(e.ImageNames.FullSize, details1.Title, details1.Description, details1.Tags, details1.IsPublic, details1.IsFamily, details1.IsFriend);
                    if ((details1.PhotoSetId != null) && (details1.PhotoSetId != string.Empty))
                    {
                        MessageBox.Show(details1.PhotoSetId);
                        flickr1.PhotosetsAddPhoto(details1.PhotoSetId, text1);
                    }
                    if (e.IsThumbnailed)
                    {
                        string text2 = flickr1.UploadPicture(e.ImageNames.Thumbnail, details1.Title + " Thumbnail", details1.Description, details1.Tags, details1.IsPublic, details1.IsFamily, details1.IsFriend);
                        if ((details1.PhotoSetId != null) && (details1.PhotoSetId != string.Empty))
                        {
                            flickr1.PhotosetsAddPhoto(details1.PhotoSetId, text2);
                        }
                    }
                }
            }
        }


        private bool ConfigExists
        {
            get
            {
                string text1 = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (File.Exists(text1 + @"\AltmanSoftware.Cropper.FlickrOutput Plugin\AltmanSoftware.Cropper.FlickrOutput.config"))
                {
                    return true;
                }
                return false;
            }
        }

        public string Description
        {
            get
            {
                return "Send to Flickr";
            }
        }

        public string Extension
        {
            get
            {
                return "png";
            }
        }

        public IPersistableImageFormat Format
        {
            get
            {
                return this;
            }
        }

        public MenuItem Menu
        {
            get
            {
                MenuItem item1 = new MenuItem();
                item1.RadioCheck = true;
                item1.Text = "Send to Flickr";
                item1.Enabled = true;
                item1.Click += new EventHandler(this.menuItem_Click);
                return item1;
            }
        }


        private IPersistableOutput _output;
        private const string DESCRIPTION = "Send to Flickr";
        private const string EXTENSION = "png";
        private const string USERSETTINGSCONFIGPATH = @"AltmanSoftware.Cropper.FlickrOutput Plugin\AltmanSoftware.Cropper.FlickrOutput.config";
    }
}

