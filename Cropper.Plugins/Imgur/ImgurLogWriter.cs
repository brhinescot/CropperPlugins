using System;
using System.IO;
using System.Xml;


namespace Cropper.SendToImgur
{
    public class ImgurLogWriter
    {
        private const string _fileName = "SendToImgurLog.xml";
        private XmlNode _imagesRoot;
        private string _saveFilePath;
        private XmlDocument _xdoc;

        public ImgurLogWriter(string directory)
        {
            this._xdoc = new XmlDocument();
            this._saveFilePath = Path.Combine(directory,_fileName);
            if (File.Exists(this._saveFilePath))
            {
                this._xdoc.Load(this._saveFilePath);
                this._imagesRoot = this._xdoc.DocumentElement;
            }
            else
            {
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
            //System.Diagnostics.Debugger.Break();
            XmlElement nodeImage = this._xdoc.CreateElement("image");
            nodeImage.SetAttribute("url", url);
            if (thumburl != string.Empty)
            {
                nodeImage.SetAttribute("thumburl", thumburl);
            }
            nodeImage.SetAttribute("when", DateTime.Now.ToString("G"));
            this._imagesRoot.AppendChild(nodeImage);
            this._xdoc.Save(this._saveFilePath);
        }
    }
}

