using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Proxy;
using System.IO;

namespace Cropper.TFSWorkItem
{
    public partial class AttachDialog : Form
    {
        private TfsSettings settings;
        private WorkItemStore wis;
        private WorkItemTypeCollection workItemTypeCollection;

        private TeamFoundationServer tfs;


        public AttachDialog(TfsSettings settings, TeamFoundationServer tfs)
        {
            InitializeComponent();
            this.settings = settings;
            if (tfs != null)
            {
                this.TFS = tfs;
            }
            else
            {
                if (!String.IsNullOrEmpty(settings.TeamServer))
                {
                    TFS = new TeamFoundationServer(
                        settings.TeamServer);
                    TFS.EnsureAuthenticated();
                }
            }
            if (!String.IsNullOrEmpty(settings.TeamProject))
            {
                TeamProject = settings.TeamProject;
                if (!String.IsNullOrEmpty(settings.WorkItemType))
                {
                    cmbWorkItemType.Enabled = true;
                    cmbWorkItemType.SelectedItem = settings.WorkItemType;
                }
            }
            txtImageName.Text = settings.DefaultImageName;
            txtAttachmentComment.Text = settings.DefaultAttachmentComment;
            txtImageEditor.Text = settings.ImageEditor;
            cbOpenImageInEditor.Checked = settings.OpenImageInEditor;
        }

        public TeamFoundationServer TFS
        {
            get
            {
                return tfs;
            }
            set
            {
                tfs = value;
                if (tfs != null)
                {
                    lblTeamServer.Text = tfs.Uri.AbsoluteUri;
                }
            }
        }

        private string teamProject;

        public string TeamProject
        {
            get { return teamProject; }
            set
            {
                teamProject = value;
                if (!String.IsNullOrEmpty(teamProject))
                {
                    lblTeamProject.Text = teamProject;
                    if (tfs != null)
                    {
                        cmbWorkItemType.Items.Clear();
                        wis = tfs.GetService(typeof(WorkItemStore)) as WorkItemStore;
                        workItemTypeCollection = wis.Projects[teamProject].WorkItemTypes;
                        foreach (WorkItemType workItemType in workItemTypeCollection)
                        {
                            cmbWorkItemType.Items.Add(workItemType.Name);
                        }
                    }
                }
                else
                {
                    lblTeamProject.Text = "{Press button to select project}";
                }
            }
        }

        private WorkItem selectedWorkItem;

        public WorkItem SelectedWorkItem
        {
            get
            {
                return selectedWorkItem;
            }
            set
            {
                selectedWorkItem = value;
            }
        }

        private string imageName;

        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }

        private string attachmentComment;

        public string AttachmentComment
        {
            get { return attachmentComment; }
            set { attachmentComment = value; }
        }

        private bool openImageInEditor;

        public bool OpenImageInEditor
        {
            get { return openImageInEditor; }
            set { openImageInEditor = value; }
        }

        private string imageEditor;

        public string ImageEditor
        {
            get { return imageEditor; }
            set { imageEditor = value; }
        }

        private WorkItem GetSelectedWorkItem()
        {
            WorkItem workItem = null;
            try
            {
                if (rbCreateNewWorkItem.Checked)
                {
                    workItem = new WorkItem(workItemTypeCollection[cmbWorkItemType.Text]);
                }
                else if (rbAddToExistingWorkItem.Checked)
                {
                    try
                    {
                        workItem = wis.GetWorkItem(int.Parse(txtWorkItemId.Text));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return workItem;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            imageName = txtImageName.Text;
            attachmentComment = txtAttachmentComment.Text;
            imageEditor = txtImageEditor.Text;
            openImageInEditor = cbOpenImageInEditor.Checked;
            if ((!String.IsNullOrEmpty(imageName)) &&
                ((cbOpenImageInEditor.Checked && !String.IsNullOrEmpty(txtImageEditor.Text) || (!cbOpenImageInEditor.Checked))))
            {
                selectedWorkItem = GetSelectedWorkItem();
                if (selectedWorkItem != null)
                {
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("Please select a valid work item.", Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (String.IsNullOrEmpty(imageName))
            {
                MessageBox.Show("Please enter image name.", Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (cbOpenImageInEditor.Checked && String.IsNullOrEmpty(txtImageEditor.Text))
            {
                MessageBox.Show("Please enter image editor.", Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbOpenImageInEditor_CheckedChanged(object sender, EventArgs e)
        {
            txtImageEditor.Enabled = cbOpenImageInEditor.Checked;
        }

        private void btnSelectTeamProject_Click(object sender, EventArgs e)
        {
            DomainProjectPicker domainProjectPicker = new DomainProjectPicker(DomainProjectPickerMode.AllowProjectSelect);
            if (domainProjectPicker.ShowDialog() == DialogResult.OK)
            {
                TFS = domainProjectPicker.SelectedServer;
                TeamProject = domainProjectPicker.SelectedProjects[0].Name;
                lblTeamServer.Text = domainProjectPicker.SelectedServer.Uri.AbsoluteUri;
                lblTeamProject.Text = domainProjectPicker.SelectedProjects[0].Name;
            }
        }

        private void btnSelectWorkItem_Click(object sender, EventArgs e)
        {
            SelectWorkItemForm frmSelectWorkItem = new SelectWorkItemForm(wis, teamProject);
            if (frmSelectWorkItem.ShowDialog() == DialogResult.OK)
            {
                txtWorkItemId.Text = frmSelectWorkItem.WorkItemId;
            }
        }

        private void rbAddToExistingWorkItem_CheckedChanged(object sender, EventArgs e)
        {
            txtWorkItemId.Enabled = rbAddToExistingWorkItem.Checked;
            btnSelectWorkItem.Enabled = rbAddToExistingWorkItem.Checked;
        }

        private void rbCreateNewWorkItem_CheckedChanged(object sender, EventArgs e)
        {
            cmbWorkItemType.Enabled = rbCreateNewWorkItem.Checked;
        }
    }
}