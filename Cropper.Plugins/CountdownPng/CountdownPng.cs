//#define Trace

using Fusion8.Cropper.Extensibility;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;         // for Conditional


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
        private int countdownDelay = 5;
        private bool copyToClipboard, saveToFile, sendToPdn;
        private MenuItem clipBoardItem, saveToFileItem, sendToPdnItem;
        private System.Diagnostics.Process _process;  // debugging only
        private readonly string noneString = "no delay";
        private string _fileName;
        private bool _isThumbEnabled;
        private string _thumbFileName;
        private Image _thumbnailImage;
        private string _PdnExecutable;


        public CountdownPngFormat()
        {
            SetupDebugConsole(); // for debugging purposes
        }


        private ImageCapturingEventHandler _capturing;
        private ImageCapturingEventHandler capturing
        {
            get
            {
                if (_capturing==null)
                    _capturing =  new ImageCapturingEventHandler(this.ImageCapturing);
                return _capturing;
            }
        }

        private ImageCapturedEventHandler _captured;
        private ImageCapturedEventHandler captured
        {
            get
            {
                if (_captured==null)
                    _captured =  new ImageCapturedEventHandler(this.ImageCaptured);
                return _captured;
            }
        }


        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }

            Trace("+--------------------------------");
            Trace("connect");

            this.output = persistableOutput;
            this.output.ImageCapturing += capturing;
            this.output.ImageCaptured += captured;
        }

        public void Disconnect()
        {
            this.output.ImageCaptured -= captured;
            this.output.ImageCapturing -= capturing;

            Trace("disconnect");
            Trace("+--------------------------------");
        }


        private void TimeSelected(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem) sender;

            this.countdownDelay = (menuItem.Text == noneString)
                ? 0
                : int.Parse(menuItem.Text, CultureInfo.InvariantCulture);

            ImageFormatEventArgs imageFormatEventArgs = new ImageFormatEventArgs();
            imageFormatEventArgs.ClickedMenuItem = menuItem;
            imageFormatEventArgs.ImageOutputFormat = this;
            this.OnImageFormatClick(sender, imageFormatEventArgs);
        }




        private void SaveOptionChanged(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem) sender;
            menuItem.Checked = !menuItem.Checked;
            Trace("SaveOption click {0}", menuItem.Checked);

            // toggle the option. They are not mutually exclusive
            if (menuItem == clipBoardItem)
                this.copyToClipboard = menuItem.Checked;
            else if (menuItem == saveToFileItem)
                this.saveToFile = menuItem.Checked;
            else
                this.sendToPdn = menuItem.Checked;
        }


        private void MenuSelect(object sender, System.EventArgs e)
        {
            Trace("Menu Select {0} {1}", clipBoardItem.Checked, saveToFileItem.Checked);
            clipBoardItem.Checked = this.copyToClipboard;
            saveToFileItem.Checked = this.saveToFile;
            sendToPdnItem.Checked = this.sendToPdn;
        }


        private void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            if (this.ImageFormatClick != null)
                this.ImageFormatClick(sender, e);
        }

        private void ImageCapturing(object sender, ImageCapturingEventArgs e)
        {
            Trace("capturing (sleeping {0}s)", countdownDelay);
            Beep(1000,40);
            System.Threading.Thread.Sleep(countdownDelay * 1000);
        }

        private void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            Trace("captured");
            if (copyToClipboard || (sendToPdn && !saveToFile))
            {
                // put the image on the clipboard
                using (MemoryStream stream = new MemoryStream())
                {
                    e.FullSizeImage.Save(stream, ImageFormat.Png);
                    using (Image bitmap = Bitmap.FromStream(stream))
                        Clipboard.SetDataObject(bitmap, true);
                }
            }

            if (saveToFile)
            {
                if (this._isThumbEnabled = e.IsThumbnailed)
                {
                    this._thumbFileName = e.ImageNames.Thumbnail;
                    this._thumbnailImage = e.ThumbnailImage;
                }

                this._fileName = e.ImageNames.FullSize;
                this.output.FetchOutputStream(new StreamHandler(this.SaveImage), e.ImageNames.FullSize, e.FullSizeImage);
            }
            else if (sendToPdn)
            {
                // not saving, but opening PDN
                try
                {
                    System.Diagnostics.Process.Start(PdnExecutable);
                }
                catch { }
            }


            Beep(4000,40);
        }


        private void SaveImage(Stream stream, Image image)
        {
            Trace("SaveImage");
            image.Save(stream, ImageFormat.Png);

            // also savve the thumb if enabled
            if (this._isThumbEnabled)
            {
                using (FileStream stream1 = new FileStream(this._thumbFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (this._thumbnailImage)
                    {
                        this._thumbnailImage.Save(stream1, ImageFormat.Png);
                    }
                }
            }

            if (this.sendToPdn)
            {
                // launch Paint.NET
                try
                {
                    System.Diagnostics.Process.Start(PdnExecutable, _fileName);
                }
                catch { }
            }
        }


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
                        if (!String.IsNullOrEmpty(s))
                            _PdnExecutable = Path.Combine(s, "PaintDotNet.exe");
                    }
                    }
                    catch (System.Exception)
                    {
                        _PdnExecutable= "";
                    }

                }
                return _PdnExecutable;
            }
        }


        public string Description
        {
            get
            {
                return "Countdown PNG";
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

        private MenuItem AddTimeMenuItem(string text, MenuItem parent)
        {
            MenuItem item = new MenuItem(text);
            item.RadioCheck = true;
            item.Checked = false;
            item.Click += this.TimeSelected;
            parent.MenuItems.Add(item);
            return item;
        }


        public MenuItem Menu
        {
            get
            {
                MenuItem parentItem = new MenuItem();
                parentItem.Text = "Countdown PNG";

                // these values should be stored in the settings file!!

                clipBoardItem = new MenuItem("Copy to Clipboard");
                clipBoardItem.Click += this.SaveOptionChanged;
                clipBoardItem.Checked = this.copyToClipboard = true;
                parentItem.MenuItems.Add(clipBoardItem);

                saveToFileItem = new MenuItem("Save to File");
                saveToFileItem.Click += this.SaveOptionChanged;
                saveToFileItem.Checked = this.saveToFile = true;
                parentItem.MenuItems.Add(saveToFileItem);

                sendToPdnItem = new MenuItem("Send to Paint.NET");
                sendToPdnItem.Enabled = !String.IsNullOrEmpty(PdnExecutable);
                if (sendToPdnItem.Enabled)
                {
                sendToPdnItem.Click += this.SaveOptionChanged;
                sendToPdnItem.Checked = this.sendToPdn= false;
                }
                else
                sendToPdnItem.Checked = this.sendToPdn= false;
                parentItem.MenuItems.Add(sendToPdnItem);


                parentItem.MenuItems.Add(new MenuItem("-"));

                this.AddTimeMenuItem(noneString, parentItem);
                this.AddTimeMenuItem("5", parentItem);
                this.AddTimeMenuItem("10", parentItem);
                this.AddTimeMenuItem("15", parentItem);
                this.AddTimeMenuItem("30", parentItem);
                this.AddTimeMenuItem("60", parentItem);

                foreach (MenuItem i in parentItem.MenuItems)
                {
                    if (i.Text == this.countdownDelay.ToString())
                    {
                        i.Checked = true;
                    }
                }

                parentItem.Select += this.MenuSelect;
                return parentItem;
            }
        }



        // everything below here is used only for debugging purposes The
        // methods get compiled in conditionally, when the symbol
        // "Trace" is defined.


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

    }
}