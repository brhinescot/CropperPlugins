using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;


namespace Cropper.SendToS3
{
    [XmlType(Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Owner
    {
        [XmlElement("ID")]
        public String Id          { get; set; }
        public string DisplayName { get; set; }
    }

    [XmlType(Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class Bucket
    {
        public string Name           { get; set; }
        public DateTime CreationDate { get; set; }
    }

    [XmlType(Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    [XmlRoot(Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class ListAllMyBucketsResult
    {
        public Owner Owner          { get; set; }
        public List<Bucket> Buckets { get; set; }
    }

    [XmlType("Contents", Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class BucketEntry
    {
        public string Key            { get; set; }
        public DateTime LastModified { get; set; }
        public string ETag           { get; set; }
        public long Size             { get; set; }
        public string StorageClass   { get; set; }
        public Owner Owner           { get; set; }
    }

    [XmlType(Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    [XmlRoot(Namespace="http://s3.amazonaws.com/doc/2006-03-01/")]
    public class ListBucketResult
    {
        public string Name                      { get; set; }
        public string Prefix                    { get; set; }
        public string Marker                    { get; set; }
        public string Delimiter                 { get; set; }
        public int MaxKeys                      { get; set; }
        public bool IsTruncated                 { get; set; }
        public string NextMarker                { get; set; }
        [XmlElement("Contents")]
        public List<BucketEntry> Entries        { get; set; }
        public List<Object> CommonPrefixEntries { get; set; }
    }
}
