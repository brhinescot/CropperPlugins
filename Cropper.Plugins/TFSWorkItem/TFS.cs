// TFSWorkItem/TFS.cs
//
// Code for a cropper plugin that sends a screen snap to
// a workitem in TFS.
//
// To enable tracing for this DLL, build like so:
//    msbuild /p:Platform=x86 /p:DefineConstants=Trace
//
//
// The TFS plugin implements IConfigurablePlugin, which is the interface
// Cropper (core) uses to give settings to a plugin (for instance on
// cropper startup), or ask a plugin to pop a form to the user to
// specify those settings.
//
// The configurable settings include: image format (jpg, png, bmp); jpg
// quality, tags, and then some things specific to TFS, including server
// name, and project name.
//
// If these settings aren't available at the time the screen is snapped,
// the plugin prompts the user to provide those settings via the
// OptionsForm.  If the server and the project aren't available after closing
// that form, obviously the plugin cannot do its work, so it quits.
//
// If however, the server + project are available, the plugin connects
// to TFS via the TFS client assemblies available through Team Explorer.
//
// The settings can also be modified out of band, by selecting the
// "Options..." menu cjhoice in the Cropper popup menu, clicking the
// plugins tab, then selecting the TFS plugin panel.
//
// Dino Chiesa
// Sat, 04 Dec 2010 20:58
//




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

using CropperPlugins.Common;       // for Tracing


namespace Cropper.TFSWorkItem
{
    public class TFS :
        DesignablePlugin,
        IConfigurablePlugin,
        Microsoft.TeamFoundation.Client.IClientLinking  // see ExecuteDefaultAction below
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
        ///   Implements the method specified in
        ///   Microsoft.TeamFoundation.Client.IClientLinking .
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This is the only method on for the vs2008 version of
        ///     IClientLinking.  This method doesn't actually do anything, and
        ///     the expectation is that it will never actually be called.  The
        ///     purpose of providing it here is to force .NET to resolve the
        ///     Microsoft.TeamFoundation.Client.dll at the time Cropper does its
        ///     plugin scan.
        ///   </para>
        ///   <para>
        ///     Without explicitly implementing the interface, the TFS plugin
        ///     might load on a machine without the TFS client installed on it.
        ///     This will lead to assembly load failures later, and hence
        ///     crashes in Cropper (See workitem 14952).  By implementing the
        ///     interface we avoid that runtime error.
        ///   </para>
        /// </remarks>
        public bool ExecuteDefaultAction(string a, string b)
        {
            return false;
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
                    return ImageFormat.Jpeg;

                if (String.Compare(Extension, "bmp", true) == 0)
                    return ImageFormat.Bmp;

                return ImageFormat.Png;
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
            if (!VerifyMinimumSettings()) return;

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
                        Process process = new Process
                            {
                                StartInfo = new ProcessStartInfo(_settings.ImageEditor,
                                                                 "\"" + fileName + "\"")
                            };
                        process.Start();
                        process.WaitForExit();
                    }
                    workItem.Attachments.Add(new Attachment(fileName, attachmentComment));
                    var form = new WorkItemEditorForm(SelectedWorkItemType, workItem);
                    form.Show();
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
                    return _settings.DefaultImageFormat;
                return "png";
            }
        }

        public WorkItemType SelectedWorkItemType { get; set; }

        private void SetWorkItemType()
        {
            _tfs = new TeamFoundationServer(_settings.TeamServer);
            _tfs.EnsureAuthenticated();
            _wis = _tfs.GetService(typeof(WorkItemStore)) as WorkItemStore;
            SelectedWorkItemType = _wis.Projects[_settings.TeamProject]
                .WorkItemTypes[_settings.WorkItemType];
        }


        private bool VerifyMinimumSettings()
        {
            Tracing.Trace("TFSWI::VerifyMinimumSettings");

            if (!PluginSettings.Completed)
            {
                var dlg = new OptionsForm(PluginSettings);
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
                MessageBox.Show(ex.Message,
                                Cropper.TFSWorkItem.TFS.PluginDescription,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
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
                return PluginSettings;
            }
            set {
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
