//#define Trace

using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Diagnostics;         // for Conditional
using System.Threading;           // for Thread.Sleep
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;      // GZipStream, DeflateStream
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Fusion8.Cropper.Extensibility;
using System.Collections.Generic;



namespace Cropper.SendToImgur
{
    public class Imgur : IPersistableImageFormat
    {
        public Imgur ()
        {
            Tracing.SetupDebugConsole(); // for debugging purposes
        }

        public event ImageFormatClickEventHandler ImageFormatClick;

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
                throw new ArgumentNullException("persistableOutput");

            this._output = persistableOutput;
            this._output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
            // <input type="text" id="htmlimage" value='&lt;img src="http://imgur.com/kuER2.jpg" alt="Hosted by imgur.com" /&gt;' readonly="readonly" />

            this._regex = new Regex("<input type=\"text\" id=\"htmlimage\" value='.+?src=\"(?<imgurl>[^\"]+)\"[^']+' readonly",
                                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }


        public void Disconnect()
        {
            this._output.ImageCaptured -= new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
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
            this._logger = new ImgurLogWriter(new FileInfo(names1.FullSize).DirectoryName);

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
                Tracing.Trace("+--------------------------------");
                Tracing.Trace("SaveImage ({0})", _fileName);
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
                //if (DateTime.UtcNow - _mostRecentUpload > _timeDelta)
                {
                    UploadImage();
                    _mostRecentUpload= DateTime.UtcNow;
                }
                //                 else
                //                 {
                //                     int moreToWait = (int)((_timeDelta - (DateTime.UtcNow - _mostRecentUpload)).TotalSeconds);
                //                     string msg = String.Format("It's too soon to upload another image! Figuring a 6 minute delay, you'll have to wait {0}s", moreToWait);
                //                     MessageBox.Show(msg);
                //                 }
            }
        }



        /// <summary>
        ///  This does the real work of the plugin - uploading to imgur.com.
        /// </summary>
        ///
        /// <remarks>
        ///   It's done in 2 stages: first upload the main image, and place the raw
        ///   image URL onto the clipboard for easy reference.  Then, optionally, upload
        ///   the thumbnail image.  This method also pops the uploaded image page in the
        ///   default browser.
        /// </remarks>
        private void UploadImage()
        {
            Tracing.Trace("UploadImage");

            _errorMessage = null;
            try
            {
                string imageUrl = string.Empty;
                string responsePageUri = this.DoUpload();
                if (String.IsNullOrEmpty(responsePageUri))
                {
                    Clipboard.SetDataObject(this._fileName, true);
                    string msg = "Whoops!";
                    if (_errorMessage != null)
                        msg += " " + _errorMessage;
                    else
                        msg += " There was a problem with the Upload." ;
                    MessageBox.Show(msg + Environment.NewLine + "  Upload this file manually: " + this._fileName);
                    return;
                }

                System.Diagnostics.Process.Start(responsePageUri);

                // The page that comes back displays the image, the raw url for the image,
                // and a bunch of ads.  We want the raw image url.

                imageUrl = responsePageUri + Path.GetExtension(this._fileName);
                Clipboard.SetDataObject(imageUrl, true);
                this._logger.Log(imageUrl);


                //                 if (!this._isThumbEnabled)
                //                 {
                //                     this._logger.Log(imageUrl);
                //                 }
                //                 if (this._isThumbEnabled)
                //                 {
                //                     responsePageUri = this.UploadFileToImgur(this._thumbFileName);
                //                     responsePageMarkup= GetPageMarkup(responsePageUri);
                //
                //                     match = this._regex.Match(responsePageMarkup);
                //                     if (!match.Success)
                //                     {
                //                         Clipboard.SetDataObject(imageUrl + ", " + this._thumbFileName, true);
                //                         string msg = String.Format("The main image was successfully uploaded and is available at {0}, but the thumbnail didn't go.  You will have to upload this manually: {1}", imageUrl, this._thumbFileName);
                //                         MessageBox.Show(msg);
                //                         return;
                //                     }
                //
                //                     string text2 = match.Groups["imgurl"].Value.ToString();
                //                     Clipboard.SetDataObject(imageUrl + ", " + text2, true);
                //                     this._logger.Log(imageUrl, text2);
                //                 }

                Tracing.Trace("all done.");
                Tracing.Trace("---------------------------------");
            }
            catch (Exception exception2)
            {
                MessageBox.Show("Upload this file manually: " + this._fileName + Environment.NewLine + exception2.Message );//+ Environment.NewLine + exception2.StackTrace);
            }
            return ;
        }





