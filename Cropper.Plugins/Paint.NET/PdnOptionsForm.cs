using System;
using System.IO;
using System.Reflection;
using Fusion8.Cropper.Extensibility;
using CropperPlugins.Utils;       // for Tracing

namespace Cropper.SendToPaintDotNet
{
    public partial class PdnOptionsForm : BaseConfigurationForm
    {
        private PdnSettings _settings;

        public PdnOptionsForm(PdnSettings settings)
        {
            Tracing.Trace("PdnOptionsForm: ctor");

            InitializeComponent();

            _settings = settings;
            PopulatePluginComboBox();

            if (settings.PostEditUpload == null)
            {
                this.chkPostEditUpload.Checked = false;
            }
            else
            {
                this.chkPostEditUpload.Checked = true;
                SelectIntelligently(this.cmbPlugins,
                                    (x)=> ((x as PluginInfo).DllName ==
                                           settings.PostEditUpload.DllName),
                                    0);
            }

            PopulateDelayComboBoxes();

            if (settings.DelayStart == null)
                this.cmbDelayStart.SelectedIndex = 0;
            else
                SelectIntelligently(this.cmbDelayStart,
                                    (x)=> ((x as TimeDelay).Milliseconds ==
                                           settings.DelayStart.Milliseconds),
                                    0);


            if (settings.DelayEdit == null)
                this.cmbDelayEdit.SelectedIndex = 0;
            else
                SelectIntelligently(this.cmbDelayEdit,
                                    (x)=> ((x as TimeDelay).Milliseconds ==
                                           settings.DelayEdit.Milliseconds),
                                    1);
            UploadCheckedChanged(null,null);
        }



        private void SelectIntelligently(System.Windows.Forms.ComboBox cmb,
                                         Func<object,bool> test,
                                         int defaultIndex)
        {
            foreach (var item in this.cmbPlugins.Items)
            {
                if (test(item))
                {
                    cmb.SelectedItem = item;
                    break;
                }
            }
            if (cmb.SelectedIndex < 0)
            {
                // just select the first one
                cmb.SelectedIndex = defaultIndex;
            }
        }


        public void ApplySettings()
        {
            _settings.PostEditUpload   =
                this.chkPostEditUpload.Checked
                ? (this.cmbPlugins.SelectedItem as PluginInfo)
                : null;
            _settings.DelayStart = (this.cmbDelayStart.SelectedItem as TimeDelay);
            _settings.DelayEdit = (this.cmbDelayEdit.SelectedItem as TimeDelay);
        }



        private void PopulateDelayComboBoxes()
        {
            this.cmbDelayStart.Items.AddRange(new TimeDelay[]{
                    new TimeDelay
                    {
                        FriendlyName = "1 seconds",
                        Milliseconds = 1 * 1000
                    },
                    new TimeDelay
                    {
                        FriendlyName = "3 seconds",
                        Milliseconds = 3 * 1000
                    },
                    new TimeDelay
                    {
                        FriendlyName = "5 seconds",
                        Milliseconds = 5 * 1000
                    },
                    new TimeDelay
                    {
                        FriendlyName = "15 seconds",
                        Milliseconds = 15 * 1000
                    }
                });
            this.cmbDelayEdit.Items.AddRange(new TimeDelay[]{
                    new TimeDelay
                    {
                        FriendlyName = "60 seconds",
                        Milliseconds = 60 * 1000
                    },
                    new TimeDelay
                    {
                        FriendlyName = "3 minutes",
                        Milliseconds = 3 * 60 * 1000
                    },
                    new TimeDelay
                    {
                        FriendlyName = "5 minutes",
                        Milliseconds = 5 * 60 * 1000
                    },
                    new TimeDelay
                    {
                        FriendlyName = "10 minutes",
                        Milliseconds = 10 * 60 * 1000
                    }
                });
        }



