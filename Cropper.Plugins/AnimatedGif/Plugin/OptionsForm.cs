namespace Cropper.AnimatedGif
{
    using System;
    using System.Windows.Forms;

    public class OptionsForm : Fusion8.Cropper.Extensibility.BaseConfigurationForm
    {
        private AnimatedGifSettings _settings;

        public OptionsForm(AnimatedGifSettings settings)
        {
            InitializeComponent();

            _settings = settings;

            this.chkPopViewer.Checked = _settings.PopViewer;
            this.chkPlaySound.Checked = _settings.PlaySound;
            this.cmbInterval.SelectedItem = _settings.CaptureInterval.ToString();
            this.cmbQuality.SelectedItem = _settings.EncodingQuality.ToString();

            if (_settings.Repeats == 0)
                this.cmbRepeats.SelectedItem = "forever";
            else if (_settings.Repeats == -1)
                this.cmbRepeats.SelectedItem = "none";
            else
                this.cmbRepeats.Text = _settings.Repeats.ToString();
        }

        public void ApplySettings()
        {
            _settings.PopViewer = this.chkPopViewer.Checked;
            _settings.PlaySound = this.chkPlaySound.Checked;
            _settings.CaptureInterval = Int32.Parse(this.cmbInterval.Text);
            _settings.EncodingQuality = Int32.Parse(this.cmbQuality.Text);

            if (this.cmbRepeats.Text == "forever")
                _settings.Repeats = 0;
            else if (this.cmbRepeats.Text == "none")
                _settings.Repeats = -1;
            else
            {
                try
                {
                    _settings.Repeats = Int32.Parse(this.cmbRepeats.Text);
                }
                catch
                {
                    // This happens when the user has keyed in a text
                    // that is not parseable as an integer.
                    // Setting the value to an invalid value will cause the
                    // AnimatedGifSettings class to use a default value.
                    _settings.Repeats = -99;
                }
            }
        }

        private void InitializeComponent()
        {
            this.chkPlaySound = new System.Windows.Forms.CheckBox();
            this.lblPlaySound = new System.Windows.Forms.Label();
            this.chkPopViewer = new System.Windows.Forms.CheckBox();
            this.lblPopViewer = new System.Windows.Forms.Label();
            this.cmbInterval= new System.Windows.Forms.ComboBox();
            this.lblInterval = new System.Windows.Forms.Label();
            this.cmbQuality= new System.Windows.Forms.ComboBox();
            this.lblQuality = new System.Windows.Forms.Label();
            this.cmbRepeats = new System.Windows.Forms.ComboBox();
            this.lblRepeats = new System.Windows.Forms.Label();
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
            this.tabPage1.Text = "Settings";
            this.tabPage1.Controls.Add(this.lblRepeats);
            this.tabPage1.Controls.Add(this.cmbRepeats);
            this.tabPage1.Controls.Add(this.lblQuality);
            this.tabPage1.Controls.Add(this.cmbQuality);
            this.tabPage1.Controls.Add(this.lblInterval);
            this.tabPage1.Controls.Add(this.cmbInterval);
            this.tabPage1.Controls.Add(this.lblPlaySound);
            this.tabPage1.Controls.Add(this.chkPlaySound);
            this.tabPage1.Controls.Add(this.lblPopViewer);
            this.tabPage1.Controls.Add(this.chkPopViewer);
            //
            // chkPlaySound
            //
            this.chkPlaySound.Location = new System.Drawing.Point(104, 4);
            this.chkPlaySound.Text = "";
            this.chkPlaySound.Name = "chkPlaySound";
            this.chkPlaySound.TabIndex = 11;
            this.tooltip.SetToolTip(chkPlaySound, "check to play a short beep to mark the\nstart and end of capture.");
            //
            // lblPlaySound
            //
            this.lblPlaySound.AutoSize = true;
            this.lblPlaySound.Location = new System.Drawing.Point(4, 8);
            this.lblPlaySound.Name = "lblPlaySound";
            this.lblPlaySound.Size = new System.Drawing.Size(68, 13);
            this.lblPlaySound.TabIndex = 60;
            this.lblPlaySound.Text = "Play Sounds?";
            //
            // cmbInterval
            //
            this.cmbInterval.FormattingEnabled = false;
            this.cmbInterval.AllowDrop = false;
            this.cmbInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterval.Items.AddRange(new object[] {
                    "40",
                    "50",
                    "80",
                    "100",
                    "150",
                    "200",
                    "300"});
            this.cmbInterval.Location = new System.Drawing.Point(104, 34);
            this.cmbInterval.Name = "cmbInterval";
            this.cmbInterval.Size = new System.Drawing.Size(72, 21);
            this.cmbInterval.TabIndex = 21;
            this.tooltip.SetToolTip(cmbInterval, "select how often a frame will be captured\nby the plugin. The default is 100ms.");
            //
            // lblInterval
            //
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(4, 36);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(50, 13);
            this.lblInterval.TabIndex = 20;
            this.lblInterval.Text = "Frame Interval:";
            //
            // cmbQuality
            //
            this.cmbQuality.FormattingEnabled = false;
            this.cmbQuality.AllowDrop = false;
            this.cmbQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            for(int i = 1; i <21; i++)
                this.cmbQuality.Items.Add(i.ToString());

            this.cmbQuality.Location = new System.Drawing.Point(104, 62);
            this.cmbQuality.Name = "cmbQuality";
            this.cmbQuality.Size = new System.Drawing.Size(72, 21);
            this.cmbQuality.TabIndex = 31;
            this.tooltip.SetToolTip(cmbQuality, "1 is the highest quality, but is the\nslowest.  20 is the lowest quality,\nbut fast. 10 is the default.");
            //
            // lblQuality
            //
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(4, 64);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(50, 13);
            this.lblQuality.TabIndex = 30;
            this.lblQuality.Text = "Encoding Quality:";
            //
            // cmbRepeats
            //
            this.cmbRepeats.FormattingEnabled = false;
            this.cmbRepeats.AllowDrop = false;
            this.cmbRepeats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbRepeats.Items.AddRange(new object[] { "none", "forever" });
            this.cmbRepeats.Location = new System.Drawing.Point(104, 90);
            this.cmbRepeats.Name = "cmbRepeats";
            this.cmbRepeats.Size = new System.Drawing.Size(72, 21);
            this.cmbRepeats.TabIndex = 41;
            this.tooltip.SetToolTip(cmbRepeats, "Specify how many times the animation will\nrepeat during playback. The total number of\nplays will be n+1. You can specify any\nnon-negative number.");
            //
            // lblRepeats
            //
            this.lblRepeats.AutoSize = true;
            this.lblRepeats.Location = new System.Drawing.Point(4, 92);
            this.lblRepeats.Name = "lblRepeats";
            this.lblRepeats.Size = new System.Drawing.Size(50, 13);
            this.lblRepeats.TabIndex = 40;
            this.lblRepeats.Text = "Playback Repeats:";
            //
            // chkPopViewer
            //
            this.chkPopViewer.Location = new System.Drawing.Point(104, 118);
            this.chkPopViewer.Text = "";
            this.chkPopViewer.Name = "chkPopViewer";
            this.chkPopViewer.TabIndex = 11;
            this.tooltip.SetToolTip(chkPopViewer, "check to open the image in\nthe default viewer after capture.");
            //
            // lblPopViewer
            //
            this.lblPopViewer.AutoSize = true;
            this.lblPopViewer.Location = new System.Drawing.Point(4, 122);
            this.lblPopViewer.Name = "lblPopViewer";
            this.lblPopViewer.Size = new System.Drawing.Size(68, 13);
            this.lblPopViewer.TabIndex = 60;
            this.lblPopViewer.Text = "Pop Viewer?";
            //
            // Options
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 391);
            this.Name = "Options";
            this.Text = "Configure AnimatedGif Options";
            this.themedTabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox chkPlaySound;
        private System.Windows.Forms.Label lblPlaySound;
        private System.Windows.Forms.CheckBox chkPopViewer;
        private System.Windows.Forms.Label lblPopViewer;
        private System.Windows.Forms.ComboBox cmbInterval;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.ComboBox cmbQuality;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.ComboBox cmbRepeats;
        private System.Windows.Forms.Label lblRepeats;
        private System.Windows.Forms.ToolTip tooltip;
    }
}

