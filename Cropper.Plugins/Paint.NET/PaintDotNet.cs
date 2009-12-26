//#define Trace

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Diagnostics;         // for Conditional
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Fusion8.Cropper.Extensibility;
using System.Collections.Generic;



namespace Cropper.SendToPaintDotNet
{
    public class PaintDotNet : IPersistableImageFormat
    {
        public PaintDotNet ()
        {
            SetupDebugConsole(); // for debugging purposes
        }
        
        public event ImageFormatClickEventHandler ImageFormatClick;

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
                throw new ArgumentNullException("persistableOutput");

            this._output = persistableOutput;
            this._output.ImageCaptured += this.ImageCaptured;

        }

        public void Disconnect()
        {
            this._output.ImageCaptured -= this.ImageCaptured;
        }

        
        private void menuItem_Click(object sender, EventArgs e)
        {
            ImageFormatEventArgs args1 = new ImageFormatEventArgs();
            args1.ClickedMenuItem = (MenuItem) sender;
            args1.ImageOutputFormat = this;
            this.ImageFormatClick.Invoke(sender, args1);
        }


        
        private void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            ImagePairNames names1 = e.ImageNames;
            this._logger = new PdnLogWriter(new FileInfo(names1.FullSize).DirectoryName);

            if (this._isThumbEnabled = e.IsThumbnailed)
            {
                this._thumbFileName = e.ImageNames.Thumbnail;
                this._thumbnailImage = e.ThumbnailImage;
            }

            this._fileName = e.ImageNames.FullSize;
            this._output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
        }



        /// <summary>
        ///   Saves the captured image to an on-disk file. 
        /// </summary>
        ///
        /// <remarks>
        ///   Saving the image to a disk file isn't strictly necessary to enable upload
        ///   of the image via the HTML FORM, but it's nice to have a cached version of
        ///   the image in the filesystem.
        /// </remarks>
        private void SaveImage(Stream stream, Image image)
        {
            bool success = false;
            try
            {
                Trace("+--------------------------------");
                Trace("SaveImage ({0})", _fileName);
                image.Save(stream, ImageFormat.Png);

                if (this._isThumbEnabled)
                {
                    using (FileStream stream1 = new FileStream(this._thumbFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        this._thumbnailImage.Save(stream1, ImageFormat.Png);
                        this._thumbnailImage.Dispose();
                        stream1.Close();
                    }
                }
                success = true;
            }
            catch (Exception exception1)
            {
                string msg = "There's been an exception while saving the image: " +
                    exception1.Message + "\n" + exception1.StackTrace;
                msg+= "\n\nYou will have to Upload this file manually: " + this._fileName ;
                MessageBox.Show(msg);
                return;
            }
            finally
            {
                image.Dispose();
                stream.Close();
            }

            if (success)
            {
                // launch Paint.NET
                try
                {
                    System.Diagnostics.Process.Start(PdnExecutable, _fileName);
                }
                catch { }
            }
        }



        private string _PdnExecutable;
        private string PdnExecutable
        {
            get
            {
                if (_PdnExecutable== null)
                {
                    string _AppRegyPath = "Software\\Paint.NET";
                    string valueName = "TARGETDIR";
                    try 
                    {
                    using (var HklmPdnKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_AppRegyPath, true))
                    {
                        string s = (string)HklmPdnKey.GetValue(valueName);
                        _PdnExecutable = Path.Combine(s, "PaintDotNet.exe");
                    }
                    }
                    catch (System.Exception)
                    {
                        _PdnExecutable = "";
                    }
                }
                return _PdnExecutable;
            }
        }

        
        public string Description
        {
            get
            {
                return "Send to Paint.NET";
            }
        }

        public string Extension
        {
            get
            {
                return "png";
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
                MenuItem item1 = new MenuItem();
                item1.RadioCheck = true;
                item1.Enabled = !String.IsNullOrEmpty(PdnExecutable);
                item1.Text = Description;
                item1.Click += new EventHandler(this.menuItem_Click);
                return item1;
            }
        }



        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        public static extern bool FreeConsole();


        /// <summary>
        /// This pops a console window to emit debugging messages into,
        /// at runtime.  It is compiled with Conditiona("Trace") so these messages
        /// never appear when Trace is not #define'd. 
        /// </summary>
        [Conditional("Trace")]
        private void SetupDebugConsole()
        {
            if ( !AttachConsole(-1) )  // Attach to a parent process console
                AllocConsole();        // Allocate a new console

            _process= System.Diagnostics.Process.GetCurrentProcess();
            System.Console.WriteLine();
        }

    
        [Conditional("Trace")]
        private void Trace(string format, params object[] args)
        {
            // these messages appear in the allocated console.
            System.Console.Write("{0:D5} ", _process.Id);
            System.Console.WriteLine(format, args);
        }


        private System.Diagnostics.Process _process;  // debugging only
        private string _fileName;
        private bool _isThumbEnabled;
        private PdnLogWriter _logger;
        private IPersistableOutput _output;
        private string _thumbFileName;
        private Image _thumbnailImage;
    }



}

