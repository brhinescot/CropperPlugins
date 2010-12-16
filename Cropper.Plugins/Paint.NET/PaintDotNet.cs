// PaintDotNet.cs
//
// Code for a cropper plugin that saves a screen snap, then opens it in
// Paint.NET.  Optionally, it uploads the edited screen shot to a photo
// service.
//
// Dino Chiesa
// 2010 Nov 9
//

using System;
using System.Linq;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Automation;       // AutomationElement
using System.Diagnostics;              // Process
using System.Reflection;               // Assembly, etc..
using Fusion8.Cropper.Extensibility;
using System.Collections.Generic;

using CropperPlugins.Utils;       // for Tracing

namespace Cropper.SendToPaintDotNet
{
    public class PaintDotNet : DesignablePluginThatUsesFetchOutputStream,
        IConfigurablePlugin
    {
        public override string Description
        {
            get { return "Send to Paint.NET"; }
        }

        public override string Extension
        {
            get { return "png"; }
        }

        public override string ToString()
        {
            return "Send to Paint.NET [Dino Chiesa]";
        }

        public override MenuItem Menu
        {
            get
            {
                MenuItem item1 = base.Menu;
                // disable if we cannot find the EXE
                item1.Enabled = !String.IsNullOrEmpty(PdnExecutable);
                return item1;
            }
        }


#if WANT_VERBOSE_TRACE
        protected override void OnImageFormatClick(object sender, ImageFormatEventArgs e)
        {
            Tracing.Trace("Imgur::MenuClick");
            base.OnImageFormatClick(sender, e);
        }
#endif


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
            catch (Exception exc1)
            {
                string msg = "There's been an exception while saving the image: " +
                             exc1.Message + "\n" + exc1.StackTrace;
                msg+= "\n\nYou will have to Upload this file manually: " + this._fileName ;
                MessageBox.Show(msg,
                                "Cropper Plugin for Paint.NET",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
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
                Tracing.Trace("Invoking PDN '{0}'  '{1}'", PdnExecutable, _fileName);
                // System.Diagnostics.Process.Start(PdnExecutable, _fileName);
                // workitem 13537 - must quote the actual filename
                try
                {
                    System.Diagnostics.Process.Start(PdnExecutable,
                                                     String.Format("\"{0}\"",_fileName));
                }
                catch (Exception exc1)
                {
                    string msg = "There's been an exception starting Paint.NET: " +
                                 exc1.Message + "\n" + exc1.StackTrace;
                    MessageBox.Show(msg,
                                    "Cropper Plugin for Paint.NET",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }

                if (PluginSettings.PostEditUpload != null)
                {
                    try
                    {
                        PostEditUpload();
                    }
                    catch (Exception exc1)
                    {
                        string msg = "There's been an exception while uploading the image: " +
                                     exc1.Message + "\n" + exc1.StackTrace;
                        msg+= "\n\nYou will have to Upload this file manually: " + this._fileName ;
                        MessageBox.Show(msg,
                                        "Cropper Plugin for Paint.NET",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                }

            }
        }



        /// <summary>
        ///   Wait for PaintDotNet, return the process ID.
        /// </summary>
        private Process WaitForPdn()
        {
            System.Threading.Thread.Sleep(PluginSettings.DelayStart.Milliseconds);
            for (int i=0; i < 5; i++)
            {
                System.Threading.Thread.Sleep(250*(i*i+1));

                var s= from p in Process.GetProcesses()
                    where p.ProcessName.Contains("PaintDotNet")
                    select p;

                if (s.Count()>0)
                    return s.First();
            }

            return null;
        }


        private void PostEditUpload()
        {
            Process pdnProc = WaitForPdn();
            if (pdnProc == null)
            {
                Tracing.Trace("PDN appears to not be running.");
                return;
            }
            bool canUpload = WaitForPdnExit(pdnProc);

            if (canUpload)
            {
                Tracing.Trace("PaintDotNet:: will now upload...");
                ActuallyUpload();
            }
        }


        private bool WaitForPdnExit(Process process)
        {
            var shortFileName = Path.GetFileName(this._fileName);
            var window = AutomationElement.RootElement.FindChildByProcessId(process.Id);
            System.Threading.Thread.Sleep(1250);
            string name =
                window.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;

            if (!name.StartsWith(shortFileName))
            {
                Tracing.Trace("PDN appears to NOT be editing the file.");
                return false;
            }

            try
            {
                int cycles = 0;
                do
                {
                    System.Threading.Thread.Sleep(800);
                    name = window.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;
                    if (!name.StartsWith(shortFileName)) break;
                    cycles++;
                } while (cycles < (PluginSettings.DelayEdit.Milliseconds / 800));

                if (!name.StartsWith(shortFileName))
                {
                    Tracing.Trace("PDN is done editing.");
                    return true;
                }
                Tracing.Trace("Timeout.");
                return false;
            }
            catch (ElementNotAvailableException)
            {
                Tracing.Trace("PDN has exited.");
                return true;
            }
        }


        private void ActuallyUpload()
        {
            var pluginSettings =
                Fusion8
                .Cropper
                .Core
                .Configuration
                .Current
                .PluginSettings;

            var GetSettings = new Func<Type,object>((t) => {
                    foreach (object o in pluginSettings)
                    {
                        if (o.GetType() == t)
                            return o;
                    }
                    return null;
                });

            Tracing.Trace("using plugin {0}", PluginSettings.PostEditUpload.DllName);

            var pluginDirectory = Path.GetDirectoryName(this.GetType().Assembly.Location);
            var dllName = Path.Combine(pluginDirectory, PluginSettings.PostEditUpload.DllName);
            var assembly = Assembly.LoadFrom(dllName.Replace("\\\\","\\"));
            var plugin = assembly.CreateInstance(PluginSettings.PostEditUpload.TypeName);
            var ipif = plugin as IPersistableImageFormat;

            var icp = plugin as IConfigurablePlugin;

            // try to set settings on the instance
            if (icp != null)
            {
                var settingsType = icp.Settings.GetType();
                object settings = GetSettings(settingsType);
                if (settings != null)
                {
                    Tracing.Trace("applying settings...");
                    icp.Settings = settings;
                }
            }

            var flags1 = BindingFlags.SetField |
                BindingFlags.Instance |
                BindingFlags.NonPublic;

            // set field:
            plugin.GetType().InvokeMember("_fileName",
                                          flags1,
                                          null,
                                          plugin,
                                          new Object[]{  this._fileName });

            var flags2 = BindingFlags.InvokeMethod |
                BindingFlags.Instance |
                BindingFlags.NonPublic;

            // invoke the UploadImage method:
            plugin.GetType().InvokeMember("UploadImage",
                                          flags2,
                                          null,
                                          plugin,
                                          null);
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


        private static bool FileExists(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                Tracing.Trace("FileExists: filename is empty.");
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
                    _configForm = new PdnOptionsForm(PluginSettings);
                    _configForm.OptionsSaved += OptionsSaved;
                }
                return _configForm;
            }
        }

        private void OptionsSaved(object sender, EventArgs e)
        {
            Tracing.Trace("Plugin::OptionsSaved");
            PdnOptionsForm form = sender as PdnOptionsForm;
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
            set { PluginSettings = value as PdnSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private PdnSettings PluginSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new PdnSettings();
                return _settings;
            }
            set { _settings = value; }
        }

        #endregion

        private PdnSettings _settings;
        private PdnOptionsForm _configForm;

        private string _fileName;
        private bool _isThumbEnabled;
        private PdnLogWriter _logger;
        private string _thumbFileName;
        private Image _thumbnailImage;
    }


    public static class AutomationExtensions
    {
        public static AutomationElement FindChildByProcessId(this AutomationElement element, int pid)
        {
            var cond = new PropertyCondition(AutomationElement.ProcessIdProperty, pid);
            var result = element.FindChildByCondition(cond);
            return result;
        }

        public static AutomationElement FindChildByCondition(this AutomationElement element, Condition cond)
        {
            var result = element.FindFirst(TreeScope.Children, cond);
            return result;
        }
    }


    public class PluginInfo
    {
        public string DllName { get; set; }
        public string TypeName { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public Object Instance { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public String FriendlyName { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public PluginInfo Self { get { return this;}}
    }

    public class TimeDelay
    {
        public String FriendlyName { get; set; }
        public int Milliseconds { get; set; }
    }

    public class PdnSettings
    {
        public PdnSettings()
        {
        }

        /// <summary>
        ///   The plugin to use for post-edit upload, if any.
        /// </summary>
        public PluginInfo PostEditUpload { get; set; }

        /// <summary>
        ///   The time for the plugin to wait for PDN to start.
        /// </summary>
        public TimeDelay DelayStart { get; set; }

        /// <summary>
        ///   The time allowed for user edits before the plugin gives up.
        /// </summary>
        public TimeDelay DelayEdit { get; set; }
    }

}

