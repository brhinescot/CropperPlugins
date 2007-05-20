using System;
using System.Collections.Generic;
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

        void SaveImage(Stream stream, Image image)
        {
            Service amazon = new Service(_settings.AccessKeyId, _settings.SecretAccessKey);
            //image.Save(stream, ImageFormat.Png);
        }

        void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            //_output.FetchOutputStream(new StreamHandler(this.SendImage), e.ImageNames.FullSize, e.FullSizeImage);
            //FileInfo file = new FileInfo(e.ImageNames.FullSize);
            //string subject = replaceTokens(PluginSettings.Subject, file);
            //string body = replaceTokens(PluginSettings.Message, file);
            //MapiMailMessage message = new MapiMailMessage(subject, body);
            //message.Files.Add(e.ImageNames.FullSize);
            //message.ShowDialog();
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
            get { return new Options(); }
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
                    _settings = new S3Settings();
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
