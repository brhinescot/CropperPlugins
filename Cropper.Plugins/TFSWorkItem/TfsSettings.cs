using System;
using System.IO;
//using System.Collections.Generic;
//using System.Text;
//using System.Configuration;

using CropperPlugins.Common;

namespace Cropper.TFSWorkItem
{
    public class TfsSettings
    {
        public TfsSettings()
        {
            // Intelligently set the default image editor
            if (FileExists(PdnExecutable))
                ImageEditor = PdnExecutable;
            else if (FileExists(MsPaintExecutable))
                ImageEditor = MsPaintExecutable;
        }

        private string _MsPaintExecutable;
        private string MsPaintExecutable
        {
            get
            {
                if (_PdnExecutable== null)
                {
                    string putativeMsPaintExe = null;
                    var windir = System.Environment.GetEnvironmentVariable("WinDir");
                        if (!String.IsNullOrEmpty(windir))
                        {
                            putativeMsPaintExe = System.IO.Path.Combine(System.IO.Path.Combine(windir, "System32"), "msPaint.exe");
                            if (FileExists(putativeMsPaintExe))
                                _MsPaintExecutable = putativeMsPaintExe;
                        }
                }
                return _MsPaintExecutable;
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
                    Tracing.Trace("TFSWI::Settings: Looking for PaintDotNet in the registry...");
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
                        Tracing.Trace("TFSWI::Settings: Looking for PaintDotNet in the filesystem...");

                        var pgmFiles = System.Environment.GetEnvironmentVariable("ProgramFiles");
                        if (!String.IsNullOrEmpty(pgmFiles))
                        {
                            putativePdnExe = System.IO.Path.Combine(System.IO.Path.Combine(pgmFiles, "Paint.NET"), "PaintDotNet.exe");
                            if (FileExists(putativePdnExe))
                                _PdnExecutable = putativePdnExe;
                            else
                            {
                                // handle case where Paint.NET is an x64 program
                                Tracing.Trace("TFSWI::Settings: Looking in %ProgramW6432%...");

                                pgmFiles = System.Environment.GetEnvironmentVariable("ProgramW6432");
                                if (!String.IsNullOrEmpty(pgmFiles))
                                {
                                    putativePdnExe = System.IO.Path.Combine(System.IO.Path.Combine(pgmFiles, "Paint.NET"), "PaintDotNet.exe");
                                    if (FileExists(putativePdnExe))
                                        _PdnExecutable = putativePdnExe;
                                }
                                else
                                    Tracing.Trace("TFSWI::Settings: No joy.");
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
                Tracing.Trace("TFSWI::Settings:FileExists: filename is empty.");
                return false;
            }
            if (File.Exists(filename))
            {
                Tracing.Trace("TFSWI::Settings:FileExists: YES '{0}'.", filename);
                return true;
            }

            Tracing.Trace("TFSWI::Settings:FileExists: No '{0}'.", filename);
            return false;
        }



        public bool Completed
        {
            get
            {
                return (!(String.IsNullOrEmpty(TeamServer)) ||
                        (String.IsNullOrEmpty(TeamProject)) ||
                        (String.IsNullOrEmpty(WorkItemType)));
            }
        }

        public string TeamServer
        {
            get; set;
        }

        public string TeamProject
        {
            get; set;
        }

        public string WorkItemType
        {
            get; set;
        }

        public string DefaultImageName
        {
            get; set;
        }

        public string DefaultImageFormat
        {
            get; set;
        }

        public int JpgImageQuality
        {
            get; set;
        }

        public string DefaultAttachmentComment
        {
            get; set;
        }

        public bool OpenImageInEditor
        {
            get; set;
        }

        public string ImageEditor
        {
            get; set;
        }
    }
}