        private string DoUpload()
        {
            var rnd = new System.Random();
            HttpWebRequest hwr;
            HttpWebResponse resp;

            Tracing.SetupDebugConsole();   // For Debugging only

            Tracing.Trace("hello");

            // Get the initial page, and any cookies the page wants us to
            // retain.
            hwr = WebRequestFactory.Create(0, _baseUri);
            resp = (HttpWebResponse) hwr.GetResponse();
            Tracing.Trace("Initial status = " + resp.StatusCode.ToString());
            if ((int)(resp.StatusCode) != 200)
                return null;

            string frontPage = GetReplyString(resp);

            if (resp.Cookies != null)
                hwr.CookieContainer.Add(new Uri(_baseUri),
                                        new Cookie("IMGURSESSION", resp.Cookies["IMGURSESSION"].Value));

            // find sid here
            string sid = GetSid(frontPage);

            if (String.IsNullOrEmpty(sid))
            {
                Tracing.Trace("Null or empty sid");
                return null;
            }
            Tracing.Trace("sid = {0}", sid);

            Thread.Sleep(130 + rnd.Next(450)); // in ms

            // get the swf page
            hwr = WebRequestFactory.Create(1,"http://imgur.com/include/flash/swfupload.swf?preventswfcaching=" + PhpTime.ToString());
            resp = (HttpWebResponse) hwr.GetResponse();
            Tracing.Trace("get swf status = " + resp.StatusCode.ToString());
            if ((int)(resp.StatusCode) != 200)
                return null;

            byte[] swf = GetReplyBytes(hwr);

            // do the check thing
            hwr = WebRequestFactory.Create(2, "http://imgur.com/include/checkCaptcha.php");
            Tracing.Trace("checkCaptcha status = " + resp.StatusCode.ToString());
            var s = hwr.GetRequestStream();
            byte[] data = Encoding.UTF8.GetBytes("files=1");
            s.Write(data, 0, data.Length);
            s.Close();
            string captchaCheck = GetReplyString(hwr);

            // finally, upload the file data
            return UploadFileToImgur(sid);
        }


        private string UploadFileToImgur(string sid)
        {
            string fileToUpload = this._fileName;
            HttpWebRequest hwr = WebRequestFactory.Create(3, "http://imgur.com/processFlash.php");
            string divider = "-------------------" + DateTime.Now.Ticks.ToString("x");
            hwr.ContentType = "multipart/form-data; boundary=" + divider;
            divider = "--" + divider + "\r\n";

            StringBuilder sb1 = new StringBuilder();
            AddField(sb1, "Filename", Path.GetFileName(fileToUpload), divider);
            AddField(sb1, "createAlbum", "0", divider);
            AddField(sb1, "sid", sid, divider);
            AddField(sb1, "albumTitle", "Optional Album Title", divider);
            AddField(sb1, "edit", "0", divider);
            sb1.Append(divider)
                .Append("Content-Disposition: form-data; name=\"Filedata\"; filename=\"")
                .Append(Path.GetFileName(fileToUpload))
                .Append("\"\r\n")
                .Append("Content-Type: application/octet-stream")
                .Append("\r\n\r\n");

            Stream s = null;

            try
            {
                s = hwr.GetRequestStream();
                byte[] formData = Encoding.UTF8.GetBytes(sb1.ToString());
                s.Write(formData, 0, formData.Length);

                // write the file data
                using (FileStream fs = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] buffer = new byte[Math.Min(0x1000, (int)fs.Length)];
                    int n = 0;
                    while ((n = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        s.Write(buffer, 0, n);
                    }
                }

                sb1 = new StringBuilder();
                sb1.Append("\r\n");
                AddField(sb1, "Upload", "Submit Query", divider);
                sb1.Append(divider)
                    .Append("--");
                byte[] finalDivider = Encoding.ASCII.GetBytes(sb1.ToString());
                s.Write(finalDivider, 0, finalDivider.Length);
                s.Close();
                s = null;

                // get the response
                try
                {
                    HttpWebResponse resp = (HttpWebResponse) hwr.GetResponse();
                    if (resp == null)
                        return null;

                    // expected response is a 200
                    if ((int)(resp.StatusCode) != 200)
                        throw new Exception(String.Format("unexpected status code ({0})", resp.StatusCode));

                    string reply = GetReplyString(resp);

                    string donePage = (_baseUri + reply).Replace(".com//", ".com/");
                    Tracing.Trace("Result: " + donePage);
                    return donePage;
                }
                catch (Exception ex1)
                {
                    Tracing.Trace("HttpPost: Response error: " + ex1.Message);
                    _errorMessage = "HttpPost: Response error: " + ex1.Message;
                }
            }
            catch (Exception ex2)
            {
                Tracing.Trace("HttpPost: Request error: " + ex2.Message);
                _errorMessage = "HttpPost: Request error: " + ex2.Message;
            }
            finally
            {
                if (s!=null)
                    s.Close();
            }

            return null;
        }