        private void PopulatePluginComboBox()
        {
            Tracing.Trace("PdnOptionsForm: Looking for plugins that do UploadImage...");
            this.cmbPlugins.Items.Clear();
            var pluginDirectory = Path.GetDirectoryName(this.GetType().Assembly.Location);
            var dlllFiles = Directory.GetFiles(pluginDirectory, "*.dll");
            foreach (var path in dlllFiles)
            {
                Object instance = SuitablePluginForAssembly(path);
                if (instance != null)
                {
                    Tracing.Trace("  {0}", path);
                    this.cmbPlugins.Items.Add(new PluginInfo
                        {
                            DllName = Path.GetFileName(path),
                            TypeName = instance.GetType().FullName,
                            Instance = instance,
                            FriendlyName = instance.ToString()
                        });
                }
            }
            this.cmbPlugins.DisplayMember = "FriendlyName";
            this.cmbPlugins.ValueMember = "Self";
        }



        private static Object SuitablePluginForAssembly(String path)
        {
            var assembly = Assembly.LoadFrom(path);
            var if1Name = typeof(IPersistableImageFormat).Name;
            var if2Name = typeof(IConfigurablePlugin).Name;
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            try
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (!t.IsPublic) continue;
                    if (!t.IsClass) continue;
                    if (t.IsAbstract) continue;
                    if (t.GetInterface(if1Name, true) == null) continue;
                    if (t.GetInterface(if2Name, true) == null) continue;
                    if (t.GetField("_fileName", flags) == null) continue;
                    if (t.GetMethod("UploadImage",
                                    flags,
                                    null,
                                    System.Type.EmptyTypes,
                                    null) == null)
                        continue;

                    // found a type that fits the criteria, return it.
                    return assembly.CreateInstance(t.FullName);
                }
            }
            catch (ReflectionTypeLoadException) { }
            catch (FileLoadException) { }
            return null;
        }


        private void UploadCheckedChanged(object sender, System.EventArgs e)
        {
            this.lblPlugins.Enabled =
                this.cmbPlugins.Enabled =
                this.lblDelayStart.Enabled =
                this.cmbDelayStart.Enabled =
                this.lblDelayEdit.Enabled =
                this.cmbDelayEdit.Enabled =
                this.chkPostEditUpload.Checked;
        }


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkPostEditUpload = new System.Windows.Forms.CheckBox();
            this.lblPostEditUpload = new System.Windows.Forms.Label();
            this.cmbPlugins = new System.Windows.Forms.ComboBox();
            this.lblPlugins = new System.Windows.Forms.Label();
            this.cmbDelayStart = new System.Windows.Forms.ComboBox();
            this.lblDelayStart = new System.Windows.Forms.Label();
            this.cmbDelayEdit = new System.Windows.Forms.ComboBox();
            this.lblDelayEdit = new System.Windows.Forms.Label();
            this.themedTabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            //
            // themedTabControl1
            //
            this.themedTabControl1.Size = new System.Drawing.Size(324, 391);
            //
            // tooltip
            //
            this.tooltip = new System.Windows.Forms.ToolTip();
            this.tooltip.AutoPopDelay = 2400;
            this.tooltip.InitialDelay = 500;
            this.tooltip.ReshowDelay = 500;
            //
            // tabPage1
            //
            this.tabPage1.Controls.Add(this.lblPlugins);
            this.tabPage1.Controls.Add(this.cmbPlugins);
            this.tabPage1.Controls.Add(this.lblDelayStart);
            this.tabPage1.Controls.Add(this.cmbDelayStart);
            this.tabPage1.Controls.Add(this.lblDelayEdit);
            this.tabPage1.Controls.Add(this.cmbDelayEdit);
            this.tabPage1.Controls.Add(this.lblPostEditUpload);
            this.tabPage1.Controls.Add(this.chkPostEditUpload);
            //
            // chkPostEditUpload
            //
            this.chkPostEditUpload.Location = new System.Drawing.Point(108, 4);
            this.chkPostEditUpload.Text = "";
            this.chkPostEditUpload.Name = "chkPostEditUpload";
            this.chkPostEditUpload.TabIndex = 11;
            this.chkPostEditUpload.CheckedChanged += UploadCheckedChanged;
            this.tooltip.SetToolTip(chkPostEditUpload, "check to upload the image after editing.");
            //
            // lblPostEditUpload
            //
            this.lblPostEditUpload.AutoSize = true;
            this.lblPostEditUpload.Location = new System.Drawing.Point(4, 8);
            this.lblPostEditUpload.Name = "lblPostEditUpload";
            this.lblPostEditUpload.Size = new System.Drawing.Size(68, 13);
            this.lblPostEditUpload.TabIndex = 70;
            this.lblPostEditUpload.Text = "Upload after edit?";
            //
            // cmbPlugins
            //
            this.cmbPlugins.FormattingEnabled = false;
            this.cmbPlugins.AllowDrop = false;
            this.cmbPlugins.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlugins.Location = new System.Drawing.Point(108, 34);
            this.cmbPlugins.Name = "cmbPlugins";
            this.cmbPlugins.Size = new System.Drawing.Size(202, 21);
            this.cmbPlugins.DisplayMember = "Name";
            this.cmbPlugins.ValueMember = "Id";
            this.cmbPlugins.Enabled = false;
            this.cmbPlugins.TabIndex = 21;
            this.tooltip.SetToolTip(cmbPlugins, "select the upload mechanism.");
            //
            // lblPlugins
            //
            this.lblPlugins.AutoSize = true;
            this.lblPlugins.Location = new System.Drawing.Point(4, 36);
            this.lblPlugins.Name = "lblPlugins";
            this.lblPlugins.Size = new System.Drawing.Size(50, 13);
            this.lblPlugins.TabIndex = 20;
            this.lblPlugins.Enabled = false;
            this.lblPlugins.Text = "Upload with:";
            //
            // cmbDelayStart
            //
            this.cmbDelayStart.FormattingEnabled = false;
            this.cmbDelayStart.AllowDrop = false;
            this.cmbDelayStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDelayStart.Location = new System.Drawing.Point(108, 62);
            this.cmbDelayStart.Name = "cmbDelayStart";
            this.cmbDelayStart.Size = new System.Drawing.Size(100, 21);
            this.cmbDelayStart.DisplayMember = "FriendlyName";
            this.cmbDelayStart.ValueMember = "Milliseconds";
            this.cmbDelayStart.Enabled = false;
            this.cmbDelayStart.TabIndex = 31;
            this.tooltip.SetToolTip(cmbDelayStart, "specify the interval the plugin will wait\nfor PaintDotNet to start.");
            //
            // lblDelayStart
            //
            this.lblDelayStart.AutoSize = true;
            this.lblDelayStart.Location = new System.Drawing.Point(4, 64);
            this.lblDelayStart.Name = "lblDelayStart";
            this.lblDelayStart.Size = new System.Drawing.Size(50, 13);
            this.lblDelayStart.TabIndex = 30;
            this.lblDelayStart.Text = "Start Delay:";
            //
            // cmbDelayEdit
            //
            this.cmbDelayEdit.FormattingEnabled = false;
            this.cmbDelayEdit.AllowDrop = false;
            this.cmbDelayEdit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDelayEdit.Location = new System.Drawing.Point(108, 90);
            this.cmbDelayEdit.Name = "cmbDelayEdit";
            this.cmbDelayEdit.Size = new System.Drawing.Size(100, 21);
            this.cmbDelayEdit.DisplayMember = "FriendlyName";
            this.cmbDelayEdit.ValueMember = "Milliseconds";
            this.cmbDelayEdit.Enabled = false;
            this.cmbDelayEdit.TabIndex = 41;
            this.tooltip.SetToolTip(cmbDelayEdit, "specify the time the plugin will wait for\ncompletion of edits, before giving up.");
            //
            // lblDelayEdit
            //
            this.lblDelayEdit.AutoSize = true;
            this.lblDelayEdit.Location = new System.Drawing.Point(4, 92);
            this.lblDelayEdit.Name = "lblDelayEdit";
            this.lblDelayEdit.Size = new System.Drawing.Size(50, 13);
            this.lblDelayEdit.TabIndex = 40;
            this.lblDelayEdit.Text = "Edit Period:";
            //
            // Options
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 391);
            this.Name = "Options";
            this.Text = "Configure Paint.NET Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPlugins;
        private System.Windows.Forms.Label lblPlugins;
        private System.Windows.Forms.ComboBox cmbDelayStart;
        private System.Windows.Forms.Label lblDelayStart;
        private System.Windows.Forms.ComboBox cmbDelayEdit;
        private System.Windows.Forms.Label lblDelayEdit;
        private System.Windows.Forms.CheckBox chkPostEditUpload;
        private System.Windows.Forms.Label lblPostEditUpload;
        private System.Windows.Forms.ToolTip tooltip;
    }

}