using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Fusion8.Cropper.Extensibility;

namespace Cropper.SendToTinyPic
{
    public class TinyPic : IPersistableImageFormat
    {
        private string _fileName;
        private bool _isThumbEnabled;
        private TinyPicLogWriter _logger;
        private NameValueCollection _nv;
        private IPersistableOutput _output;
        private Regex _regex;
        private string _thumbFileName;
        private Image _thumbnailImage;
        private const string DESCRIPTION = "TinyPic Hosted Image";
        private const string EXTENSION = "png";

        public event ImageFormatClickEventHandler ImageFormatClick;

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }

            this._output = persistableOutput;
            this._output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
            this._regex = new Regex("name=\"myurl\" value=\"((.|\n)*?)\"", RegexOptions.IgnoreCase);
            this._nv = new NameValueCollection();
            this._nv.Add("action", "upload");
            this._nv.Add("addtype", "");
            this._nv.Add("submit", "Host Picture");
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
            try
            {
                image.Save(stream, ImageFormat.Png);
                image.Dispose();
                stream.Close();

                if (this._isThumbEnabled)
                {
                    FileStream stream1 = new FileStream(this._thumbFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                    this._thumbnailImage.Save(stream1, ImageFormat.Png);
                    this._thumbnailImage.Dispose();
                    stream1.Close();
                }
            }
            catch (Exception exception1)
            {
                MessageBox.Show("Upload this file manually: " + this._fileName + Environment.NewLine + exception1.Message + Environment.NewLine + exception1.StackTrace);
                return;
            }

            try
            {
                string text1 = string.Empty;
                Match match1 = this.Upload(this._fileName);

                if (match1.Success)
                {
                    text1 = match1.Value.Replace("name=\"myurl\" value=\"", string.Empty).Replace("\"", string.Empty);
                    Clipboard.SetDataObject(text1, true);
                    if (!this._isThumbEnabled)
                    {
                        this._logger.Log(text1);
                    }
                }
                else
                {
                    Clipboard.SetDataObject(this._fileName, true);
                    MessageBox.Show("Regex failed to match on proper result.  Upload this file manually: " + this._fileName);
                    return;
                }

                if (this._isThumbEnabled)
                {
                    Match match2 = this.Upload(this._thumbFileName);
                    if (match2.Success)
                    {
                        string text2 = match2.Value.Replace("name=\"myurl\" value=\"", string.Empty).Replace("\"", string.Empty);
                        Clipboard.SetDataObject(text1 + ", " + text2, true);
                        this._logger.Log(text1, text2);
                    }
                    else
                    {
                        Clipboard.SetDataObject(this._thumbFileName, true);
                        MessageBox.Show("Regex failed to match on proper result.  Upload this file manually: " + this._fileName);
                    }
                }
            }
            catch (Exception exception2)
            {
                MessageBox.Show("Upload this file manually: " + this._fileName + Environment.NewLine + exception2.Message + Environment.NewLine + exception2.StackTrace);
                return;
            }
        }

        private Match Upload(string fileName)
        {
            string text1 = this.UploadFileEx(fileName, "http://tinypic.com", "the_file", "", this._nv, null);
            return this._regex.Match(text1);
        }

        public string UploadFileEx(string uploadfile, string url, string fileFormName, string contenttype, NameValueCollection querystring, CookieContainer cookies)
        {
            if ((fileFormName == null) || (fileFormName.Length == 0))
            {
                fileFormName = "file";
            }

            if ((contenttype == null) || (contenttype.Length == 0))
            {
                contenttype = "application/octet-stream";
            }

            string text1 = "?";
            if (querystring != null)
            {
                foreach (string text2 in querystring.Keys)
                {
                    string text6 = text1;
                    text1 = text6 + text2 + "=" + querystring.Get(text2) + "&";
                }
            }

            Uri uri1 = new Uri(url + text1);
            string text3 = "----------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request1 = (HttpWebRequest) WebRequest.Create(uri1);
            request1.CookieContainer = cookies;
            request1.ContentType = "multipart/form-data; boundary=" + text3;
            request1.Method = "POST";
            request1.Proxy.Credentials = CredentialCache.DefaultCredentials;

            StringBuilder builder1 = new StringBuilder();
            builder1.Append("--");
            builder1.Append(text3);
            builder1.Append("\r\n");
            builder1.Append("Content-Disposition: form-data; name=\"");
            builder1.Append(fileFormName);
            builder1.Append("\"; filename=\"");
            builder1.Append(Path.GetFileName(uploadfile));
            builder1.Append("\"");
            builder1.Append("\r\n");
            builder1.Append("Content-Type: ");
            builder1.Append(contenttype);
            builder1.Append("\r\n");
            builder1.Append("\r\n");
            string text4 = builder1.ToString();

            byte[] buffer1 = Encoding.UTF8.GetBytes(text4);
            byte[] buffer2 = Encoding.ASCII.GetBytes("\r\n--" + text3 + "\r\n");
            FileStream stream1 = new FileStream(uploadfile, FileMode.Open, FileAccess.Read, FileShare.Read);
            long num1 = (buffer1.Length + stream1.Length) + buffer2.Length;
            request1.ContentLength = num1;
            Stream stream2 = request1.GetRequestStream();
            stream2.Write(buffer1, 0, buffer1.Length);
            byte[] buffer3 = new byte[Math.Min(0x1000, (int) stream1.Length)];

            int num2 = 0;
            while ((num2 = stream1.Read(buffer3, 0, buffer3.Length)) != 0)
            {
                stream2.Write(buffer3, 0, num2);
            }

            stream2.Write(buffer2, 0, buffer2.Length);
            Stream stream3 = request1.GetResponse().GetResponseStream();
            StreamReader reader1 = new StreamReader(stream3);
            string text5 = reader1.ReadToEnd();
            reader1.Close();
            stream2.Close();
            return text5;
        }


        public string Description
        {
            get
            {
                return "TinyPic Hosted Image";
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
                item1.Text = "TinyPic Hosted Image";
                item1.Click += new EventHandler(this.menuItem_Click);
                return item1;
            }
        }
    }
}