        private string GetReplyString(HttpWebRequest hwr)
        {
            HttpWebResponse resp = (HttpWebResponse) hwr.GetResponse();
            return GetReplyString(resp);
        }

        private string GetReplyString(HttpWebResponse resp)
        {
            Stream s = resp.GetResponseStream();
            if (resp.ContentEncoding.ToLower().Contains("gzip"))
                s = new GZipStream(s, CompressionMode.Decompress);
            else if (resp.ContentEncoding.ToLower().Contains("deflate"))
                s = new DeflateStream(s, CompressionMode.Decompress);

            string cs = String.IsNullOrEmpty(resp.CharacterSet) ? "UTF-8" : resp.CharacterSet;
            Encoding e = Encoding.GetEncoding(cs);
            using (StreamReader sr = new StreamReader(s, e))
            {
                string r = sr.ReadToEnd();
                if (r.Length > 80)
                    Tracing.Trace("Reply: {0}...", r.Substring(0,78));
                else
                    Tracing.Trace("Reply: {0}", r);
                return r;
            }
        }


        private byte[] GetReplyBytes(HttpWebRequest hwr)
        {
            HttpWebResponse resp = (HttpWebResponse) hwr.GetResponse();
            Stream s = resp.GetResponseStream();
            if (resp.ContentEncoding.ToLower().Contains("gzip"))
                s = new GZipStream(s, CompressionMode.Decompress);
            else if (resp.ContentEncoding.ToLower().Contains("deflate"))
                s = new DeflateStream(s, CompressionMode.Decompress);

            byte[] buffer = new byte[1024];
            int n = 0;
            var ms = new MemoryStream();
            while ((n = s.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, n);
            }
            return ms.GetBuffer();
        }


        /// <summary>
        ///   Adds a single field to a form POST, appending it to the given StringBuffer.
        /// </summary>
        private static void AddField(StringBuilder sb1, string key, string value, string divider)
        {
            sb1.Append(divider)
                .Append("Content-Disposition: form-data; name=\"")
                .Append(key)
                .Append("\"\r\n\r\n")
                .Append(value)
                .Append("\r\n");

        }

        private string GetSid(string html)
        {
            const string regex =  @"<input id=""sid"" name=""UPLOAD_IDENTIFIER"" type=""hidden"" value=""([^""]+)"" />";
            var r = new Regex(regex);
            var m = r.Match(html);
            if (m.Success)
            {
                Group g = m.Groups[1];
                return g.Value.ToString();
            }
            return null;
        }

        /// <summary>
        ///   The image submission requires a timestamp on the form.
        ///   This produces the appropriate timestamp value.
        /// </summary>
        private Int64 PhpTime
        {
            get
            {
                System.TimeZone tz = System.TimeZone.CurrentTimeZone;
                System.DateTime now = System.DateTime.Now;
                System.TimeSpan ts = tz.GetUtcOffset(now);
                System.DateTime utc = System.DateTime.Now - ts;
                System.TimeSpan delta =  utc - _unixEpoch;
                Int64 phpTime = (System.Int64)(delta.TotalSeconds * 1000);
                Tracing.Trace("phpTime = {0}", phpTime);
                return phpTime;
            }
        }



