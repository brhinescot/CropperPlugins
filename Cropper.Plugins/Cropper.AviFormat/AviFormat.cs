#region License Information 

/**********************************************************************************
Shared Source License for Cropper
Copyright 9/07/2004 Brian Scott
http://blogs.geekdojo.net/brian

This license governs use of the accompanying software ('Software'), and your
use of the Software constitutes acceptance of this license.

You may use the Software for any commercial or noncommercial purpose,
including distributing derivative works.

In return, we simply require that you agree:
1. Not to remove any copyright or other notices from the Software. 
2. That if you distribute the Software in source code form you do so only
   under this license (i.e. you must include a complete copy of this license
   with your distribution), and if you distribute the Software solely in
   object form you only do so under a license that complies with this
   license.
3. That the Software comes "as is", with no warranties. None whatsoever.
   This means no express, implied or statutory warranty, including without
   limitation, warranties of merchantability or fitness for a particular
   purpose or any warranty of title or non-infringement. Also, you must pass
   this disclaimer on whenever you distribute the Software or derivative
   works.
4. That no contributor to the Software will be liable for any of those types
   of damages known as indirect, special, consequential, or incidental
   related to the Software or this license, to the maximum extent the law
   permits, no matter what legal theory it’s based on. Also, you must pass
   this limitation of liability on whenever you distribute the Software or
   derivative works.
5. That if you sue anyone over patents that you think may apply to the
   Software for a person's use of the Software, your license to the Software
   ends automatically.
6. That the patent rights, if any, granted in this license only apply to the
   Software, not to any derivative works you make.
7. That the Software is subject to U.S. export jurisdiction at the time it
   is licensed to you, and it may be subject to additional export or import
   laws in other places.  You agree to comply with all such laws and
   regulations that may apply to the Software after delivery of the software
   to you.
8. That if you are an agency of the U.S. Government, (i) Software provided
   pursuant to a solicitation issued on or after December 1, 1995, is
   provided with the commercial license rights set forth in this license,
   and (ii) Software provided pursuant to a solicitation issued prior to
   December 1, 1995, is provided with “Restricted Rights” as set forth in
   FAR, 48 C.F.R. 52.227-14 (June 1987) or DFAR, 48 C.F.R. 252.227-7013
   (Oct 1988), as applicable.
9. That your rights under this License end automatically if you breach it in
   any way.
10.That all rights not expressly granted to you in this license are reserved.

**********************************************************************************/

#endregion

#region Using Directives

using System;
using System.Drawing;
using System.Windows.Forms;
using AviFile;
using Fusion8.Cropper.Extensibility;

#endregion

namespace CropperPlugins
{
	/// <summary>
	/// Summary description for BmpFormat.
	/// </summary>
	public class AviFormat : IPersistableImageFormat, IDisposable
	{
		private const string FORMAT_NAME = "Avi";
		private string description = "Avi - Stopped";
		private bool record;
		private bool isDisposed;

		private AviManager aviManager;
		private VideoStream stream;
		private IPersistableOutput output;
		private Timer recordTimer = new Timer();
		public event ImageFormatClickEventHandler ImageFormatClick;

		public IPersistableImageFormat Format
		{
			get { return this; }
		}

		public string Extension
		{
			get { return FORMAT_NAME; }
		}

		public string Description
		{
			get { return description; }
		}

		public MenuItem Menu
		{
			get
			{
				MenuItem mi = new MenuItem();
				mi.RadioCheck = true;
				mi.Text = FORMAT_NAME;
				mi.Click += new EventHandler(MenuItemClick);
				return mi;
			}
		}

		public void Connect(IPersistableOutput persistableOutput)
		{
			if (persistableOutput == null)
				throw new ArgumentNullException("persistableOutput");

			output = persistableOutput;
			recordTimer.Interval = 100;
			recordTimer.Tick += new EventHandler(_recordTimer_Tick);
			output.ImageCaptured += new ImageCapturedEventHandler(persistableOutput_ImageCaptured);
			output.ImageCapturing += new ImageCapturingEventHandler(persistableOutput_ImageCapturing);
		}

		public void Disconnect()
		{
			output.ImageCaptured -= new ImageCapturedEventHandler(persistableOutput_ImageCaptured);
			output.ImageCapturing -= new ImageCapturingEventHandler(persistableOutput_ImageCapturing);
			recordTimer.Dispose();
		}

		private void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
		{
			if (record)
			{
				aviManager = new AviManager(e.ImageNames.FullSize, false);
				stream = aviManager.AddVideoStream(true, 10, (Bitmap) e.FullSizeImage);
				recordTimer.Start();
			}
			else
			{
				recordTimer.Stop();
				aviManager.Close();
			}
		}

		private void _recordTimer_Tick(object sender, EventArgs e)
		{
			output.FetchCapture(new ImageHandler(AddImageToAvi));
		}

		private void AddImageToAvi(Image image)
		{
			stream.AddFrame((Bitmap) image);
			image.Dispose();
		}

		private void MenuItemClick(object sender, EventArgs e)
		{
			ImageFormatEventArgs formatEvents = new ImageFormatEventArgs();
			formatEvents.ClickedMenuItem = (MenuItem) sender;
			formatEvents.ImageOutputFormat = this;
			OnImageFormatClick(sender, formatEvents);
		}

		private void OnImageFormatClick(object sender, ImageFormatEventArgs e)
		{
			if (ImageFormatClick != null)
				ImageFormatClick(sender, e);
		}

		private void persistableOutput_ImageCapturing(object sender, ImageCapturingEventArgs e)
		{
			record = !record;

			if (record)
				description = "Avi - Recording";
			else
			{
				description = "Avi - Stopped";
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				recordTimer.Dispose();
				stream.Close();
				aviManager.Close();
			}
		}

		#endregion
	}
}