using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Cropper.SendToS3
{
    public class S3Settings : IXmlSerializable
    {
        public string AccessKeyId;
        public string SecretAccessKey;
        public string BucketName;

        public void Save()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cropper Plugins\\SendToS3");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string settingsFilepath = Path.Combine(path, "settings.xml");
            XmlWriter writer = XmlWriter.Create(settingsFilepath);

            WriteXml(writer);

            writer.Close();
        }

        public static S3Settings Load()
        {
            S3Settings settings = new S3Settings();
            string settingsFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cropper Plugins\\SendToS3\\settings.xml");
            if (File.Exists(settingsFilepath))
            {
                XmlReader reader = XmlReader.Create(settingsFilepath);
                settings.ReadXml(reader);
                reader.Close();
            }

            return settings;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();
            AccessKeyId = reader.ReadElementString("AccessKeyId");
            SecretAccessKey = reader.ReadElementString("SecretAccessKey");
            BucketName = reader.ReadElementString("BucketName");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("SendToS3Settings");

            writer.WriteElementString("AccessKeyId", AccessKeyId);
            writer.WriteElementString("SecretAccessKey", SecretAccessKey);
            writer.WriteElementString("BucketName", BucketName);

            writer.WriteEndElement();
        }

        #endregion
    }
}
