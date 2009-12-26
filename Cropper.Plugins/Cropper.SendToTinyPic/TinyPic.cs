//#define Trace

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;         // for Conditional

using Fusion8.Cropper.Extensibility;
using System.Collections.Generic;

namespace Cropper.SendToTinyPic
{
    public class TinyPic : IPersistableImageFormat
    {
        private string _fileName;
        private bool _isThumbEnabled;
        private TinyPicLogWriter _logger;
        private IPersistableOutput _output;
        private Regex _regex;
        private string _thumbFileName;
        private string _errorMessage;
        private Image _thumbnailImage;
        private System.Diagnostics.Process _process;  // debugging only

        //private const string DESCRIPTION = "TinyPic Hosted Image";
        //private const string EXTENSION = "png"; //??


        public TinyPic()
        {
            SetupDebugConsole(); // for debugging purposes only
        }


        public event ImageFormatClickEventHandler ImageFormatClick;

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
                throw new ArgumentNullException("persistableOutput");

            Trace("+--------------------------------");
            Trace("connect");

            this._output = persistableOutput;
            this._output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
            this._regex = new Regex("<strong><a href=\"(?<imgurl>[^\"]+)\" ([^>]*?)>Click here</a> to view your image</strong>",
                                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            // example line:
            // <strong><a href="http://tinypic.com/view.php?pic=2ajnhx4&s=5" target="_blank">Click here</a> to view your image</strong>

        }

        public void Disconnect()
        {
            this._output.ImageCaptured -= new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);

            Trace("disconnect");
            Trace("+--------------------------------");
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            ImageFormatEventArgs args1 = new ImageFormatEventArgs();
            args1.ClickedMenuItem = (MenuItem) sender;
            args1.ImageOutputFormat = this;
            this.ImageFormatClick.Invoke(sender, args1);
        }

        private void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            ImagePairNames names1 = e.ImageNames;
            this._logger = new TinyPicLogWriter(new FileInfo(names1.FullSize).DirectoryName);

            if (this._isThumbEnabled = e.IsThumbnailed)
            {
                this._thumbFileName = e.ImageNames.Thumbnail;
                this._thumbnailImage = e.ThumbnailImage;
            }

