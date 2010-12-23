// PicasaUploadHttpForm.cs
// ------------------------------------------------------------------
//
// Models an HTTP Form for uploading a photo to Picasa.
// Patterned after code in the Microsoft.Http library.
//
// Author     : Dino
// Created    : Sat Dec 11 09:37:44 2010
// Last Saved : <2010-December-23 17:19:47>
//
// ------------------------------------------------------------------
//
// Copyright (c) 2010 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using Microsoft.Http;

using CropperPlugins.Utils;

namespace Cropper.SendToPicasa
{
    internal class PicasaUploadHttpForm // : ICreateHttpContent
    {
        // workitem 14947:
        //
        // While this class is modelled after a pattern in the Microsoft.Http.dll,
        // and so theoretically *could* explicitly support ICreateHttpContent,
        // specifying that interface as explicitly supported by this class causes a
        // plugin load error by Cropper, if Microsoft.Http.dll is not in the GAC.
        // This happens even if Microsoft.Http.dll is available in the
        // ApplicationBase of the plugin (eg, the plugins subdir of the Cropper
        // install directory). The symptom is that this plugin simply does not
        // appear in the list of available plugins when Cropper starts up.

        public HttpContent CreateHttpContent()
        {
            var c = new PicasaUploadHttpFormContent(this);
            return HttpContent.Create(new Action<Stream>(c.WriteTo), c.ContentType);
        }

        // Properties
        public string File { get; set; }
        public string Summary { get; set; }

        private sealed class PicasaUploadHttpFormContent
        {
            private readonly string boundary;
            private static System.DateTime _unixEpoch =
                new System.DateTime(1970,1,1, 0,0,0, DateTimeKind.Utc);

            // Methods
            public PicasaUploadHttpFormContent(PicasaUploadHttpForm form)
            {
                this.Form = form;
                this.boundary = "xxxx" + PhpTime.ToString();
                this.ContentType =
                    String.Format("multipart/related; boundary=\"{0}\"", boundary);
                boundary = "--" + boundary;
            }

            private static Int64 PhpTime
            {
                get
                {
                    System.TimeZone tz = System.TimeZone.CurrentTimeZone;
                    System.DateTime now = System.DateTime.Now;
                    System.TimeSpan ts = tz.GetUtcOffset(now);
                    System.DateTime utc = System.DateTime.Now - ts;
                    System.TimeSpan delta =  utc - _unixEpoch;
                    Int64 phpTime = (System.Int64)(delta.TotalSeconds * 1000);
                    return phpTime;
                }
            }

            public void WriteTo(Stream stream)
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write("Media multipart posting\r\n");
                    writer.Write(boundary  + "\r\n");
                    string desc = "Content-Type: application/atom+xml\r\n" +
                                  "\r\n" +
                                  "<entry xmlns='http://www.w3.org/2005/Atom'>\r\n" +
                                  "  <title>{0}</title>\r\n" +
                                  "  <summary>{1}</summary>\r\n" +
                                  "  <category scheme='http://schemas.google.com/g/2005#kind'\r\n" +
                                  "    term='http://schemas.google.com/photos/2007#photo'/>\r\n" +
                                  "</entry>\r\n";
                    string shortFileName = this.Form.File.Contains("\\")
                        ? System.IO.Path.GetFileName(this.Form.File)
                        : this.Form.File;
                    writer.Write(String.Format(desc, shortFileName, this.Form.Summary));
                    writer.Write(boundary + "\r\n");
                    var mimetype = GetMimeType(this.Form.File);
                    writer.Write(String.Format("Content-Type: {0}\r\n\r\n", mimetype));
                    writer.Flush();
                    stream.Flush();
                    // write binary image data
                    using (var fs = System.IO.File.OpenRead(this.Form.File))
                    {
                        var buffer = new byte[1024];
                        int n;
                        while ((n= fs.Read(buffer,0, buffer.Length)) > 0)
                            stream.Write(buffer, 0, n);
                    }
                    stream.Flush();
                    writer.Write("\r\n" + boundary + "--\r\n");
                    writer.Flush();
                }
            }


            private static string GetMimeType(String filename)
            {
                var extension = System.IO.Path.GetExtension(filename).ToLower();
                var regKey =  Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);

                string result =
                    ((regKey != null) && (regKey.GetValue("Content Type") != null))
                    ? regKey.GetValue("Content Type").ToString()
                    : "image/unknown" ;
                return result;
            }


            // Properties
            public string ContentType { get; private set; }
            public PicasaUploadHttpForm Form { get;  private set; }
        }
    }
}
