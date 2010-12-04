using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using System.Xml;

namespace Cropper.TFSWorkItem
{
    public partial class WorkItemEditorForm : Form
    {
        private WorkItemFormControl frmWorkItemControl;

        public WorkItemEditorForm(WorkItemType workItemType, WorkItem workItem)
        {
            InitializeComponent();
            frmWorkItemControl = new WorkItemFormControl();
            XmlDocument xmlDocument = workItemType.Export(false);
            frmWorkItemControl.FormDefinition = xmlDocument.InnerXml;
            frmWorkItemControl.Dock = DockStyle.Fill;
            frmWorkItemControl.Item = workItem;
            this.Controls.Add(frmWorkItemControl);
            frmWorkItemControl.BringToFront();
        }

        private void WorkItemEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (frmWorkItemControl.Item.IsDirty)
            {
                DialogResult dialogResult = MessageBox.Show("Would you like to save the work item?",
                    Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    try
                    {
                        frmWorkItemControl.Item.Save();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            if (frmWorkItemControl.Item.IsDirty)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    frmWorkItemControl.Item.Save();
                    tsslBugId.Text = frmWorkItemControl.Item.Id.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            if (!frmWorkItemControl.Item.IsDirty)
            {
                Cursor = Cursors.WaitCursor;
                try
                {
                    frmWorkItemControl.Item.SyncToLatest();
                    tsslBugId.Text = frmWorkItemControl.Item.Id.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Cropper.TFSWorkItem.TFS.PluginDescription, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }
    }
}