            this._fileName = e.ImageNames.FullSize;
            this._output.FetchOutputStream(new StreamHandler(this.SaveImage), this._fileName, e.FullSizeImage);
        }


        private void SaveImage(Stream stream, Image image)
        {
            Trace("SaveImage");
            bool success = false;
            try
            {
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
                UploadImage();
        }


        private void UploadImage()
        {
            Trace("UploadImage");
            _errorMessage = null;
            try
            {
                string imageUrl = string.Empty;
                string response = this.Upload(this._fileName);
                Match match = this._regex.Match(response);

                if (!match.Success)
                {
                    Trace("Did not find response in expected format.");
                    Clipboard.SetDataObject(this._fileName, true);
                    string msg = "Failed to upload to TinyPic.";
                    if (_errorMessage != null)
                        msg += " " + _errorMessage;
                    else
                        msg += " Seems like there was a problem with the Regex." ;
                    MessageBox.Show(msg + "  Upload this file manually: " + this._fileName);
                    return;
                }

                // The page that comes back displays the image, the raw url for the image,
                // and a bunch of ads.  We want the raw image url.
                string urlForCookedImagePage = match.Groups["imgurl"].Value.ToString();
                imageUrl = FindRawUrl(urlForCookedImagePage);

                if (imageUrl==null)
                    imageUrl = urlForCookedImagePage;


                Clipboard.SetDataObject(imageUrl, true);
                if (!this._isThumbEnabled)
                {
                    this._logger.Log(imageUrl);
                }

                // show it in the default browser:
                System.Diagnostics.Process.Start(urlForCookedImagePage);

                if (this._isThumbEnabled)
                {
                    response = this.Upload(this._thumbFileName);
                    match = this._regex.Match(response);
                    if (!match.Success)
                    {
                        Clipboard.SetDataObject(imageUrl + ", " + this._thumbFileName, true);
                        string msg = String.Format("The main image was successfully uploaded and is available at {0}, but the thumbnail didn't go.  You will have to upload this manually: {1}", imageUrl, this._thumbFileName);
                        MessageBox.Show(msg);
                        return;
                    }


                    string text2 = match.Groups["imgurl"].Value.ToString();
                    Clipboard.SetDataObject(imageUrl + ", " + text2, true);
                    this._logger.Log(imageUrl, text2);

                }
            }
            catch (Exception exception2)
            {
                MessageBox.Show("Upload this file manually: " + this._fileName + Environment.NewLine + exception2.Message + Environment.NewLine + exception2.StackTrace);
            }
            return ;
        }



        private string FindRawUrl(string uri)
        {
            string pageMarkup = GetPageMarkup(uri);

            // look for:
            // <label for="direct-url">Direct Link for Layouts</label>
            var regex1 = new Regex("<label for=\"direct-url\">Direct Link for Layouts</label>");
            var m1 = regex1.Match(pageMarkup);
            if (!m1.Success) return null;

            // look for something like this in the output:
            //   fo.addVariable("ipt", "http%3A%2F%2Fi41.tinypic.com%2F24dqxlc.jpg");
            string remainder = pageMarkup.Substring(m1.Groups[0].Index);
            var regex2 = new Regex("addVariable\\(\"ipt\", \"(?<escapedUrl>[^\"]*)\"\\);");
            var m2 = regex2.Match(remainder);
            if (!m2.Success) return null;

            string escapedUrl = m2.Groups["escapedUrl"].Value.ToString();
            string decodedUrl = System.Web.HttpUtility.UrlDecode(escapedUrl);
            Trace("\n\nraw image Url: {0}", decodedUrl);
            return decodedUrl;
        }



        private string Upload(string fileName)
        {
            string pageResponse = this.UploadFileToTinyPic(fileName);

            // debugging
            //             string pluginLogfile = "c:\\desktop\\SendToTinyPic.Log.out.txt";
            //             using (StreamWriter sw = File.AppendText(pluginLogfile))
            //             {
            //                 sw.Write(pageResponse);
            //             }

            return pageResponse;
        }

        private static void AddField(StringBuilder sb1, string key, string value, string divider)
        {
            sb1.Append(divider)
                .Append("Content-Disposition: form-data; name=\"")
                .Append(key)
                .Append("\"\r\n\r\n")
                .Append(value)
                .Append("\r\n");

        }

        private string UploadFileToTinyPic(string fileToUpload)
        {
            UploadParams p = GetParams();

            string divider = "-----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(p.UploadUri);
            webRequest.CookieContainer = PersistentCookies.GetCookieContainerForUrl("http://tinypic.com");
            webRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + divider;
            divider = "--" + divider + "\r\n";

            StringBuilder sb1 = new StringBuilder();
            foreach (var key in p.HiddenFields.Keys)
            {
                AddField(sb1, key, p.HiddenFields[key], divider);
            }

            AddField(sb1, "dimension", "1600", divider);
            AddField(sb1, "description", "cropper", divider);
            AddField(sb1, "file_type", "image", divider);

            sb1.Append(divider)
                .Append("Content-Disposition: form-data; name=\"")
                .Append(p.FileField)
                .Append("\"; filename=\"")
                .Append(Path.GetFileName(fileToUpload))
                .Append("\"\r\n")
                .Append("Content-Type: application/octet-stream\r\n\r\n");

            Stream s = null;
            string resp = null;
            try
            {
                s = webRequest.GetRequestStream();
                byte[] formData = Encoding.UTF8.GetBytes(sb1.ToString());
                s.Write(formData, 0, formData.Length);
                using (FileStream fs = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] buffer = new byte[Math.Min(0x1000, (int)fs.Length)];
                    int n = 0;
                    while ((n = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        s.Write(buffer, 0, n);
                    }
                }

                byte[] finalDivider = Encoding.ASCII.GetBytes("\r\n" + divider);
                s.Write(finalDivider, 0, finalDivider.Length);
                s.Close();
                s = null;

                // get the response
                try
                {
                    WebResponse webResponse = webRequest.GetResponse();
                    if (webResponse == null)
                        return null;
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        resp = sr.ReadToEnd().Trim();
                    }
                }
                catch (Exception ex1)
                {
                    _errorMessage = "HttpPost: Response error: " + ex1.Message;
                }
            }
            catch (Exception ex2)
            {
                _errorMessage = "HttpPost: Request error: " + ex2.Message;
            }
            finally
            {
                if (s!=null)
                    s.Close();
            }

            return resp;
        }




        private class UploadParams
        {
            public string UploadUri;
            public Dictionary<String,String> HiddenFields;
            public string FileField;
        }



        private UploadParams GetParams()
        {
            UploadParams p = new UploadParams();

            string homePageMarkup= GetPageMarkup("http://TinyPic.com");
            var regexUri = new Regex("action=\"(?<form>[^\"]+(upload\\.php))\"");
            var regexFile = new Regex("<input type=\"file\" (.*?)name=\"(?<file>[^\"]+)\"");
            var regexHidden = new Regex("<input type=\"hidden\" name=\"(?<name>[^\"]+)\" (.*?)value=\"(?<value>[^\"]+)\"");

            var mUri = regexUri.Match(homePageMarkup);
            if (!mUri.Success)
                return null;

            Group g = mUri.Groups["form"];
            p.UploadUri = g.Value.ToString();
            p.HiddenFields = new Dictionary<String,String>();
            // only search on what's left:
            Trace("index: {0}", g.Index);
            Trace("length: {0}", g.Length);

            // now look for the name of the file-to-upload field
            string remainder = homePageMarkup.Substring(g.Index);
            var mFile = regexFile.Match(remainder);
            if (mFile.Success)
                p.FileField = mFile.Groups["file"].Value.ToString();

            // now look for hidden fields
            do
            {
                var mHidden = regexHidden.Match(remainder);
                if (!mHidden.Success)
                    break;
                else
                {
                    string key = mHidden.Groups["name"].Value.ToString();
                    string value = mHidden.Groups["value"].Value.ToString();
                    //Console.WriteLine("key({0}) value({1})", key, value);
                    p.HiddenFields.Add(key, value);
                    int index = mHidden.Groups["value"].Index;
                    int length = mHidden.Groups["value"].Length;
                    remainder = remainder.Substring(index + length - 1);
                }
            } while (true);

            return p;
        }

        private static string GetPageMarkup(string uri)
        {
            string pageData = null;
            using (WebClientEx client = new WebClientEx())
            {
                pageData = client.DownloadString(uri);
            }
            return pageData;
        }


        public string Description
        {
            get
            {
                return "Send to TinyPic";
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
                item1.Text = Description;
                item1.Click += new EventHandler(this.menuItem_Click);
                return item1;
            }
        }


        class WebClientEx : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request is HttpWebRequest)
                {
                    (request as HttpWebRequest).CookieContainer = PersistentCookies.GetCookieContainerForUrl(address);
                }
                return request;
            }
        }





        class PersistentCookies
        {
            // to get persistent cookies for TinyPic
            [DllImport("wininet.dll", CharSet=CharSet.Auto , SetLastError=true)]
            private static extern bool InternetGetCookie (string url, string name, StringBuilder data, ref int dataSize);


            private static string RetrieveIECookiesForUrl(string url)
            {
                StringBuilder cookieHeader = new StringBuilder(new String(' ', 256), 256);
                int datasize = cookieHeader.Length;
                if (!InternetGetCookie(url, null, cookieHeader, ref datasize))
                {
                    if (datasize < 0)
                        return String.Empty;
                    cookieHeader = new StringBuilder(datasize); // resize with new datasize
                    InternetGetCookie(url, null, cookieHeader, ref datasize);
                }
                // result is like this: "KEY=Value; KEY2=what ever"
                return cookieHeader.ToString();
            }

            public static CookieContainer GetCookieContainerForUrl(string url)
            {
                return GetCookieContainerForUrl(new Uri(url));
            }

            public static CookieContainer GetCookieContainerForUrl(Uri url)
            {
                CookieContainer container = new CookieContainer();
                string cookieHeaders = RetrieveIECookiesForUrl(url.AbsoluteUri);
                if (cookieHeaders.Length > 0)
                {
                    try
                    {
                        container.SetCookies(url, cookieHeaders);
                    }
                    catch (CookieException) {}
                }
                return container;
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

