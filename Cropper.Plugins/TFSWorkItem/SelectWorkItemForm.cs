using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Cropper.TFSWorkItem
{
    public partial class SelectWorkItemForm : Form
    {
        private const string wiqlFormat = "SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.TeamProject] = '{0}' ORDER BY [System.Id], [System.Title]";

        public SelectWorkItemForm(WorkItemStore wis, string teamProject)
        {
            InitializeComponent();
            workItemResultGrid = new WorkItemResultGrid();
            workItemResultGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            workItemResultGrid.MultiSelect = false;
            workItemResultGrid.Dock = DockStyle.Fill;
            workItemResultGrid.StretchLastColumn = true;
            string wiql = String.Format(wiqlFormat, teamProject);
            workItemResultGrid.LoadFromWiql(wis, wiql);
            workItemResultGrid.SelectionChanged += new EventHandler(workItemResultGrid_SelectionChanged);
            workItemResultGrid.DoubleClick += new EventHandler(workItemResultGrid_DoubleClick);
            this.Controls.Add(workItemResultGrid);
        }

        public string WorkItemId
        {
            get 
            { 
                return (string)workItemResultGrid.SelectedRows[0].Cells[0].Value; 
            }
        }

        void workItemResultGrid_DoubleClick(object sender, EventArgs e)
        {
            if (workItemResultGrid.SelectedRows.Count == 1)
            {
                DialogResult = DialogResult.OK;
            }
        }

        void workItemResultGrid_SelectionChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = workItemResultGrid.SelectedRows.Count == 1;
        }

        WorkItemResultGrid workItemResultGrid;

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (workItemResultGrid.SelectedRows.Count == 1)
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}