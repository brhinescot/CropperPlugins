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

namespace Cropper.TFSWorkItem
{
    public class TFS : IPersistableImageFormat
    {
        private IPersistableOutput _output;
        private Settings settings = new Settings();
        private TeamFoundationServer tfs;
        private WorkItemStore wis;

        private WorkItemType selectedWorkItemType;

        public WorkItemType SelectedWorkItemType
        {
            get { return selectedWorkItemType; }
            set { selectedWorkItemType = value; }
        }

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }
            _output = persistableOutput;
            _output.ImageCaptured += new ImageCapturedEventHandler(persistableOutput_ImageCaptured);
            LoadSettings();
        }

        private ImageFormat GetImageFormat(string extension)
        {
            if (String.Compare(extension, "jpg", true) == 0)
            {
                return ImageFormat.Jpeg;
            }
            else if (String.Compare(extension, "bmp", true) == 0)
            {
                return ImageFormat.Bmp;
            }
            else
            {
                return ImageFormat.Png;
            }
        }

        private void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            CaptureOptionsForm frmCaptureOptions = new CaptureOptionsForm(settings, tfs);
            if (frmCaptureOptions.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string fileName = Path.Combine(Path.GetTempPath(), frmCaptureOptions.ImageName + "." + Extension);
                    string attachmentComment = frmCaptureOptions.AttachmentComment;
                    bool openImageInEditor = frmCaptureOptions.OpenImageInEditor;
                    string imageEditor = frmCaptureOptions.ImageEditor;
                    WorkItem workItem = frmCaptureOptions.SelectedWorkItem;
                    e.FullSizeImage.Save(fileName, GetImageFormat(Extension));
                    if (openImageInEditor)
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo(settings.ImageEditor, "\"" + fileName + "\"");
                        Process process = new Process();
                        process.StartInfo = processStartInfo;
                        process.Start();
                        process.WaitForExit();
                    }
                    workItem.Attachments.Add(new Attachment(fileName, attachmentComment));
                    WorkItemEditorForm frmWorkItemEditor = new WorkItemEditorForm(selectedWorkItemType, workItem);
                    frmWorkItemEditor.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Constants.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Action canceled.", Constants.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        public string Description
        {
            get 
            {
                return Constants.PluginDescription;    
            }
        }

        public void Disconnect()
        {
            _output.ImageCaptured -= new ImageCapturedEventHandler(persistableOutput_ImageCaptured);
        }

        public string Extension
        {
            get 
            {
                if (!String.IsNullOrEmpty(settings.DefaultOutputExtension))
                {
                    return settings.DefaultOutputExtension;
                }
                else
                {
                    return "png";
                }
            }
        }

        public IPersistableImageFormat Format
        {
            get 
            { 
                return this; 
            }
        }

        private void SetWorkItemType()
        {
            tfs = new TeamFoundationServer(settings.TeamServer);
            tfs.EnsureAuthenticated();
            wis = tfs.GetService(typeof(WorkItemStore)) as WorkItemStore;
            selectedWorkItemType = wis.Projects[settings.TeamProject].WorkItemTypes[settings.WorkItemType];
        }

        private void LoadSettings()
        {
            if ((!settings.DoNotShowOptionsDialogAgain) ||
                (Control.ModifierKeys == Keys.Shift) ||
                (String.IsNullOrEmpty(settings.TeamServer)) ||
                (String.IsNullOrEmpty(settings.TeamProject)) ||
                (String.IsNullOrEmpty(settings.WorkItemType)))
            {
                OptionsForm frmOptions = new OptionsForm(settings);
                if (frmOptions.ShowDialog() != DialogResult.OK)
                {
                    Disconnect();
                    return;
                }
            }
            try
            {
                SetWorkItemType();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Constants.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Disconnect();
            }
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            ImageFormatEventArgs args1 = new ImageFormatEventArgs();
            args1.ClickedMenuItem = (MenuItem)sender;
            args1.ImageOutputFormat = this;
            this.ImageFormatClick.Invoke(sender, args1);
        }

        public event ImageFormatClickEventHandler ImageFormatClick;

        public MenuItem Menu
        {
            get 
            {
                MenuItem menuItem = new MenuItem();
                menuItem.RadioCheck = true;
                menuItem.Text = Constants.PluginDescription;
                menuItem.Click += new EventHandler(menuItem_Click);
                return menuItem;
            }
        }
    }
}
