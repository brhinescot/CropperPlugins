using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.Proxy;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Client;
using System.Configuration;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;

namespace Cropper.TFSWorkItem
{
    public partial class OptionsForm : Form
    {
        public OptionsForm(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;
            if (!String.IsNullOrEmpty(settings.TeamServer))
            {
                TFS = new TeamFoundationServer(
                    settings.TeamServer);
                TFS.EnsureAuthenticated();
                if (!String.IsNullOrEmpty(settings.TeamProject))
                {
                    TeamProject = settings.TeamProject;
                    if (!String.IsNullOrEmpty(settings.WorkItemType))
                    {
                        cmbWorkItemType.Enabled = true;
                        cmbWorkItemType.SelectedItem = settings.WorkItemType;
                    }
                }
            }
            txtDefaultImageName.Text = settings.DefaultImageName;
            cmbDefaultOutputExtension.SelectedItem = settings.DefaultOutputExtension;
            txtDefaultAttachmentComment.Text = settings.DefaultAttachmentComment;
            cbDoNotShowAgain.Checked = settings.DoNotShowOptionsDialogAgain;
            txtImageEditor.Text = settings.ImageEditor;
            cbOpenImageInEditor.Checked = settings.OpenImageInEditor;
        }

        private Settings settings;
        private WorkItemTypeCollection workItemTypeCollection;

        private TeamFoundationServer tfs;

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
                        cmbWorkItemType.Enabled = true;
                        cmbWorkItemType.Items.Clear();
                        WorkItemStore wis = tfs.GetService(typeof(WorkItemStore)) as WorkItemStore;
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            settings.TeamServer = lblTeamServer.Text; ;
            settings.TeamProject = lblTeamProject.Text;
            settings.WorkItemType = cmbWorkItemType.Text;
            settings.DoNotShowOptionsDialogAgain = cbDoNotShowAgain.Checked;
            settings.DefaultImageName = txtDefaultImageName.Text;
            settings.DefaultOutputExtension = cmbDefaultOutputExtension.Text;
            settings.DefaultAttachmentComment = txtDefaultAttachmentComment.Text;
            settings.ImageEditor = txtImageEditor.Text;
            settings.OpenImageInEditor = cbOpenImageInEditor.Checked;
        }

        private void cmbWorkItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWorkItemType.SelectedItem != null)
            {
                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        private void btnBrowseImageEditor_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtImageEditor.Text = openFileDialog.FileName;
            }
        }
    }
}