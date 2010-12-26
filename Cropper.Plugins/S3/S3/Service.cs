using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;

using CropperPlugins.Utils;

namespace Cropper.SendToS3
{
    public class S3Object
    {
        public System.IO.Stream Stream { get; set; }
        public Dictionary<String,String> Metadata { get; set; }

    }

    public static class Extensions
    {
        public static string GetResponseMessage(this HttpWebResponse response)
        {
            string data = Utils.SlurpInputStream( response.GetResponseStream() );
            response.GetResponseStream().Close();
            return data;
        }
    }


    public class Service
    {
        private string awsAccessKeyId;
        private string awsSecretAccessKey;
        private bool isSecure;
        private string server;
        private int port;
        private int chunkSize = 4096;

        public Service( string awsAccessKeyId, string awsSecretAccessKey ):
            this( awsAccessKeyId, awsSecretAccessKey, false )
        {
        }

        public Service( string awsAccessKeyId, string awsSecretAccessKey, bool isSecure ):
            this( awsAccessKeyId, awsSecretAccessKey, isSecure, "s3.amazonaws.com" )
        {
        }

        public Service( string awsAccessKeyId, string awsSecretAccessKey, bool isSecure,
                        string server ) :
            this( awsAccessKeyId, awsSecretAccessKey, server,
                  isSecure ? 443 : 80 )
        {
        }

        public Service( string awsAccessKeyId,
                        string awsSecretAccessKey,
                        string server, int port )
        {
            this.awsAccessKeyId = awsAccessKeyId;
            this.awsSecretAccessKey = awsSecretAccessKey;
            this.server = server;
            this.port = port;
            this.isSecure = (port==443);
        }

        /// <summary>
        ///   size of the chunk to use when uploading a large stream of
        ///   data to the server.
        /// </summary>
        public int ChunkSize
        {
            get { return chunkSize; }
            set { chunkSize = value; }
        }

        /// <summary>
        /// Creates a new bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to create</param>
        /// <param name="headers">A Map of string to string representing the headers to pass (can be null)</param>
        public WebResponse CreateBucket( string bucket, Dictionary<String,String> headers )
        {
            S3Object obj = new S3Object();
            WebRequest request = MakeRequest("PUT", bucket, headers, obj);
            request.ContentLength = 0;
            request.GetRequestStream().Close();
            return request.GetResponse();
        }

        /// <summary>
        /// Lists the contents of a bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to list</param>
        /// <param name="prefix">All returned keys will start with this string (can be null)</param>
        /// <param name="marker">All returned keys will be lexographically greater than this string (can be null)</param>
        /// <param name="maxKeys">The maximum number of keys to return (can be 0)</param>
        /// <param name="headers">A Map of string to string representing HTTP headers to pass.</param>
        public ListBucketResult ListBucket( string bucket,
                                            string prefix,
                                            string marker,
                                            int maxKeys,
                                            Dictionary<String,String> headers )
        {
            return ListBucket( bucket, prefix, marker, maxKeys, null, headers );
        }

        /// <summary>
        /// Lists the contents of a bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to list</param>
        /// <param name="prefix">All returned keys will start with this string (can be null)</param>
        /// <param name="marker">All returned keys will be lexographically greater than this string (can be null)</param>
        /// <param name="maxKeys">The maximum number of keys to return (can be 0)</param>
        /// <param name="headers">A Map of string to string representing HTTP headers to pass.</param>
        /// <param name="delimiter">Keys that contain a string between the prefix and the first
        /// occurrence of the delimiter will be rolled up into a single element.</param>
        public ListBucketResult ListBucket( string bucket,
                                            string prefix,
                                            string marker,
                                            int maxKeys,
                                            string delimiter,
                                            Dictionary<String,String> headers )
        {
            StringBuilder path = new StringBuilder( bucket );
            path.Append( "?" );
            if (prefix != null)
                path.Append("prefix=").Append(HttpUtility.UrlEncode(prefix)).Append("&");
            if ( marker != null )
                path.Append( "marker=" ).Append(HttpUtility.UrlEncode(marker)).Append( "&" );
            if ( maxKeys != 0 )
                path.Append( "max-keys=" ).Append(maxKeys).Append( "&" );
            if (delimiter != null)
                path.Append("delimiter=").Append(HttpUtility.UrlEncode(delimiter)).Append("&");
            // remove the last ampersand
            path.Remove( path.Length - 1, 1 );

            var request = MakeRequest( "GET", path.ToString(), headers );
            var result = (ListBucketResult)
                ReadAndDeserialize(request, typeof(ListBucketResult));
            return result;
        }


