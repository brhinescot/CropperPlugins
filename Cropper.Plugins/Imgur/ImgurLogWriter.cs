using System;
using System.IO;
using System.Xml;


namespace Cropper.SendToImgur
{
    public class ImgurLogWriter
    {
        private const string _fileName = "ImgurLog.xml";
        private XmlNode _imagesRoot;
        private string _saveFilePath;
        private XmlDocument _xdoc;

        public ImgurLogWriter(string directory)
        {
            this._xdoc = new XmlDocument();
            this._saveFilePath = Path.Combine(directory,_fileName);
            FileInfo info1 = new FileInfo(this._saveFilePath);
            if (info1.Exists)
            {
                this._xdoc.Load(this._saveFilePath);
                this._imagesRoot = this._xdoc.DocumentElement;
            }
            else
            {
                string text1 = "type='text/xsl' href='ImgurLog.xsl'";
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

            // add a timestamp
            XmlAttribute attr1 = this._xdoc.CreateAttribute("dt");
            attr1.InnerText = DateTime.Now.ToString("G");
            element1.AppendChild(attr1);

            // entries appear in the file, in reverse order
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

