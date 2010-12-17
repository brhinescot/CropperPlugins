#region Info
// Animated GIF Plugin for Cropper
//
// WHAT IS IT?
// Cropper is a great free screen capture program, available here:
// http://blogs.geekdojo.net/brian/articles/Cropper.aspx.  It has a cool
// plugin system which lets you send the screenshots anywhere you can
// write code to send it. I wrote this plugin to save a portion of the
// screen to an Animated GIF.
//
// INSTALLATION
// To install, copy all files to your Croppper\Plugins directory.
//
// CREDITS
// This plugin was written by Jon Galloway
// (http://weblogs.asp.net/jgalloway).  I made use of some code from a
// CodeProject article, NGif
// (http://www.codeproject.com/dotnet/NGif.asp). This code was a direct
// Java to .NET port of Kevin Weiner's AnimatedGifEncoder
// (http://www.fmsware.com/stuff/gif.html), which in turn was based on a
// bunch of public domain code dating back as far as 1994. The GIF
// Quantization uses NeuQuant, an interesting Neural Network quantizer
// (http://members.ozemail.com.au/~dekker/NEUQUANT.HTML).
//
// My plugin code is published under public domain license. Based on my
// research on the origin of the code above, I believe all the NGif code
// is public domain as well. I did some significant refactoring to the
// NGif code; the CodeProject version was a direct port from Java and
// didn't leverage .NET Framework classes like System.Drawing. Bottom
// line - all this code is under public domain license.
//
// You can download the code and source for this plugin from :
// http://tools.veloc-it.com
#endregion

