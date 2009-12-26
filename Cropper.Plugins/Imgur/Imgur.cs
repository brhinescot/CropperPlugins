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



namespace Cropper.SendToImgur
{
    public class Imgur : IPersistableImageFormat
    {
        public Imgur ()
        {
            SetupDebugConsole(); // for debugging purposes
            _cookieJar = PersistentCookies.GetCookieContainerForUrl(_baseUri);            
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
            Trace("UploadImage");
            
            _errorMessage = null;
            try
            {
                string imageUrl = string.Empty;
                string responsePageUri = this.UploadFileToImgur(this._fileName);

                string responsePageMarkup= GetPageMarkup(responsePageUri);
                
                Match match = this._regex.Match(responsePageMarkup);
            
                if (!match.Success)
                {
                    Clipboard.SetDataObject(this._fileName, true);
                    string msg = "Failed to upload to Imgur.";
                    if (_errorMessage != null)
                        msg += " " + _errorMessage;
                    else
                        msg += " Seems like there was a problem with the Regex." ;
                    MessageBox.Show(msg + "  Upload this file manually: " + this._fileName);
                    return;
                }

                System.Diagnostics.Process.Start(responsePageUri);
                
                // The page that comes back displays the image, the raw url for the image,
                // and a bunch of ads.  We want the raw image url.
                imageUrl = match.Groups["imgurl"].Value.ToString();

                Trace("Raw img url: {0}", imageUrl);
                
                Clipboard.SetDataObject(imageUrl, true);
                if (!this._isThumbEnabled)
                {
                    this._logger.Log(imageUrl);
                }
                
                if (this._isThumbEnabled)
                {
                    responsePageUri = this.UploadFileToImgur(this._thumbFileName);
                    responsePageMarkup= GetPageMarkup(responsePageUri);

                    match = this._regex.Match(responsePageMarkup);
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
                
                Trace("all done.");
                Trace("---------------------------------");
            }
            catch (Exception exception2)
            {
                MessageBox.Show("Upload this file manually: " + this._fileName + Environment.NewLine + exception2.Message );//+ Environment.NewLine + exception2.StackTrace);
            }
            return ;
        }



        
        /// <summary>
        ///   Returns the URL at which the uploaded image is visible. 
        /// </summary>
        ///
        /// <remarks>
        ///   Do a GET on the returned URL in order to get the page  content.
        /// </remarks>
        ///
        private string UploadFileToImgur(string fileToUpload)
        {
            ImgurUploadParams p = GetParams();

            SendUploadProgressRequest(p);

            //System.Diagnostics.Debugger.Break();
            
            // the divider can be "anything", but by convention it looks like this
            string divider = "-----------------------------" + DateTime.Now.Ticks.ToString("x");

            Trace("WebRequest.Create");

            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(_baseUri + ImgurUploadParams.UploadUri1);
            req.Referer=_baseUri;
            req.UserAgent= _userAgent;
            req.Headers.Add("Pragma","no-cache");
            req.AllowAutoRedirect = false;
            req.CookieContainer = _cookieJar;
            //req.Proxy.Credentials = CredentialCache.DefaultCredentials;
            req.Method = "POST";
            req.ContentType = "multipart/form-data; boundary=" + divider;
            divider = "--" + divider + "\r\n";
            
            StringBuilder sb1 = new StringBuilder();
            foreach (var key in p.Dict.Keys)
            {
                if (key!="action")
                {
                    Trace("{0} = {1}", key, p.Dict[key]);
                    AddField(sb1, key, p.Dict[key], divider);
                }                    
            }

            string contentType;
            if (fileToUpload.EndsWith(".png"))
                contentType = "image/x-png";
            else if (fileToUpload.EndsWith(".jpg"))
                contentType = "image/pjpeg";
            else
                contentType = "application/octet-stream";
                
            sb1.Append(divider)
                .Append("Content-Disposition: form-data; name=\"file[]\"; filename=\"")
                .Append(Path.GetFileName(fileToUpload))
                .Append("\"\r\n")
                .Append("Content-Type: ")
                .Append(contentType)
                .Append("\r\n\r\n");
           
            Stream s = null;

            try
            {
                Trace("GetRequestStream");
                s = req.GetRequestStream();
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

                sb1 = new StringBuilder();
                sb1.Append("\r\n");
                AddField(sb1, "submit", "", divider);
                sb1.Append(divider)
                    .Append("--");
                byte[] finalDivider = Encoding.ASCII.GetBytes(sb1.ToString());
                s.Write(finalDivider, 0, finalDivider.Length);
                s.Close();
                s = null;

                Trace("writing all bytes");

                // get the response
                try
                {
                    Trace("GetResponse");
                    //System.Diagnostics.Debugger.Break();
                    
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    if (resp == null)
                        return null;

                    // expected response is a 302 with a redirect to /processUpload2.php
                    if ((int)(resp.StatusCode) != 302)
                        throw new Exception(String.Format("unexpected status code ({0} ({1}))", resp.StatusCode, (int)resp.StatusCode));
                    
                    Trace("Redirect to: {0}", resp.Headers["Location"]);
                    string redirect = _baseUri + resp.Headers["Location"];

                    /// process cookies 
                    if (resp.Headers["Set-Cookie"] != null)
                        _cookieJar.Add(new Uri(_baseUri), resp.Cookies);
                    
                    resp = SendImgurGetRequest(redirect);
                    
                    // expected response from *that* is a 302 with a redirect to the short image URL
                    if ((int)(resp.StatusCode) != 302)
                        throw new Exception(String.Format("unexpected status code ({0} ({1}))", resp.StatusCode, (int)resp.StatusCode));

                    Trace("Redirect to: {0}", resp.Headers["Location"]);
                    redirect = _baseUri + resp.Headers["Location"];

                    return redirect;
                }
                catch (Exception ex1)
                {
                    Trace("HttpPost: Response error: " + ex1.Message);
                    _errorMessage = "HttpPost: Response error: " + ex1.Message;
                    throw ;
                }
            }
            catch (Exception ex2)
            {
                Trace("HttpPost: Request error: " + ex2.Message);
                _errorMessage = "HttpPost: Request error: " + ex2.Message;
                throw ;
            }
            finally
            { 
                if (s!=null)
                    s.Close();
            }        
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

                Trace("phpTime = {0}", phpTime);
                
                return phpTime;
            }
        }


        
        
