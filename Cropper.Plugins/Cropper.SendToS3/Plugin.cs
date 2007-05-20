using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using Fusion8.Cropper.Extensibility;
using Cropper.SendToS3.S3;

namespace Cropper.SendToS3
{
    public class Plugin : IPersistableImageFormat, IConfigurablePlugin
    {
        Options _opts;
        S3Settings _settings;
        IPersistableOutput _output;

        #region IPersistableImageFormat Members
                
        public string Description
        {
            get { return "Send screenshot to Amazon S3 account."; }
        }

        public string Extension
        {
            get { return "Png"; }
        }

        public IPersistableImageFormat Format
        {
            get { return this; }
        }


        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }
            _output = persistableOutput;
            _output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
            _output.ImageCapturing += new ImageCapturingEventHandler(this.persistableOutput_ImageCapturing);
        }

        public void Disconnect()
        {
            _output.ImageCaptured -= new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
            _output.ImageCapturing -= new ImageCapturingEventHandler(this.persistableOutput_ImageCapturing);
        }

        public event ImageFormatClickEventHandler ImageFormatClick;

        private void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            if (ImageFormatClick != null)
            {
                ImageFormatClick(sender, e);
            }
        }

        void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {            
            Service amazon = new Service(_settings.AccessKeyId, _settings.SecretAccessKey);
            MemoryStream imageStream = new MemoryStream();
            e.FullSizeImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
            imageStream.Position = 0;
            S3Object obj = new S3Object(imageStream, null);
            SortedList headers = new SortedList();
            headers.Add("x-amz-acl", "public-read");
            headers.Add("Content-Type", "image/png");
            string imageName = _settings.BaseKey + Guid.NewGuid().ToString() + ".png";
            Response r = amazon.Put(_settings.BucketName, imageName, obj, headers);

            if (r.Status.ToString() != "OK")
            {
                MessageBox.Show(r.Status.ToString() + ": " + r.getResponseMessage());
            }
            else
            {
                string url = string.Format("http://s3.amazonaws.com/{0}/{1}", _settings.BucketName, imageName);
                Clipboard.SetText(url, TextDataFormat.Text);
            }
        }


        void persistableOutput_ImageCapturing(object sender, ImageCapturingEventArgs e)
        {
        }

        public MenuItem Menu
        {
            get
            {
                MenuItem item = new MenuItem();
                item.RadioCheck = true;
                item.Text = "Send to Amazon S3";
                item.Click += new EventHandler(this.MenuItemClick);
                return item;
            }
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            ImageFormatEventArgs args = new ImageFormatEventArgs();
            args.ClickedMenuItem = (MenuItem)sender;
            args.ImageOutputFormat = this;
            this.OnImageFormatClick(sender, args);
        }


        #endregion

        #region IConfigurablePlugin Members

        public BaseConfigurationForm ConfigurationForm
        {
            get 
            {
                if (_opts == null)
                {
                    _settings = S3Settings.Load();
                    _opts = new Options();
                    _opts.OptionsSaved += new EventHandler(opts_OptionsSaved);
                    _opts.AccessKeyId = _settings.AccessKeyId;
                    _opts.SecretAccessKey = _settings.SecretAccessKey;
                    _opts.BucketName = _settings.BucketName;
                    _opts.BaseKey = _settings.BaseKey;
                    _opts.LoadList();
                }

                return _opts;
            }
        }

        void opts_OptionsSaved(object sender, EventArgs e)
        {
            _settings.AccessKeyId = ((Options)sender).AccessKeyId;
            _settings.SecretAccessKey = ((Options)sender).SecretAccessKey;
            _settings.BucketName = ((Options)sender).BucketName;
            _settings.BaseKey = ((Options)sender).BaseKey;

            _settings.Save();
        }

        public bool HostInOptions
        {
            get { return true; }
        }

        public object Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = S3Settings.Load();
                }

                return _settings;
            }
            set
            {
                _settings = value as S3Settings;
            }
        }

        #endregion
    }
}
