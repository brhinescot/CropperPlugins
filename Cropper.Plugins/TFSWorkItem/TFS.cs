using System;
using System.Collections.Generic;
using System.Text;
using Fusion8.Cropper.Extensibility;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Configuration;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Client;
using System.Diagnostics;

using CropperPlugins.Utils;       // for Tracing



namespace Cropper.TFSWorkItem
{
    public class TFS : DesignablePlugin, IConfigurablePlugin
    {
        public static  string PluginDescription = "TFS Work Item";

        private TfsSettings _settings = new TfsSettings();
        private TeamFoundationServer _tfs;
        private WorkItemStore _wis;
        private string _fileName;
        private OptionsForm _configForm;


        public override string ToString()
        {
            return "Send to TFS Work Item";
        }


        public override void Connect(IPersistableOutput persistableOutput)
        {
            Tracing.Trace("TFSWI::Connect");
            base.Connect(persistableOutput);
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            Tracing.Trace("+--------------------------------");
            Tracing.Trace("TFSWI::ImageCaptured ({0})", _fileName);

            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage),
                                     this._fileName, e.FullSizeImage);
        }



        /// <summary>
        ///   Saves the captured image to an on-disk file, then
        ///   attaches it to a TFS workitem.
        /// </summary>
        private void SaveImage(Stream stream, System.Drawing.Image image)
        {
            bool success = false;
            try
            {
                Tracing.Trace("+--------------------------------");
                Tracing.Trace("TFSWI::SaveImage ({0})", _fileName);

                SaveImageInDesiredFormat(stream, image);

                success = true;
            }
            catch (Exception exception1)
            {
                Tracing.Trace("Exception while saving the image: {0}", exception1.Message);
                string msg = "There's been an exception while saving the image: " +
                    exception1.Message + "\n" + exception1.StackTrace;
                MessageBox.Show(msg);
                return;
            }
            finally
            {
                image.Dispose();
                stream.Close();
            }

            if (success)
                AttachImageToWorkItem();
        }




        private ImageFormat DesiredImageFormat
        {
            get
            {
                if (String.Compare(Extension, "jpg", true) == 0)
                {
                    return ImageFormat.Jpeg;
                }
                else if (String.Compare(Extension, "bmp", true) == 0)
                {
                    return ImageFormat.Bmp;
                }
                else
                {
                    return ImageFormat.Png;
                }
            }
        }



        private void SaveImageInDesiredFormat(Stream stream, System.Drawing.Image image)
        {
            Tracing.Trace("TFS::SaveImageInDesiredFormat");
            if (String.Compare(Extension, "jpg", true) == 0)
            {
                SaveImage_Jpg(stream, image);
            }
            else
            {
                image.Save(stream, DesiredImageFormat);
            }
        }


        private void SaveImage_Jpg(Stream stream, System.Drawing.Image image)
        {
            var myEncoder = System.Drawing.Imaging.Encoder.Quality;
            var cInfo = GetEncoderInfo("image/jpeg");
            using (var p1 = new EncoderParameters(1))
            {
                using (var p2 = new EncoderParameter(myEncoder, PluginSettings.JpgImageQuality))
                {
                    p1.Param[0] = p2;
                    image.Save(stream, cInfo, p1);
                }
            }
        }


        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int j = 0; j < encoders.Length; j++)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }



        private void AttachImageToWorkItem()
        {
            if (!VerifyBasicSettings()) return;

            var dlg = new AttachDialog(_settings, _tfs);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileName = Path.Combine(Path.GetTempPath(), dlg.ImageName + "." + Extension);
                    string attachmentComment = dlg.AttachmentComment;
                    bool openImageInEditor = dlg.OpenImageInEditor;
                    string imageEditor = dlg.ImageEditor;
                    WorkItem workItem = dlg.SelectedWorkItem;
                    if (openImageInEditor)
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo(_settings.ImageEditor, "\"" + fileName + "\"");
                        Process process = new Process();
                        process.StartInfo = processStartInfo;
                        process.Start();
                        process.WaitForExit();
                    }
                    workItem.Attachments.Add(new Attachment(fileName, attachmentComment));
                    WorkItemEditorForm frmWorkItemEditor = new WorkItemEditorForm(SelectedWorkItemType, workItem);
                    frmWorkItemEditor.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, this.Description, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Action canceled.", this.Description, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }



        public override string Description
        {
            get
            {
                return TFS.PluginDescription;
            }
        }

        public override string Extension
        {
            get
            {
                if (!String.IsNullOrEmpty(_settings.DefaultImageFormat))
                {
                    return _settings.DefaultImageFormat;
                }
                else
                {
                    return "png";
                }
            }
        }

        public WorkItemType SelectedWorkItemType
        {
            get ;
            set ;
        }

        private void SetWorkItemType()
        {
            _tfs = new TeamFoundationServer(_settings.TeamServer);
            _tfs.EnsureAuthenticated();
            _wis = _tfs.GetService(typeof(WorkItemStore)) as WorkItemStore;
            SelectedWorkItemType = _wis.Projects[_settings.TeamProject].WorkItemTypes[_settings.WorkItemType];
        }


        private bool VerifyBasicSettings()
        {
            Tracing.Trace("TFSWI::VerifyBasicSettings");

            if (!PluginSettings.Completed)
            {
                var dlg = new OptionsForm(_settings);
                dlg.MakeButtonsVisible();
                if (dlg.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show("You must configure TFS settings before " +
                                    "saving an image to a workitem.\n\n" +
                                    "Missing TFS Settings");
                    return false;
                }
                return true;
            }
            try
            {
                SetWorkItemType();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Disconnect();
                return false;
            }
        }


        #region IConfigurablePlugin Implementation

        /// <summary>
        /// Gets the plug-ins impementation of the <see cref="BaseConfigurationForm"/> used
        /// for setting plug-in specific options.
        /// </summary>
        BaseConfigurationForm IConfigurablePlugin.ConfigurationForm
        {
            get
            {
                if (_configForm == null)
                {
                    _configForm = new OptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        /// <summary>
        /// Gets a value indicating if the <see cref="ConfigurationForm"/> should
        /// be hosted in the options dialog or shown in its own dialog window.
        /// </summary>
        bool IConfigurablePlugin.HostInOptions
        {
            get { return true; }
        }

        /// <summary>
        /// The settings for this plugin. Required by IConfigurablePlugin.
        /// </summary>
        ///
        /// <remarks>
        /// <para>
        ///   This property is set during startup with the settings contained in the
        ///   applications configuration file.
        /// </para>
        /// <para>
        ///   The object returned by this property is serialized into the applications
        ///   configuration file on shutdown.
        /// </para>
        /// </remarks>
        public object Settings
        {
            get {
                //System.Diagnostics.Debugger.Break();
                return PluginSettings;
            }
            set {
                //System.Diagnostics.Debugger.Break();
                PluginSettings = value as Cropper.TFSWorkItem.TfsSettings;
            }
        }

        #endregion

        // Helper property for IConfigurablePlugin Implementation
        private Cropper.TFSWorkItem.TfsSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new Cropper.TFSWorkItem.TfsSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        /// <summary>
        ///   Invoked when the options form is hosted in the tabbed UI,
        ///   and the user clicks OK.
        /// </summary>
        private void OptionsSaved(object sender, EventArgs e)
        {
            OptionsForm form = sender as OptionsForm;
            if (form == null) return;
            form.ApplySettings();
        }

    }
}
