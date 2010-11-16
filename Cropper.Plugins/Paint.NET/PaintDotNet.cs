// PaintDotNet.cs
//
// Code for a cropper plugin that sends a screen snap to
// Paint.NET.
//
// Dino Chiesa
// 2010 Nov 9
//

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Fusion8.Cropper.Extensibility;
using System.Collections.Generic;

using CropperPlugins.Utils;       // for Tracing

namespace Cropper.SendToPaintDotNet
{
    public class PaintDotNet : DesignablePluginThatUsesFetchOutputStream
    {

#if NOTNOTNOT
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

#endif

        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("Imgur::MenuClick");
            base.OnImageFormatClick(sender, e);
        }


        protected override void ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            ImagePairNames names1 = e.ImageNames;
            this._logger = new PdnLogWriter(new FileInfo(names1.FullSize).DirectoryName);

            if (this._isThumbEnabled = e.IsThumbnailed)
            {
                this._thumbFileName = e.ImageNames.Thumbnail;
                this._thumbnailImage = e.ThumbnailImage;
            }

            this._fileName = e.ImageNames.FullSize;
            output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
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
        protected override void SaveImage(Stream stream, Image image)
        {
            bool success = false;
            try
            {
                Tracing.Trace("+--------------------------------");
                Tracing.Trace("SaveImage ({0})", _fileName);
                image.Save(stream, ImageFormat.Png);

                if (this._isThumbEnabled)
                {
                    using (FileStream stream1 = new FileStream(this._thumbFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        this._thumbnailImage.Save(stream1, ImageFormat.Png);
                        this._thumbnailImage.Dispose();
                        //stream1.Close();
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
                    Tracing.Trace("Invoking PDN '{0}'  '{1}'", PdnExecutable, _fileName);
                    // System.Diagnostics.Process.Start(PdnExecutable, _fileName);
                    // workitem 13537 - must quote the actual filename
                    System.Diagnostics.Process.Start(PdnExecutable,
                        String.Format("\"{0}\"",_fileName));
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
                    string putativePdnExe = null;
                    Tracing.Trace("+--------------------------------");
                    Tracing.Trace("Looking for PaintDotNet in the registry...");
                    string _AppRegyPath = "Software\\Paint.NET";
                    string valueName = "TARGETDIR";
                    try
                    {
                        using (var HklmPdnKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(_AppRegyPath, true))
                        {
                            string s = (string)HklmPdnKey.GetValue(valueName);
                            putativePdnExe = Path.Combine(s, "PaintDotNet.exe");
                            if (FileExists(putativePdnExe))
                                _PdnExecutable = putativePdnExe;
                        }
                    }
                    catch (System.Exception)
                    {
                    }

                    if (_PdnExecutable == null)
                    {
                        Tracing.Trace("Looking for PaintDotNet in the filesystem...");

                        var pgmFiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                        if (!String.IsNullOrEmpty(pgmFiles))
                        {
                            putativePdnExe = System.IO.Path.Combine(System.IO.Path.Combine(pgmFiles, "Paint.NET"), "PaintDotNet.exe");
                            if (FileExists(putativePdnExe))
                                _PdnExecutable = putativePdnExe;
                            else
                            {
                                // handle case where Paint.NET is an x64 program
                                Tracing.Trace("Looking in %ProgramW6432%...");

                                pgmFiles = System.Environment.GetEnvironmentVariable("ProgramW6432");
                                if (!String.IsNullOrEmpty(pgmFiles))
                                {
                                    putativePdnExe = System.IO.Path.Combine(System.IO.Path.Combine(pgmFiles, "Paint.NET"), "PaintDotNet.exe");
                                    if (FileExists(putativePdnExe))
                                        _PdnExecutable = putativePdnExe;
                                }
                                else
                                    Tracing.Trace("No joy.");
                             }
                        }
                    }

                }
                return _PdnExecutable;
            }
        }

        private bool FileExists(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                Tracing.Trace("FileExists: file is empty.");
                return false;
            }
            if (File.Exists(filename))
            {
                Tracing.Trace("FileExists: YES '{0}'.", filename);
                return true;
            }

            Tracing.Trace("FileExists: No '{0}'.", filename);
            return false;
        }

        public override string Description
        {
            get
            {
                return "Send to Paint.NET";
            }
        }

        public override string Extension
        {
            get
            {
                return "png";
            }
        }

#if NOTNOTNOT
        public IPersistableImageFormat Format
        {
            get
            {
                return this;
            }
        }
#endif


        public override MenuItem Menu
        {
            get
            {
                MenuItem item1 = base.Menu;
                item1.Enabled = !String.IsNullOrEmpty(PdnExecutable);
                return item1;
            }
        }

        private string _fileName;
        private bool _isThumbEnabled;
        private PdnLogWriter _logger;
        private string _thumbFileName;
        private Image _thumbnailImage;
    }

}