using Gif.Components;
using Fusion8.Cropper.Extensibility;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Cropper.AnimatedGif
{
    /// <summary>
    /// Animated GIF Plugin for Cropper
    /// </summary>
    public class Plugin : IPersistableImageFormat, IDisposable, IConfigurablePlugin
    {
        [DllImport("Kernel32.dll")]
        public static extern bool Beep(UInt32 frequency, UInt32 duration);

        private IPersistableOutput _output;
        private AnimatedGifEncoder _AnimatedGifEncoder;
        private Timer _recordTimer;
        private ImageHandler _imageHandler;
        private DateTime _previousTimestamp = DateTime.MinValue;
        private string _previousHash = string.Empty;
        private Image _previousImage;
        private string _fileName;
        private bool _isDisposed;
        private bool _record;

        public event ImageFormatClickEventHandler ImageFormatClick;

        public Plugin()
        {
            this._recordTimer = new Timer();
            this._imageHandler = new ImageHandler(HandleImage);
        }

        /// <summary>
        ///   Override ToString() in support of IConfigurablePlugin.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This string shows up in the dropdown on the Plugins tab
        ///     inside the "Options..." dialog.
        ///   </para>
        /// </remarks>
        public override String ToString()
        {
            return "Animated GIF";
        }

        private void _recordTimer_Tick(object sender, EventArgs e)
        {
            this._output.FetchCapture(_imageHandler);
        }

        private void HandleImage(Image image)
        {
            string currentHash = GetHashFromImage(image);
            DateTime timeStamp = DateTime.Now;

            //Check if image has changed from previous
            if(currentHash != _previousHash)
            {
                if(_previousTimestamp != DateTime.MinValue)
                {
                    //This is not the first image being added
                    AddImageToAnimation(timeStamp);
                }
                StoreImage(image, currentHash, timeStamp);
            }
        }

        private void StoreImage(Image image, string imageHash, DateTime timeStamp)
        {
            _previousImage = (Image)image.Clone();
            _previousHash = imageHash;
            _previousTimestamp = timeStamp;
        }

        private void AddImageToAnimation(DateTime timeStamp)
        {
            TimeSpan timeSpan = timeStamp.Subtract(_previousTimestamp);
            _AnimatedGifEncoder.SetDelay(timeSpan.Milliseconds);
            this._AnimatedGifEncoder.AddFrame(_previousImage);
            _previousImage = null;
        }

        private void FinishCapture()
        {
            if(_previousImage != null)
                AddImageToAnimation(DateTime.Now);
            if(_AnimatedGifEncoder != null)
            {
                this._AnimatedGifEncoder.Finish();
                Beep(4000,30);
                if (PluginSettings.PopViewer && (this._fileName != null))
                    System.Diagnostics.Process.Start(this._fileName);
            }
        }

        // http://www.codeproject.com/dotnet/comparingimages.asp
        private string GetHashFromImage(Image image)
        {
            System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();
            byte[] imageBytes = new byte[1];
            imageBytes = (byte[])ic.ConvertTo(image, imageBytes.GetType());
            System.Security.Cryptography.MD5CryptoServiceProvider md5 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(imageBytes);
            return BitConverter.ToString(hashBytes);
        }

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }
            this._output = persistableOutput;
            this._recordTimer.Interval = PluginSettings.CaptureInterval;
            this._recordTimer.Tick += new EventHandler(this._recordTimer_Tick);
            this._output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
            this._output.ImageCapturing += new ImageCapturingEventHandler(this.persistableOutput_ImageCapturing);
        }

        public void Disconnect()
        {
            this._output.ImageCaptured -= new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
            this._output.ImageCapturing -= new ImageCapturingEventHandler(this.persistableOutput_ImageCapturing);
            this._recordTimer.Dispose();
            FinishCapture();
        }

        public void Dispose()
        {
            if (!this._isDisposed)
            {
                this._isDisposed = true;
                this._recordTimer.Dispose();
                FinishCapture();
            }
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            ImageFormatEventArgs args = new ImageFormatEventArgs();
            args.ClickedMenuItem = (MenuItem) sender;
            args.ImageOutputFormat = this;
            this.OnImageFormatClick(sender, args);
        }

        private void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            if (this.ImageFormatClick != null)
            {
                this.ImageFormatClick(sender, e);
            }
        }

        private void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            if (this._record)
            {
                //Initialize values;
                _previousHash = string.Empty;
                _previousTimestamp = DateTime.MinValue;
                _fileName = e.ImageNames.FullSize;

                this._AnimatedGifEncoder = new AnimatedGifEncoder();
                this._AnimatedGifEncoder.Start(e.ImageNames.FullSize);
                this._AnimatedGifEncoder.SetDelay(PluginSettings.CaptureInterval);
                this._AnimatedGifEncoder.SetQuality(PluginSettings.EncodingQuality);
                this._AnimatedGifEncoder.SetRepeat(PluginSettings.Repeats);
                this._recordTimer.Start();
                Beep(1000,30);
            }
            else
            {
                this._recordTimer.Stop();
                FinishCapture();
            }
        }

        private void persistableOutput_ImageCapturing(object sender, ImageCapturingEventArgs e)
        {
            this._record = !this._record;
            //Sure would be nice to be able to update the UI... if (this._record{}else{}
        }


        public string Description
        {
            get
            {
                return  "Animated Gif";
            }
        }

        public string Extension
        {
            get
            {
                return "Gif";
            }
        }

        public IPersistableImageFormat Format
        {
            get
            {
                return this;
            }
        }

        public MenuItem Menu
        {
            get
            {
                MenuItem item = new MenuItem();
                item.RadioCheck = true;
                item.Text = Description;
                item.Click += new EventHandler(this.MenuItemClick);
                return item;
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

        private void OptionsSaved(object sender, EventArgs e)
        {
            OptionsForm form = sender as OptionsForm;
            if (form == null) return;
            form.ApplySettings();
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
            get { return PluginSettings; }
            set { PluginSettings = value as AnimatedGifSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private AnimatedGifSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new AnimatedGifSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        #endregion


        private AnimatedGifSettings _settings;
        private OptionsForm _configForm;

    }

    public class OptionsForm : Fusion8.Cropper.Extensibility.BaseConfigurationForm
    {
        private AnimatedGifSettings _settings;

        public OptionsForm(AnimatedGifSettings settings)
        {
            InitializeComponent();

            _settings = settings;

            this.chkPopViewer.Checked = _settings.PopViewer;
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
            this.tabPage1.Controls.Add(this.lblPopViewer);
            this.tabPage1.Controls.Add(this.chkPopViewer);
            //
            // chkPopViewer
            //
            this.chkPopViewer.Location = new System.Drawing.Point(104, 5);
            this.chkPopViewer.Text = "";
            this.chkPopViewer.Name = "chkPopViewer";
            this.chkPopViewer.TabIndex = 11;
            this.tooltip.SetToolTip(chkPopViewer, "check to open the image in\nthe default viewer after capture.");
            //
            // lblPopViewer
            //
            this.lblPopViewer.AutoSize = true;
            this.lblPopViewer.Location = new System.Drawing.Point(4, 8);
            this.lblPopViewer.Name = "lblPopViewer";
            this.lblPopViewer.Size = new System.Drawing.Size(68, 13);
            this.lblPopViewer.TabIndex = 60;
            this.lblPopViewer.Text = "Pop Viewer?";
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


    public class AnimatedGifSettings
    {
        private static readonly int DEFAULT_CAPTURE_INTERVAL = 100;
        private static readonly int DEFAULT_ENCODING_QUALITY = 10;
        private static readonly int DEFAULT_REPEATS = 0;
        private int _CaptureInterval;
        private int _EncodingQuality;
        private int _Repeats;

        public AnimatedGifSettings()
        {
            // defaults
            PopViewer = true;
            _CaptureInterval = DEFAULT_CAPTURE_INTERVAL;
            _EncodingQuality = DEFAULT_ENCODING_QUALITY;
            _Repeats = DEFAULT_REPEATS;
        }

        /// <summary>
        ///   True: pop the default GIF viewer with the capture, after it's complete.
        ///   False: don't.
        /// </summary>
        public bool PopViewer { get; set; }

        /// <summary>
        ///   The interval in ms on which a frame will be captured.
        /// </summary>
        public int CaptureInterval
        {
            get
            {
                return _CaptureInterval;
            }
            set
            {
                _CaptureInterval = (value > 20 && value < 1000)
                    ? value
                    : DEFAULT_CAPTURE_INTERVAL;
            }
        }

        /// <summary>
        ///   The GIF Encoder quality level.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   Valid values are between 1 and 20 inclusive.  1 = highest quality,
        ///   but slowest encoding. Levels above 20 do not appreciably
        ///   increase speed.  The default is 10.
        ///   </para>
        /// </remarks>
        public int EncodingQuality
        {
            get
            {
                return _EncodingQuality;
            }
            set
            {
                _EncodingQuality = (value <= 20 && value > 0)
                    ? value
                    : DEFAULT_ENCODING_QUALITY;
            }
        }

        /// <summary>
        ///   The number of repeats for the Animated GIF.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   -1 implies no repeat at all.  Zero implies infinite repeat.
        ///   Any other number implies the number of times to repeat the play.
        ///   The default is 0, infinite repeats.
        ///   </para>
        /// </remarks>
        public int Repeats
        {
            get
            {
                return _Repeats;
            }
            set
            {
                _Repeats = (value >= -1)
                    ? value
                    : DEFAULT_REPEATS;
            }
        }
   }
}
