// This software code is made available "AS IS" without warranties of any        
// kind.  You may copy, display, modify and redistribute the software            
// code either by itself or as incorporated into your code; provided that        
// you do not remove any proprietary notices.  Your use of this software         
// code is at your own risk and you waive any claim against Amazon               
// Digital Services, Inc. or its affiliates with respect to your use of          
// this software code. (c) 2006 Amazon Digital Services, Inc. or its             
// affiliates.          

using System;
using System.Collections;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Cropper.SendToS3.com.amazonaws.s3
{
    /// An interface into the S3 system.  It is initially configured with
    /// authentication and connection parameters and exposes methods to access and
    /// manipulate S3 data.
    public class AWSAuthConnection
    {
        private static readonly string SERVICE_NAME = "AmazonS3";

        private AmazonS3 s3 = new AmazonS3();
        private string awsAccessKeyId;
        private string awsSecretAccessKey;

        public AWSAuthConnection(string awsAccessKeyId, string awsSecretAccessKey)
        {
            this.awsAccessKeyId = awsAccessKeyId;
            this.awsSecretAccessKey = awsSecretAccessKey;
        }

        public AWSAuthConnection(string awsAccessKeyId, string awsSecretAccessKey,
                                  string server, int port)
        {
            this.awsAccessKeyId = awsAccessKeyId;
            this.awsSecretAccessKey = awsSecretAccessKey;
            StringBuilder url = new StringBuilder();
            url.Append("https://").Append(server).Append(":").Append(port).Append("/soap/");
            s3.Url = url.ToString();
        }

        /// <summary>
        /// Creates a new bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to create</param>
        /// <param name="accessControlList">Access Control List (can be null)</param>
        public CreateBucketResult createBucket(string bucket, Grant[] accessControlList)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("CreateBucket", timestamp);
            return s3.CreateBucket(bucket, accessControlList, awsAccessKeyId, timestamp, true, signature);
        }

        /// <summary>
        /// Lists the contents of a bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to list</param>
        /// <param name="prefix">All returned keys will start with this string (can be null)</param>
        /// <param name="marker">All returned keys will be lexographically greater than this string (can be null)</param>
        /// <param name="maxKeys">The maximum number of keys to return (can be 0)</param>
        public ListBucketResult listBucket(string bucket, string prefix, string marker, int maxKeys)
        {
            return listBucket(bucket, prefix, marker, maxKeys, null);
        }

        /// <summary>
        /// Lists the contents of a bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to list</param>
        /// <param name="prefix">All returned keys will start with this string (can be null)</param>
        /// <param name="marker">All returned keys will be lexographically greater than this string (can be null)</param>
        /// <param name="maxKeys">The maximum number of keys to return (can be 0)</param>
        /// <param name="delimiter">If specified, the keys between the prefix and the first occurrence of the
        /// delimiter are rolled into a single result element in the CommonPrefixes property of the returned value
        /// (can be null)</param>
        public ListBucketResult listBucket(string bucket, string prefix, string marker, int maxKeys, string delimiter)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("ListBucket", timestamp);
            return s3.ListBucket(bucket, prefix, marker, maxKeys, maxKeys != 0, delimiter, awsAccessKeyId, timestamp, true, signature, null);
        }

        /// <summary>
        /// Deletes an empty Bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to delete</param>
        public Status deleteBucket(string bucket)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("DeleteBucket", timestamp);
            return s3.DeleteBucket(bucket, awsAccessKeyId, timestamp, true, signature, null);
        }

        /// <summary>
        /// Writes an object to S3.
        /// </summary>
        /// <param name="bucket">The name of the bucket to which the object will be added.</param>
        /// <param name="key">The name of the key to use</param>
        /// <param name="obj">Object to write</param>
        /// <param name="accessControlList">Access Control List (can be null)</param>
        /// <param name="metadata">Metadata (can be null)</param>
        public PutObjectResult put(string bucket, string key, string obj, MetadataEntry[] metadata, Grant[] accessControlList)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("PutObjectInline", timestamp);
            ASCIIEncoding ae = new ASCIIEncoding();
            return s3.PutObjectInline(bucket, key, metadata, ae.GetBytes(obj), obj.Length, accessControlList,
                                      StorageClass.STANDARD, false,
                                      awsAccessKeyId, timestamp, true, signature, null);
        }

        /// <summary>
        /// Reads an object from S3
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives</param>
        /// <param name="key">The name of the key to use</param>
        public GetObjectResult get(string bucket, string key)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("GetObject", timestamp);
            return s3.GetObject(bucket, key, false, true, true, awsAccessKeyId, timestamp, true, signature, null);
        }

        /// <summary>
        /// Delete an object from S3.
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives.</param>
        /// <param name="key">The name of the key to use.</param>
        public Status delete(string bucket, string key)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("DeleteObject", timestamp);
            return s3.DeleteObject(bucket, key, awsAccessKeyId, timestamp, true, signature, null);
        }

        /// <summary>
        /// Get the logging configuration for a given bucket
        /// </summary>
        /// <param name="bucket">The name of the bucket</param>
        public BucketLoggingStatus getBucketLogging(string bucket)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("GetBucketLoggingStatus", timestamp);
            return s3.GetBucketLoggingStatus(bucket, awsAccessKeyId, timestamp, true, signature, null);
        }

        /// <summary>
        /// Set the logging configuration for a given bucket
        /// </summary>
        /// <param name="bucket">The name of the bucket</param>
        /// <param name="status">The configuration object that represents how the bucket should be logged.</param>
        public void putBucketLogging(string bucket, BucketLoggingStatus status)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("SetBucketLoggingStatus", timestamp);
            s3.SetBucketLoggingStatus(bucket, awsAccessKeyId, timestamp, true, signature, null, status);
        }

        /// <summary>
        /// Get the ACL for a given object
        /// </summary>
        /// <param name="bucket">The bucket to get the ACL from.</param>
        public AccessControlPolicy getBucketACL(string bucket)
        {
            return getACL(bucket, null);
        }

        /// <summary>
        /// Get the ACL for a given object
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives, or
        /// the bucket to get the ACL from.</param>
        /// <param name="key">The name of the key to use.</param>
        public AccessControlPolicy getACL(string bucket, string key)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            if (key != null)
            {
                string signature = makeSignature("GetObjectAccessControlPolicy", timestamp);
                return s3.GetObjectAccessControlPolicy(bucket, key, awsAccessKeyId, timestamp, true, signature, null);
            }
            else
            {
                string signature = makeSignature("GetBucketAccessControlPolicy", timestamp);
                return s3.GetBucketAccessControlPolicy(bucket, awsAccessKeyId, timestamp, true, signature, null);
            }
        }

        /// <summary>
        /// Write a new ACL for a given object
        /// </summary>
        /// <param name="bucket">The name of the bucket to change the ACL.</param>
        /// <param name="accessControList">Access Control Policy</param>
        public void putBucketACL(string bucket, Grant[] accessControlList)
        {
            putACL(bucket, null, accessControlList);
        }

        /// <summary>
        /// Write a new ACL for a given object (or bucket if key is null)
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives or the
        /// name of the bucket to change the ACL if key is null.</param>
        /// <param name="key">The name of the key to use; can be null.</param>
        /// <param name="accessControList">Access Control Policy</param>
        public void putACL(string bucket, string key, Grant[] accessControList)
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            if (key != null)
            {
                string signature = makeSignature("SetObjectAccessControlPolicy", timestamp);
                s3.SetObjectAccessControlPolicy(bucket, key, accessControList, awsAccessKeyId, timestamp, true, signature, null);
            }
            else
            {
                string signature = makeSignature("SetBucketAccessControlPolicy", timestamp);
                s3.SetBucketAccessControlPolicy(bucket, accessControList, awsAccessKeyId, timestamp, true, signature, null);
            }
        }

        /// <summary>
        /// List all the buckets created by this account.
        /// </summary>
        public ListAllMyBucketsResult listAllMyBuckets()
        {
            DateTime timestamp = AWSDateFormatter.GetCurrentTimeResolvedToMillis();
            string signature = makeSignature("ListAllMyBuckets", timestamp);
            return s3.ListAllMyBuckets(awsAccessKeyId, timestamp, true, signature);
        }

        internal string makeSignature(string method, DateTime timestamp)
        {
            string canonicalString = SERVICE_NAME + method + AWSDateFormatter.FormatAsISO8601(timestamp);
            Encoding ae = new UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(ae.GetBytes(awsSecretAccessKey));
            return Convert.ToBase64String(
                                        signature.ComputeHash(ae.GetBytes(
                                                        canonicalString.ToCharArray()))
                                               );
        }

    }


    class AWSDateFormatter
    {
        private static string TIMESTAMP_FORMAT = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

        /// <summary>
        /// Converts the date to an ISO-8601, resolved to milliseconds.
        /// </summary>
        public static string FormatAsISO8601(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString(TIMESTAMP_FORMAT,
                                System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets time in the DateTime format, resolved to milliseconds.
        /// </summary>
        public static DateTime GetCurrentTimeResolvedToMillis()
        {
            DateTime dateTime = DateTime.Now;
            // NOTE: THIS IS .NET VERSION SPECIFIC:
            //
            // For .NET 2.x, please ensure that you have DateTimeKind.Local at
            // the end of this method.
            //
            // For .NET 1.x, DateTimeKind does not exist and will generate
            // a compile-time error.; simply comment that line out.
            //
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                                 dateTime.Hour, dateTime.Minute, dateTime.Second,
                                 dateTime.Millisecond
                                 , DateTimeKind.Local   // COMMENT OUT THIS LINE FOR .NET 1.1
                               );
        }
    }
}
