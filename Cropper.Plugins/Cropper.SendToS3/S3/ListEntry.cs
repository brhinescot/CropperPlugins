using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace Cropper.SendToS3.S3
{
    public class ListEntry
    {
        private string key;
        public string Key {
            get {
                return key;
            }
        }

        private DateTime lastModified;
        public DateTime LastModified {
            get {
                return lastModified;
            }
        }

        private string etag;
        public string ETag {
            get {
                return etag;
            }
        }

        private long size;
        public long Size {
            get {
                return size;
            }
        }

        private string storageClass;
        public string StorageClass {
            get {
                return storageClass;
            }
        }

        private Owner owner;
        public Owner Owner {
            get {
                return owner;
            }
        }

        public ListEntry( string key,
                          DateTime lastModified,
                          string etag,
                          long size,
                          string storageClass,
                          Owner owner)
        {
            this.key = key;
            this.lastModified = lastModified;
            this.etag = etag;
            this.size = size;
            this.storageClass = storageClass;
            this.owner = owner;
        }

        public ListEntry(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name.Equals("Key"))
                {
                    key = Utils.getXmlChildText(child);
                }
                else if (child.Name.Equals("LastModified"))
                {
                    string value = Utils.getXmlChildText(child);
                    lastModified = Utils.parseDate(value);
                }
                else if ( child.Name.Equals("ETag" ) ) {
                    etag = Utils.getXmlChildText(child);
                }
                else if ( child.Name.Equals("Size" ) )
                {
                    size = long.Parse( Utils.getXmlChildText( child ) );
                }
                else if ( child.Name.Equals( "Owner" ) )
                {
                    owner = new Owner( child );
                }
                else if ( child.Name.Equals( "StorageClass" ) )
                {
                    storageClass = Utils.getXmlChildText( child );
                }
            }
        }
    }
}
