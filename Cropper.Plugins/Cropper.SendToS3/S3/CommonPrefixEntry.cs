using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace Cropper.SendToS3.S3
{
    public class CommonPrefixEntry
    {
        /// <summary>
        /// The prefix common to the delimited keys it represents.
        /// </summary>
        private string prefix;
        public string Prefix
        {
            set
            {
                this.prefix = value;
            }
            get
            {
                return prefix;
            }
        }

        public CommonPrefixEntry(string prefix)
        {
            this.prefix = prefix;
        }

        public CommonPrefixEntry(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name.Equals("Prefix"))
                {
                    prefix = Utils.getXmlChildText(child);
                }
            }
        }
    }
}