        /// <summary>
        /// Deletes an empty Bucket.
        /// </summary>
        /// <param name="bucket">The name of the bucket to delete</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        /// <returns></returns>
        public HttpWebResponse DeleteBucket( string bucket, Dictionary<String,String> headers)
        {
            var req = MakeRequest( "DELETE", bucket, headers );
            return req.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Writes an object to S3.
        /// </summary>
        /// <param name="bucket">The name of the bucket to which the object will be added.</param>
        /// <param name="key">The name of the key to use</param>
        /// <param name="obj">An S3Object containing the data to write.</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse Put( string bucket, string key, S3Object obj, Dictionary<String,String> headers )
        {
            var request = MakeRequest("PUT",
                                      bucket,
                                      EncodeKeyForSignature(key),
                                      headers,
                                      obj);

            request.ContentLength = obj.Stream.Length;
            write(key, obj.Stream, request.GetRequestStream());
            obj.Stream.Close();
            request.GetRequestStream().Close();

            return request.GetResponse() as HttpWebResponse;
        }

        public event PutDataEventHandler PutData;
        public event FileUploadCompleteEventHandler FileUploadComplete;

        protected void OnPutData(string key, int bytesSent, long totalBytesSent, long totalByteCount)
        {
            if (key == null) return;
            if (PutData == null) return;

            PutData(this, new PutDataEventArgs(key, bytesSent, totalBytesSent, totalByteCount));

        }

        protected void OnFileUploadComplete(string key)
        {
            if (key == null) return;
            if (FileUploadComplete == null) return;

            FileUploadComplete(this, new FileUploadCompleteEventArgs(key));
        }

        // NOTE: The System.Net.Uri class does modifications to the URL.
        // For example, if you have two consecutive slashes, it will
        // convert these to a single slash.  This could lead to invalid
        // signatures as best and at worst keys with names you do not
        // care for.
        private static string EncodeKeyForSignature(string key)
        {
            return HttpUtility.UrlEncode(key).Replace("%2f", "/");
        }

        /// <summary>
        /// Reads an object from S3
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives</param>
        /// <param name="key">The name of the key to use</param>
        /// <param name="headers">A Map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse Get( string bucket,
                                    string key,
                                    Dictionary<String,String> headers )
        {
            var req = MakeRequest("GET",
                                  bucket,
                                  EncodeKeyForSignature(key),
                                  headers);
            return req.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Delete an object from S3.
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives.</param>
        /// <param name="key">The name of the key to use.</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        /// <returns></returns>
        public HttpWebResponse Delete( string bucket,
                                       string key,
                                       Dictionary<String,String> headers )
        {
            var req = MakeRequest("DELETE",
                                  bucket,
                                  EncodeKeyForSignature(key),
                                  headers);
            return req.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Get the logging xml document for a given bucket
        /// </summary>
        /// <param name="bucket">The name of the bucket</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse GetBucketLogging(string bucket,
                                                Dictionary<String,String> headers)
        {
            var req = MakeRequest("GET", bucket + "?logging", headers);
            return req.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Write a new logging xml document for a given bucket
        /// </summary>
        /// <param name="bucket">The name of the bucket to enable/disable logging on</param>
        /// <param name="loggingXMLDoc">The xml representation of the logging configuration as a String.</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse PutBucketLogging(string bucket,
                                         string loggingXMLDoc,
                                         Dictionary<String,String> headers)
        {
            S3Object obj = new S3Object
                {
                    Stream = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(loggingXMLDoc))
                };

            WebRequest request = MakeRequest("PUT", bucket + "?logging", headers, obj);

            request.ContentLength = obj.Stream.Length;
            write(obj.Stream, request.GetRequestStream());
            obj.Stream.Close();
            request.GetRequestStream().Close();

            return request.GetResponse() as HttpWebResponse;
        }


