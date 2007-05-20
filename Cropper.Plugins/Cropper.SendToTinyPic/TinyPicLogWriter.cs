using System;
using System.IO;
using System.Xml;


namespace Cropper.SendToTinyPic
{
    public class TinyPicLogWriter
    {
        private const string _fileName = "TinyPicLog.xml";
        private XmlNode _imagesRoot;
        private string _saveFilePath;
        private XmlDocument _xdoc;

        public TinyPicLogWriter(string directory)
        {
            this._xdoc = new XmlDocument();
            this._saveFilePath = directory + @"\TinyPicLog.xml";
            FileInfo info1 = new FileInfo(this._saveFilePath);
            if (info1.Exists)
            {
                this._xdoc.Load(this._saveFilePath);
                this._imagesRoot = this._xdoc.DocumentElement;
            }
            else
            {
                string text1 = "type='text/xsl' href='TinyPicLog.xsl'";
                XmlProcessingInstruction instruction1 = this._xdoc.CreateProcessingInstruction("xml-stylesheet", text1);
                this._xdoc.AppendChild(instruction1);
                this._imagesRoot = this._xdoc.CreateElement("images");
                this._xdoc.AppendChild(this._imagesRoot);
            }
        }

        public void Log(string url)
        {
            this.Log(url, string.Empty);
        }

        public void Log(string url, string thumburl)
        {
            XmlElement element1 = this._xdoc.CreateElement("image");
            XmlElement element2 = this._xdoc.CreateElement("url");
            element2.InnerText = url;
            element1.AppendChild(element2);
            if (thumburl != string.Empty)
            {
                XmlElement element3 = this._xdoc.CreateElement("thumburl");
                element3.InnerText = thumburl;
                element1.AppendChild(element3);
            }
            if (this._imagesRoot.FirstChild == null)
            {
                this._imagesRoot.AppendChild(element1);
            }
            else
            {
                this._imagesRoot.InsertBefore(element1, this._imagesRoot.FirstChild);
            }
            this._xdoc.Save(this._saveFilePath);
        }
    }
}

