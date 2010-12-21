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
        private System.Windows.Forms.Timer _recordTimer;
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
            this._recordTimer = new System.Windows.Forms.Timer();
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
                if (PluginSettings.PlaySound)
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
            args.ClickedMenuItem = (System.Windows.Forms.MenuItem) sender;
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
                if (PluginSettings.PlaySound)
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

        public System.Windows.Forms.MenuItem Menu
        {
            get
            {
                var item = new System.Windows.Forms.MenuItem();
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
}
