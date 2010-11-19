using System;
using System.IO;
using System.Xml;

namespace Cropper.SendToFlickr
{
    public class Settings
    {
        private FileInfo _fi;
        private XmlElement _settingsElement;
        private string _settingsPath;
        private XmlDocument _xdoc;
        public const string APIKEY = "ab782e182b4eb406d285211811d625ff";
        public const string APISHAREDSECRET = "b080496c05335c3d";
        public const string CONFIGPATH = @"\Cropper.SendToFlickr Plugin\Cropper.SendToFlickr.config";

        public Settings(string settingsPath)
        {
            this._settingsPath = settingsPath;
            this._fi = new FileInfo(this._settingsPath);
            this.LoadSettings();
        }

        private void LoadSettings()
        {
            this._xdoc = new XmlDocument();
            if (this._fi.Exists)
            {
                this._xdoc.Load(this._fi.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
            }
            else
            {
                if (!this._fi.Directory.Exists)
                {
                    this._fi.Directory.Create();
                }
                this._settingsElement = this._xdoc.CreateElement("settings");
                this._xdoc.AppendChild(this._settingsElement);
                XmlElement element1 = this._xdoc.CreateElement("token");
                element1.InnerText = "";
                XmlElement element2 = this._xdoc.CreateElement("tags");
                element2.InnerText = "";
                XmlElement element3 = this._xdoc.CreateElement("photoset");
                element3.InnerText = "";
                this._settingsElement.AppendChild(element1);
                this._settingsElement.AppendChild(element2);
                this._settingsElement.AppendChild(element3);
                this.Save();
            }
        }

        private void Save()
        {
            this._xdoc.Save(this._fi.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite));
        }


        public string PhotoSet
        {
            get
            {
                return this._xdoc["settings"]["photoset"].InnerText;
            }
            set
            {
                this._xdoc["settings"]["photoset"].InnerText = value;
                this.Save();
            }
        }

        public string Tags
        {
            get
            {
                return this._xdoc["settings"]["tags"].InnerText;
            }
            set
            {
                this._xdoc["settings"]["tags"].InnerText = value;
                this.Save();
            }
        }

        public string Token
        {
            get
            {
                return this._xdoc["settings"]["token"].InnerText;
            }
            set
            {
                this._xdoc["settings"]["token"].InnerText = value;
                this.Save();
            }
        }
    }
}