        private ImgurUploadParams GetParams()
        {
            ImgurUploadParams p = new ImgurUploadParams();
           
            string homePageMarkup= GetPageMarkup(_baseUri);

            foreach (var key in ImgurUploadParams.Regexi.Keys)
            {
                var r = new Regex(ImgurUploadParams.Regexi[key]);
                var m = r.Match(homePageMarkup);
                if (m.Success)
                {
                    Group g = m.Groups[key];
                    p.Dict.Add(key, g.Value.ToString());
                }
            }
                
            return p;
        }




        private string GetPageMarkup(string uri)
        {
            using (WebClientEx client = new WebClientEx(_cookieJar))
            {
                return client.DownloadString(uri);
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
        /// This class holds HTML FORM parameters for image upload to imgur.com
        /// </summary>
        ///
        /// <remarks>
        ///   The HTTP transaction flow for imgur.com is this:
        ///     1. open http://imgur.com (submitting cookies, if any).  
        ///     2. Receive response containing a FORM, with a generated UPLOAD_IDENTIFIER <input>. '
        ///     3. POST the form to processUpload1.php
        ///     4. Receive a 302 Found (redirect), to processUpload2.php
        ///     5. Submit a GET to processUpload2.php
        ///     6. Receive a 302 Found (redirect), to the actual, new URL
        ///     7. Submit a GET to the new generated URL
        ///     8. Embedded in that page is an <img src="..."> tag
        ///
        ///   What this class does is hold info for steps 1 - 3.  The Form action,
        ///   The UPLOAD_IDENTIFIER, and the /processUpload1.php path. 
        ///
        /// </remarks>
        class ImgurUploadParams
        {
            public Dictionary<String, String> Dict = new Dictionary<String, String>();

            public static readonly String UploadUri1;
            public static readonly Dictionary<String,String> Regexi;

            static ImgurUploadParams()
            {
                UploadUri1 = "processUpload1.php";
                    
                Regexi = new Dictionary<String, String>();

                // <form action="uploadprogress.php" ...
                Regexi.Add("action", "<form\\s+action=\"(?<action>[^\"]+)\"");

                // <input type="hidden" name="MAX_FILE_SIZE" value="10485760" />
                Regexi.Add("MAX_FILE_SIZE", "<input\\s+(.*?)name=\"MAX_FILE_SIZE\"(.*?) value=\"(?<MAX_FILE_SIZE>[^\"]+)\"");
                
                // <input id="sid" name="UPLOAD_IDENTIFIER" type="hidden" value="92c2e1e62c290ca5a296ae670a507804" />
                Regexi.Add("UPLOAD_IDENTIFIER", "<input\\s+(.*?)name=\"UPLOAD_IDENTIFIER\"(.*?) value=\"(?<UPLOAD_IDENTIFIER>[^\"]+)\"");
            }
        }



        /// <summary>
        ///   Imgur.com allows for asynch status inquiry of in-process uploads. 
        /// </summary>
        private void SendUploadProgressRequest(ImgurUploadParams p)
        {
            string url = _baseUri + "uploadprogress.php";
            string format = "{0}?id={1}&_={2}";

            string requestUri= String.Format(format, url, p.Dict["action"], p.Dict["UPLOAD_IDENTIFIER"], PhpTime);

            HttpWebResponse resp = SendImgurGetRequest(requestUri);
        }
        

        
        private HttpWebResponse SendImgurGetRequest(string requestUri)
        {
            try
            {
                Trace("SendImgurGetRequest");
                
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(requestUri);
                req.CookieContainer = _cookieJar;
                req.Method = "GET";
                req.AllowAutoRedirect = false;
                req.UserAgent= _userAgent;
                req.Headers.Add("x-requested-with", "XMLHttpRequest"); 
                req.Headers.Add("Accept-Language", "en-us");
                req.ProtocolVersion=System.Net.HttpVersion.Version11;
                req.Referer= _baseUri;
                req.Accept="application/json, text/javascript, */*";
                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();

                if (resp.Headers["Set-Cookie"] != null)
                    _cookieJar.Add(new Uri(_baseUri), resp.Cookies);
                
                Trace("A-OK (SendImgurGetRequest)");
                return resp;
            }
            catch (WebException)
            {

            }
            return null;
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


        
        private string _errorMessage; 
        private System.Diagnostics.Process _process;  // debugging only
        private static readonly string _userAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; .NET CLR 3.5.30729; Zune 3.0;)";
        private static readonly string _baseUri= "http://imgur.com/";
        private static System.DateTime _unixEpoch = new System.DateTime(1970,1,1, 0,0,0, DateTimeKind.Utc);
        private CookieContainer _cookieJar; 
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



            
    class WebClientEx : WebClient
    {
        private CookieContainer _cookieJar; 
            
        public WebClientEx(CookieContainer cc)
        {
            _cookieJar = cc;
        }
            
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                request.Headers.Add("Pragma","no-cache");
                (request as HttpWebRequest).CookieContainer = _cookieJar;
            }
            return request;
        }
    }




   
    

    class PersistentCookies
    {
        // to get persistent cookies for Imgur
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



}

