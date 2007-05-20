#region Info
//Animated GIF Plugin for Cropper
//
//WHAT IS IT?
//Cropper is a great free screen capture program, available here: http://blogs.geekdojo.net/brian/articles/Cropper.aspx. 
//It has a cool plugin system which lets you send the screenshots anywhere you can write code to send it. I wrote this 
//plugin to save a portion of the screen to an Animated GIF.
//
//INSTALLATION
//To install, copy all files to your Croppper\Plugins directory.
//
//CREDITS
//This plugin was written by Jon Galloway (http://weblogs.asp.net/jgalloway). 
//I made use of some code from a CodeProject article, NGif (http://www.codeproject.com/dotnet/NGif.asp). This code was a 
//direct Java to .NET port of Kevin Weiner's AnimatedGifEncoder (http://www.fmsware.com/stuff/gif.html), which in turn was 
//based on a bunch of public domain code dataing back as far as 1994. The GIF Quantization uses NeuQuant, an interesting 
//Neural Network quantizer (http://members.ozemail.com.au/~dekker/NEUQUANT.HTML).
//
//My plugin code is published under public domain license. Based on my research on the origin of the code above, I believe 
//all the NGif code is public domain as well. I did some significant refactoring to the NGif code; the CodeProject version 
//was a direct port from Java and didn't leverage .NET Framework classes like System.Drawing. Bottom line - all this code 
//is under public domain license.
//
//You can download the code and source for this plugin from : http://tools.veloc-it.com
#endregion 

using Gif.Components;
using Fusion8.Cropper.Extensibility;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CropperPlugins
{
    /// <summary>
    /// Animated GIF Plugin
    /// </summary>
    public class AnimatedGifFormat : IPersistableImageFormat, IDisposable
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
		private string _description;
		private bool _isDisposed;
		private bool _record;
		private const string FORMAT_NAME = "Animated Gif";

		public event ImageFormatClickEventHandler ImageFormatClick;

		public AnimatedGifFormat()
		{
			this._description = FORMAT_NAME;
			this._recordTimer = new Timer();
			this._imageHandler = new ImageHandler(HandleImage);
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
				this._AnimatedGifEncoder.Finish();
			Beep(4000,30);
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
			this._recordTimer.Interval = 100;
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

				this._AnimatedGifEncoder = new AnimatedGifEncoder();
				this._AnimatedGifEncoder.Start(e.ImageNames.FullSize);
				this._AnimatedGifEncoder.SetDelay(100);
				this._AnimatedGifEncoder.SetQuality(15);

				//-1:no repeat,0:always repeat
				this._AnimatedGifEncoder.SetRepeat(0);
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
				return this._description;
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
				item.Text = FORMAT_NAME;
				item.Click += new EventHandler(this.MenuItemClick);
				return item;
			}
		}
	}
}