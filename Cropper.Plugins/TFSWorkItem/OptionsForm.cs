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
    public partial class OptionsForm : Fusion8.Cropper.Extensibility.BaseConfigurationForm
    {
        private TfsSettings settings;
        private WorkItemTypeCollection workItemTypeCollection;
        private TeamFoundationServer _tfs;
        private string _teamProject;


        public OptionsForm(TfsSettings settings)
        {
            InitializeComponent();

            this.settings = settings;
            if (!String.IsNullOrEmpty(settings.TeamServer))
            {
                TFS = new TeamFoundationServer(settings.TeamServer);
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
            txtDefaultImageName.Text           = settings.DefaultImageName;
            cmbImageFormat.SelectedItem = settings.DefaultImageFormat;
            txtDefaultAttachmentComment.Text   = settings.DefaultAttachmentComment;
            txtImageEditor.Text                = settings.ImageEditor;
            cbOpenImageInEditor.Checked        = settings.OpenImageInEditor;
            SelectedImageFormatChanged(null,null);
            HandleQualitySliderValueChanged(null,null);
        }


        /// <summary>
        ///   Show the OK and Cancel buttons.
        /// </summary>
        ///
        /// <remarks>
        ///   This form can be shown in two ways: as a standalone
        ///   dialog, and hosted within the tabbed "Options" UI provided
        ///   by the Cropper Core.  By default, the OK and Cancel
        ///   buttons are not visible.  When used as a standalone dialog
        ///   the caller should invoke this method before calling
        ///   ShowDialog().
        /// </remarks>
        public void MakeButtonsVisible()
        {
            btnOK.Visible = true;
            btnCancel.Visible = true;
        }

        public TeamFoundationServer TFS
        {
            get
            {
                return _tfs;
            }
            set
            {
                _tfs = value;
                if (_tfs != null)
                {
                    lblTeamServer.Text = _tfs.Uri.AbsoluteUri;
                }
            }
        }


        public string TeamProject
        {
            get { return _teamProject; }
            set
            {
                _teamProject = value;
                if (String.IsNullOrEmpty(_teamProject))
                {
                    lblTeamProject.Text = "(not set)";
                }
                else
                {
                    lblTeamProject.Text = _teamProject;
                    if (_tfs != null)
                    {
                        cmbWorkItemType.Enabled = true;
                        cmbWorkItemType.Items.Clear();
                        WorkItemStore wis = _tfs.GetService(typeof(WorkItemStore)) as WorkItemStore;
                        workItemTypeCollection = wis.Projects[_teamProject].WorkItemTypes;
                        foreach (WorkItemType workItemType in workItemTypeCollection)
                        {
                            cmbWorkItemType.Items.Add(workItemType.Name);
                        }
                    }
                }
            }
        }

        public int JpgImageQuality
        {
            get
            {
                return this.qualitySlider.Value;
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
            this.ApplySettings();
        }

        public void ApplySettings()
        {
            //System.Diagnostics.Debugger.Break();
            if (this.TFS != null)
            {
                settings.TeamServer           = this.TFS.Uri.AbsoluteUri;
                settings.TeamProject          = this.lblTeamProject.Text;
            }
            settings.WorkItemType             = this.cmbWorkItemType.Text;
            settings.DefaultImageName         = this.txtDefaultImageName.Text;
            settings.DefaultImageFormat       = this.cmbImageFormat.Text;
            settings.DefaultAttachmentComment = this.txtDefaultAttachmentComment.Text;
            settings.ImageEditor              = this.txtImageEditor.Text;
            settings.OpenImageInEditor        = this.cbOpenImageInEditor.Checked;
            settings.JpgImageQuality          = this.JpgImageQuality;

        }

        private void cmbWorkItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (cmbWorkItemType.SelectedItem != null)
            // {
            //     btnOK.Enabled = true;
            // }
            // else
            // {
            //     btnOK.Enabled = false;
            // }
        }

        private void SelectedImageFormatChanged(object sender, EventArgs e)
        {
            if (this.cmbImageFormat.Text == "jpg")
            {
                qualitySlider.Visible = true;
                qualitySlider.Enabled = true;
                this.txtDefaultAttachmentComment.Location = new System.Drawing.Point(94, 180);
                this.label5.Location = new System.Drawing.Point(4, 182);
                this.btnBrowseImageEditor.Location = new System.Drawing.Point(202, 210);
                this.cbOpenImageInEditor.Location = new System.Drawing.Point(6, 210);
                this.txtImageEditor.Location = new System.Drawing.Point(94, 232);
                this.label7.Location = new System.Drawing.Point(4, 234);
            }
            else
            {
                qualitySlider.Visible = false;
                qualitySlider.Enabled = false;
                this.txtDefaultAttachmentComment.Location = new System.Drawing.Point(94, 144);
                this.label5.Location = new System.Drawing.Point(4, 146);
                this.btnBrowseImageEditor.Location = new System.Drawing.Point(202, 174);
                this.cbOpenImageInEditor.Location = new System.Drawing.Point(6, 174);
                this.txtImageEditor.Location = new System.Drawing.Point(94, 196);
                this.label7.Location = new System.Drawing.Point(4, 198);
            }
        }

        private void btnBrowseImageEditor_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "EXE Files|*.exe";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (System.IO.File.Exists(dlg.FileName))
                    txtImageEditor.Text = dlg.FileName;
            }
        }

        private void HandleQualitySliderValueChanged(object sender, System.EventArgs e)
        {
            this.tooltip.SetToolTip(qualitySlider,
                                    "quality=" + qualitySlider.Value.ToString());
        }
   }
}