using Fusion8.Cropper.Extensibility;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CropperPlugins
{
		public class CountdownPngFormat : IPersistableImageFormat
    {
		[DllImport("Kernel32.dll")]
		public static extern bool Beep(UInt32 frequency, UInt32 duration);
		
		public event ImageFormatClickEventHandler ImageFormatClick;
		private const int EncoderParameterCount = 1;
		private const string EncoderType = "image/png";
		private const string FormatName = "Countdown Png";
		private IPersistableOutput output;
		private int countdownDelay = 10;

        public CountdownPngFormat()
        {
        }

        private MenuItem AddCountdownTimeMenuItem(string text, MenuItem parent)
        {
            MenuItem parentItem = new MenuItem(text);
            parentItem.RadioCheck = true;
            if (text == this.countdownDelay.ToString(CultureInfo.InvariantCulture))
            {
                parentItem.Checked = true;
            }
            parentItem.Click += new EventHandler(this.CountdownTimerMenuHandler);
            parent.MenuItems.Add(parentItem);
            return parentItem;
        }

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }
            this.output = persistableOutput;
			this.output.ImageCapturing += new ImageCapturingEventHandler(this.persistableOutput_ImageCapturing);
			this.output.ImageCaptured += new ImageCapturedEventHandler(this.output_ImageCaptured);
        }

        public void Disconnect()
        {
            this.output.ImageCaptured -= new ImageCapturedEventHandler(this.output_ImageCaptured);
        }

	    private void CountdownTimerMenuHandler(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem) sender;
            this.countdownDelay = int.Parse(menuItem.Text, CultureInfo.InvariantCulture);
            ImageFormatEventArgs imageFormatEventArgs = new ImageFormatEventArgs();
            imageFormatEventArgs.ClickedMenuItem = menuItem;
            imageFormatEventArgs.ImageOutputFormat = this;
            this.OnImageFormatClick(sender, imageFormatEventArgs);
        }

        private void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            if (this.ImageFormatClick != null)
            {
                this.ImageFormatClick(sender, e);
            }
        }

		private void persistableOutput_ImageCapturing(object sender, ImageCapturingEventArgs e)
		{
			Beep(1000,30);
			System.Threading.Thread.Sleep(countdownDelay * 1000);
        }
		
		private void output_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            this.output.FetchOutputStream(new StreamHandler(this.SaveImage), e.ImageNames.FullSize, e.FullSizeImage);
            if (e.IsThumbnailed)
            {
                this.output.FetchOutputStream(new StreamHandler(this.SaveImage), e.ImageNames.Thumbnail, e.ThumbnailImage);
            }
			Beep(4000,30);
        }

		private void SaveImage(Stream stream, Image image)
		{
			image.Save(stream, ImageFormat.Png);
		}

        public string Description
        {
            get
            {
                return "Countdown Png";
            }
        }

        public string Extension
        {
            get
            {
                return "Png";
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
				MenuItem parentItem = new MenuItem();
				parentItem.Text = "Countdown";
				MenuItem childItem = new MenuItem("Time (sec)");
				childItem.Enabled = false;
				parentItem.MenuItems.Add(childItem);
				childItem = new MenuItem("-");
				parentItem.MenuItems.Add(childItem);
				this.AddCountdownTimeMenuItem("5", parentItem);
				this.AddCountdownTimeMenuItem("10", parentItem);
				this.AddCountdownTimeMenuItem("15", parentItem);
				this.AddCountdownTimeMenuItem("30", parentItem);
				this.AddCountdownTimeMenuItem("60", parentItem);
				return parentItem;
            }
        }
    }
}