// S3\Plugin.cs
//

// Cropper workitem 14970
#define HACK

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using Fusion8.Cropper.Extensibility;

using CropperPlugins.Common;       // for Tracing

namespace Cropper.SendToS3
{
    public class Plugin : DesignablePlugin,
        IConfigurablePlugin
#if HACK
#else
        , CropperPlugins.Common.IUpload
#endif

    {
        OptionsForm _optionsForm;
        S3Settings _settings;

        public override string Description
        {
            get { return "Send to Amazon S3"; }
        }

        public override string Extension
        {
            // TODO: make the output format configurable
            get { return "Png"; }
        }

        public override string ToString()
        {
            return "Send to Amazon S3 [paltman.com]";
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            try
            {
                MemoryStream imageStream = new MemoryStream();
                e.FullSizeImage.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                imageStream.Position = 0;
                UploadImageStream(imageStream, GenerateUniqueImageName(".png"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }

        private string GenerateUniqueImageName(string extension)
        {
            return _settings.BaseKey + Guid.NewGuid().ToString() +
                extension;
        }

        private bool VerifyBasicSettings()
        {
            if (!PluginSettings.Completed)
            {
                var dlg = new OptionsForm(PluginSettings);
                dlg.MakeButtonsVisible();
                dlg.ShowDialog();
            }

            if (!PluginSettings.Completed)
            {
                MessageBox.Show("You must configure S3 settings before " +
                                "uploading an image to the service.\n",
                                "Missing S3 Settings");
                return false;
            }
            return true;
        }


        private void UploadImageStream(Stream imageStream, string imageName)
        {
            if (!VerifyBasicSettings()) return;

            Service s3 = new Service(_settings.AccessKeyId, _settings.SecretAccessKey);
            S3Object obj = new S3Object
                {
                    Stream = imageStream
                };
            var headers = new Dictionary<String,String>();
            headers.Add("x-amz-acl", "public-read");
            headers.Add("Content-Type", "image/png");
            var r = s3.Put(_settings.BucketName, imageName, obj, headers);

            if (r.StatusCode != System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("Status: " + r.StatusCode + ": " + r.GetResponseMessage());
            }
            else
            {
                string url = string.Format("http://s3.amazonaws.com/{0}/{1}",
                                           _settings.BucketName, imageName);
                Clipboard.SetText(url, TextDataFormat.Text);
            }
        }



        public void UploadFile(string fileName)
        {
            using (var fs = File.OpenRead(fileName))
            {
                string uniqueName = GenerateUniqueImageName(Path.GetExtension(fileName));
                UploadImageStream(fs, uniqueName);
            }
        }



#if Trace
        public Plugin()
        {
            Tracing.Trace("S3::ctor ({0:X8})", this.GetHashCode());
        }

        // these methods are needed only for diagnostic purposes.
        public override void Connect(IPersistableOutput persistableOutput)
        {
            Tracing.Trace("S3::{0:X8}::Connect", this.GetHashCode());
            base.Connect(persistableOutput);
        }

        public override void Disconnect()
        {
            Tracing.Trace("S3::{0:X8}::Disconnect", this.GetHashCode());
            base.Disconnect();
        }

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("S3::{0:X8}::MenuClick", this.GetHashCode());
            base.OnImageFormatClick(sender, e);
        }
#endif


        #region IConfigurablePlugin Members

        public BaseConfigurationForm ConfigurationForm
        {
            get
            {
                if (_optionsForm == null)
                {
                    _optionsForm = new OptionsForm(PluginSettings);
                    _optionsForm.OptionsSaved += OptionsSaved;
                }

                return _optionsForm;
            }
        }

        public bool HostInOptions
        {
            get { return true; }
        }

        public object Settings
        {
            /// <summary>
            ///   The getter.  This gets invoked by Cropper Core when
            ///   Cropper exits, so that Cropper can store the settings
            ///   into the cropper.config file.
            /// </summary>
            get { return PluginSettings; }

            /// <summary>
            ///   The setter.  This gets called by the Cropper Core when it
            ///   reads in cropper.config file; Cropper will provide *just
            ///   the settings for thie plugin* with this setter.
            /// </summary>
            set { PluginSettings = value as S3Settings; }
        }
        #endregion


        private void OptionsSaved(object sender, EventArgs e)
        {
            OptionsForm form = sender as OptionsForm;
            if (form == null) return;
            form.ApplySettings();
        }

        private S3Settings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new S3Settings();

                return _settings;
            }
            set { _settings = value; }
        }
    }
}