        private long write(Stream source, Stream destination)
        {
            return write(null, source, destination);
        }


        private long write(string key, Stream source, Stream destination)
        {
            int read = 0;
            long totalSize = source.Length;
            long totalWritten = 0;

            byte[] buffer = new byte[chunkSize];

            while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, read);
                totalWritten += read;

                OnPutData(key, read, totalWritten, totalSize);
            }

            OnFileUploadComplete(key);
            return totalWritten;
        }

        /// <summary>
        /// Get the ACL for a given bucket.
        /// </summary>
        /// <param name="bucket">The the bucket to get the ACL from.</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse GetBucketACL(string bucket, Dictionary<String,String> headers)
        {
            return GetACL(bucket, null, headers);
        }

        /// <summary>
        /// Get the ACL for a given object
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives</param>
        /// <param name="key">The name of the key to use.</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse GetACL( string bucket, string key, Dictionary<String,String> headers )
        {
            if ( key == null ) key = "";
            var req = MakeRequest("GET",
                                  bucket,
                                  EncodeKeyForSignature(key) + "?acl",
                                  headers);
            return req.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Write a new ACL for a given bucket
        /// </summary>
        /// <param name="bucket">The name of the bucket to change the ACL.</param>
        /// <param name="aclXMLDoc">An XML representation of the ACL as a string.</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse PutBucketACL(string bucket,
                                     string aclXMLDoc,
                                     Dictionary<String,String> headers)
        {
            return PutACL(bucket, null, aclXMLDoc, headers);
        }

        /// <summary>
        /// Write a new ACL for a given object
        /// </summary>
        /// <param name="bucket">The name of the bucket where the object lives or the
        /// name of the bucket to change the ACL if key is null.</param>
        /// <param name="key">The name of the key to use; can be null.</param>
        /// <param name="aclXMLDoc">An XML representation of the ACL as a string.</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public HttpWebResponse PutACL(string bucket,
                               string key,
                               string aclXMLDoc,
                               Dictionary<String,String> headers)
        {
            S3Object obj = new S3Object
                {
                    Stream = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(aclXMLDoc))
                };
            if ( key == null ) key = "";

            var request = MakeRequest("PUT", bucket + "/" + EncodeKeyForSignature(key) + "?acl", headers, obj);

            request.ContentLength = obj.Stream.Length;
            write(obj.Stream, request.GetRequestStream());
            obj.Stream.Close();
            request.GetRequestStream().Close();

            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// List all the buckets created by this account.
        /// </summary>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        public ListAllMyBucketsResult ListAllMyBuckets( Dictionary<String,String> headers )
        {
            var request = MakeRequest("GET", "", headers);
            var result = (ListAllMyBucketsResult)
                ReadAndDeserialize(request, typeof(ListAllMyBucketsResult));
            return result;
        }


        /// <summary>
        ///   utility method to read a response and deserialize a particular
        ///   type from the XML.
        /// </summary>
        private object ReadAndDeserialize(HttpWebRequest request, Type type)
        {
            using (var r = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(r.GetResponseStream()))
                {
                    var s1 = new XmlSerializer(type);
                    var r2= s1.Deserialize(new System.Xml.XmlTextReader(reader));
                    return r2;
                }
            }
        }


        /// <summary>
        ///   Construct a new WebRequest without an S3Object, and without specifying a
        ///   bucket.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This will construct a request using the "legacy" format for the
        ///     AWS REST API.  The legacy interface requires the bucket to be
        ///     specified as the first segment in the resource path, as in
        ///     /bucket1/foo.jpg.  The recommended AWS API is to specify the
        ///     bucket name as the prefix to the aws host name, as in
        ///     bucket1.aws.amazon.com .
        ///   </para>
        ///   <para>
        ///     To use the *recommended* interface, call a MakeRequest overload that
        ///     allows you to explicitly specify the bucket name.
        ///   </para>
        ///   <para>
        ///     The constructed request includes the Authorization header,
        ///     containing the HMACSHA1 signature required by Amaazon AWS.
        ///   </para>
        ///   <para>
        ///     This method does not actually *send* the request to AWS.  It
        ///     merely constructs it.  The caller is responsible for sending.
        ///     Calling request.GetResponse() will do so implicitly.
        ///   </para>
        /// </remarks>
        private HttpWebRequest MakeRequest( string method,
                                            string resource,
                                            Dictionary<String,String> headers )
        {
            return MakeRequest( method, null, resource, headers, null );
        }

        /// <summary>
        ///   Construct a new WebRequest.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This will construct an HttpWebRequest using the recommended AWS
        ///     REST API, using the bucket name as the DNS prefix.  For example,
        ///     specifying "GET", "bucket1" and "foo.jpg" as the arguments will
        ///     result in a HttpWebRequest to the URL
        ///     http://bucket1.aws.amazon.com/foo.jpg.
        ///   </para>
        ///   <para>
        ///     The constructed request includes the Authorization header,
        ///     containing the HMACSHA1 signature required by Amaazon AWS.
        ///   </para>
        ///   <para>
        ///     This method does not actually *send* the request to AWS.  It
        ///     merely constructs it.  The caller is responsible for sending.
        ///     Calling request.GetResponse() will do so implicitly.
        ///   </para>
        /// </remarks>
        private HttpWebRequest MakeRequest( string method,
                                            string bucket,
                                            string resource,
                                            Dictionary<String,String> headers )
        {
            return MakeRequest( method, bucket, resource, headers, null );
        }


        private HttpWebRequest MakeRequest( string method,
                                            string resource,
                                            Dictionary<String,String> headers,
                                            S3Object obj )

        {
            return MakeRequest(  method, null, resource, headers, obj );
        }


        /// <summary>
        ///   Construct a new HttpWebRequest for AWS, for the specified resource,
        ///   within the specified bucket, and using the given headers.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     The constructed request includes the Authorization header,
        ///     containing the HMACSHA1 signature required by Amaazon AWS.
        ///   </para>
        ///   <para>
        ///     This method does not actually *send* the request to AWS.  It
        ///     merely constructs it.  The caller is responsible for sending.
        ///     Calling request.GetResponse() will do so implicitly.
        ///   </para>
        ///   <para>
        ///     If there is an S3Object specified, as for example when uploading a
        ///     file to S3, only the metadata associated to that object is added
        ///     to the constructed request.  The caller must explicitly send the
        ///     data associated to the S3Object stream, before calling
        ///     GetResponse().
        ///   </para>
        /// </remarks>
        /// <param name="method">The HTTP method to use (GET, PUT, DELETE)</param>
        /// <param name="bucket">The bucket name</param>
        /// <param name="resource">The resource name (key)</param>
        /// <param name="headers">A map of string to string representing the HTTP headers to pass (can be null)</param>
        /// <param name="obj">S3Object that is to be written (can be null).</param>
        private HttpWebRequest MakeRequest( string method,
                                            string bucket,
                                            string resource,
                                            Dictionary<String,String> headers,
                                            S3Object obj )
        {
            string url = MakeURL( bucket, resource );
            var req = WebRequest.Create( url ) as HttpWebRequest;
            req.Timeout = System.Threading.Timeout.Infinite;

            if (req is HttpWebRequest)
                (req as HttpWebRequest).AllowWriteStreamBuffering = false;

            req.Method = method;
            AddHeaders( req, headers );
            if ( obj != null ) AddMetadataHeaders( req, obj.Metadata );
            AddAuthHeader( req, bucket, resource );

            return req;
        }


        /// <summary>
        /// Add the given headers to the WebRequest
        /// </summary>
        ///
        /// <param name="req">Web request to add the headers to.</param>
        /// <param name="headers">
        ///    A map of string to string representing the HTTP headers to pass
        ///    (can be null)
        /// </param>
        private void AddHeaders( WebRequest req, Dictionary<String,String> headers )
        {
            AddHeaders( req, headers, "" );
        }

        /// <summary>
        /// Add the given metadata fields to the WebRequest.
        /// </summary>
        /// <param name="req">Web request to add the headers to.</param>
        /// <param name="metadata">A map of string to string representing the S3 metadata for this resource.</param>
        private void AddMetadataHeaders( WebRequest req, Dictionary<String,String> metadata )
        {
            AddHeaders( req, metadata, Utils.METADATA_PREFIX );
        }

        /// <summary>
        /// Add the given headers to the WebRequest with a prefix before the keys.
        /// </summary>
        /// <param name="req">WebRequest to add the headers to.</param>
        /// <param name="headers">Headers to add.</param>
        /// <param name="prefix">String to prepend to each before ebfore adding it to the WebRequest</param>
        private void AddHeaders( WebRequest req,
                                 Dictionary<String,String> headers,
                                 string prefix )
        {
            if ( headers != null )
            {
                foreach ( string key in headers.Keys )
                {
                    if (prefix.Length == 0 && key.Equals("Content-Type"))
                    {
                        req.ContentType = headers[key] as string;
                    }
                    else
                    {
                        req.Headers.Add(prefix + key, headers[key] as string);
                    }
                }
            }
        }

        /// <summary>
        /// Add the appropriate Authorization header to the WebRequest
        /// </summary>
        /// <param name="request">Request to add the header to</param>
        /// <param name="resource">The resource name (bucketName + "/" + key)</param>
        private void AddAuthHeader( WebRequest request, string bucket, string resource )
        {
            if ( request.Headers[Utils.ALTERNATIVE_DATE_HEADER] == null )
            {
                request.Headers.Add(Utils.ALTERNATIVE_DATE_HEADER, Utils.GetHttpDate());
            }

            string stringToSign = Utils.MakeCanonicalString( bucket, resource, request );
            Tracing.Trace("StringToSign: {0}", stringToSign);
            string encodedSig = Utils.SignAndEncode( awsSecretAccessKey, stringToSign, false );
            Tracing.Trace("signature: {0}", encodedSig);
            request.Headers.Add( "Authorization", "AWS " + awsAccessKeyId + ":" + encodedSig );
        }

        /// <summary>
        ///   Format a URL for the given resource.
        /// </summary>
        /// <param name="resource">The resource name (bucketName + "/" + key)</param>
        private string MakeURL( string resource )
        {
            return String.Format("http{0}://{1}:{2}/{3}",
                                 isSecure ? "s" : "",
                                 server, port, resource);
        }


        /// <summary>
        ///   Format a URL for the given resource.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     There are two formats for resources via Amazon's REST API. The
        ///     legacy format specifies the bucket name as the first segment in
        ///     the resource path. So, you can do an HTTP GET on
        ///     http://aws.amazon.com/bucket1/foo.jpg to get the object with the
        ///     key "foo.jpg" from the bucket named "bucket1".
        ///   </para>
        ///   <para>
        ///     The updated format calls for the use of the bucket name as a
        ///     prefix to the aws server name.  The same resource as in the above
        ///     example is reachable via http://bucket1.aws.amazon.com/foo.jpg .
        ///   </para>
        ///   <para>
        ///     This method works for either format. In case the bucket name is
        ///   </para>
        /// </remarks>
        private string MakeURL( string bucket, string resource)
        {
            if (bucket==null)
                return MakeURL( resource );

            return String.Format("http{0}://{1}.{2}:{3}/{4}",
                                 isSecure ? "s" : "",
                                 bucket, server, port, resource);
        }
    }
}
