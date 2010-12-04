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
    public class Plugin : DesignablePlugin, IConfigurablePlugin
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
            if (!VerifyBasicSettings()) return;

            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
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
                                "saving an image to the service.\n\n" +
                                "For information on these settings, see " +
                                "http://aws.amazon.com/s3\n",
                                "Missing S3 Settings");
                return false;
            }
            return true;
        }





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