        public string Description
        {
            get
            {
                return "Send to Imgur";
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



        /// <summary>
        ///   Like Webclient, but attaches a cookie container to each request
        /// </summary>
        public class WebRequestFactory
        {
            private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729; Zune 3.0;)";
            private static readonly string DefaultAccept =
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            private static readonly string _baseUri = "http://imgur.com/";

            public static HttpWebRequest Create (int flavor, string uri)
            {
                HttpWebRequest hwr = (HttpWebRequest) WebRequest.Create(uri);
                Condition(hwr, uri);

                switch(flavor)
                {
                    case 0:
                        hwr.Accept = DefaultAccept;
                        hwr.Method = "GET";
                        hwr.UserAgent = WebRequestFactory.DefaultUserAgent;
                        hwr.Headers.Add(HttpRequestHeader.KeepAlive, "300");
                        break;

                    case 1:
                        hwr.UserAgent = WebRequestFactory.DefaultUserAgent;
                        hwr.Accept = DefaultAccept;
                        hwr.Method = "GET";
                        hwr.Headers.Add(HttpRequestHeader.KeepAlive, "300");
                        hwr.Referer = _baseUri;
                        break;

                    case 2:
                        hwr.Accept = "application/json, text/javascript, */*";
                        hwr.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        hwr.Headers.Add(HttpRequestHeader.Pragma,"no-cache");
                        hwr.Headers.Add(HttpRequestHeader.CacheControl,"no-cache");
                        hwr.UserAgent = WebRequestFactory.DefaultUserAgent;
                        hwr.Headers.Add(HttpRequestHeader.KeepAlive, "300");
                        hwr.Method = "POST";
                        hwr.Referer = _baseUri;
                        hwr.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                        break;

                    case 3:
                        hwr.Method = "POST";
                        hwr.Accept = "text/*";
                        hwr.UserAgent = "Shockwave Flash";
                        hwr.Headers.Add(HttpRequestHeader.Pragma,"no-cache");
                        break;

                    default:
                        break;
                }

                return hwr;
            }

            private static void Condition(HttpWebRequest hwr, string address)
            {
                hwr.CookieContainer =
                    PersistentCookies.GetCookieContainerForUrl(_baseUri);
                hwr.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us,en;q=0.5");
                hwr.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                hwr.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                hwr.KeepAlive = true;
            }
        }



        private string _errorMessage;
        private static readonly string _baseUri= "http://imgur.com/";
        private static System.DateTime _unixEpoch = new System.DateTime(1970,1,1, 0,0,0, DateTimeKind.Utc);
        private string _fileName;
        private bool _isThumbEnabled;
        private ImgurLogWriter _logger;
        private IPersistableOutput _output;
        private DateTime _mostRecentUpload = new System.DateTime(1970,1,1, 0,0,0, DateTimeKind.Utc);
        private TimeSpan _timeDelta = new TimeSpan(0,6,0); // six mins
        private Regex _regex;
        private string _thumbFileName;
        private Image _thumbnailImage;

    }




    internal static class Tracing
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        [Conditional("Trace")]
        public static void SetupDebugConsole()
        {
            if ( !AttachConsole(-1) )  // Attach to a parent process console
                AllocConsole(); // Alloc a new console

            _process= System.Diagnostics.Process.GetCurrentProcess();
            System.Console.WriteLine();
        }

        [Conditional("Trace")]
        public static void Trace(string format, params object[] args)
        {
            System.Console.Write("{0:D5} ", _process.Id);
            System.Console.WriteLine(format, args);
        }

        private static System.Diagnostics.Process _process;
    }




    public static class PersistentCookies
    {
        // To get persistent cookies for any website.
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

        private static Dictionary<String, CookieContainer> _cache = new Dictionary<String, CookieContainer>();

        public static CookieContainer GetCookieContainerForUrl(string url)
        {
            if (_cache.ContainsKey(url))
            {
                Tracing.Trace("CC[{0}]= {1:X8} (cached)", url, _cache[url].GetHashCode());
                return _cache[url];
            }
            var x = GetCookieContainerForUrl(new Uri(url));
            _cache[url] = x;
            Tracing.Trace("CC[{0}]= {1:X8} (new)", url, x.GetHashCode());
            return x;
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


}

