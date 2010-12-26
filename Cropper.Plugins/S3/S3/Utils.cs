using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using CropperPlugins.Utils;

namespace Cropper.SendToS3
{
    static class Utils
    {
        public static readonly string METADATA_PREFIX = "x-amz-meta-";
        public static readonly string AMAZON_HEADER_PREFIX = "x-amz-";
        public static readonly string ALTERNATIVE_DATE_HEADER = "x-amz-date";


        public static string MakeCanonicalString( string bucket, string resource, WebRequest request )
        {
            return MakeCanonicalString( bucket + "/" + resource, request );
        }


        public static string MakeCanonicalString( string resource, WebRequest request )
        {
            // I'm not sure why we want to clone the headers here.
            var headersClone = new Dictionary<String,String>();
            foreach ( string key in request.Headers )
            {
                headersClone.Add( key, request.Headers[ key ] );
            }
            if (!headersClone.ContainsKey("Content-Type"))
                headersClone.Add("Content-Type", request.ContentType);

            return MakeCanonicalString(request.Method, resource, headersClone, null);
        }



        public static string MakeCanonicalString( string verb,
                                                  string resource,
                                                  Dictionary<String,String> headers,
                                                  string expires )
        {
            StringBuilder buf = new StringBuilder(verb);
            buf.Append( "\n" );

            // AWS wants headers to be alphabetically sorted by key (header) name
            var hdrsToSign = new SortedDictionary<String,String>();
            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    //Tracing.Trace("C14n: looking at '{0}'", key);
                    string lk = key.ToLower();
                    if (lk.Equals("content-type") ||
                        lk.Equals("content-md5") ||
                        lk.Equals("date") ||
                        lk.StartsWith(AMAZON_HEADER_PREFIX))
                    {
                        hdrsToSign.Add(lk, headers[key]);
                    }
                }
            }

            if ( expires != null  &&  !hdrsToSign.ContainsKey("date"))
                hdrsToSign.Add( "date", expires );

            // per http://docs.amazonwebservices.com/AmazonS3/latest/dev/index.html?RESTAuthentication.html
            // encode these headers as blank lines, if they don't exist.

            if (!hdrsToSign.ContainsKey( "date" ) )
                hdrsToSign.Add( "date", "" );
            if (!hdrsToSign.ContainsKey( "content-type" ) )
                hdrsToSign.Add( "content-type", "" );
            if (!hdrsToSign.ContainsKey( "content-md5" ) )
                hdrsToSign.Add( "content-md5", "" );

            // generate the string to be signed
            foreach ( string key in hdrsToSign.Keys )
            {
                //Tracing.Trace("generating sig: hdr '{0}'", key);
                if ( key.StartsWith( AMAZON_HEADER_PREFIX ) )
                {
                    buf.Append( key ).Append( ":" ).Append( ( hdrsToSign[ key ] as string ).Trim() );
                }
                else
                {
                    // for Non-amazon headers, insert only the value, no name
                    buf.Append( hdrsToSign[ key ] );
                }
                buf.Append( "\n" );
            }

            // Do not include the query string parameters
            int queryIndex = resource.IndexOf( '?' );
            if ( queryIndex == -1 )
                buf.Append( "/" + resource );
            else
                buf.Append( "/" + resource.Substring( 0, queryIndex ) );

            Regex aclQueryStringRegEx = new Regex( ".*[&?]acl($|=|&).*" );
            Regex torrentQueryStringRegEx = new Regex( ".*[&?]torrent($|=|&).*" );
            Regex loggingQueryStringRegEx = new Regex(".*[&?]logging($|=|&).*");
            if (aclQueryStringRegEx.IsMatch(resource))
            {
                buf.Append( "?acl" );
            }
            else if ( torrentQueryStringRegEx.IsMatch( resource ) )
            {
                buf.Append( "?torrent" );
            }
            else if (loggingQueryStringRegEx.IsMatch(resource))
            {
                buf.Append("?logging");
            }

            return buf.ToString();
        }



        public static string SignAndEncode( string awsSecretAccessKey,
                                            string canonicalString,
                                            bool urlEncode )
        {
            Encoding utf8 = new UTF8Encoding();
            HMACSHA1 hmac = new HMACSHA1(utf8.GetBytes(awsSecretAccessKey));
            var signature = hmac.ComputeHash(utf8.GetBytes(canonicalString));
            var b64sig = Convert.ToBase64String(signature);

            return (urlEncode)
                ? HttpUtility.UrlEncode(b64sig)
                : b64sig;
        }

        public static string SlurpInputStream(Stream stream)
        {
            System.Text.Encoding encode =
                System.Text.Encoding.GetEncoding("utf-8");
            StringBuilder data = new StringBuilder();
            using (StreamReader sr = new StreamReader(stream, encode))
            {
                const int stride = 4096;
                char[] buffer = new char[stride];
                int count;
                while ((count = sr.Read(buffer, 0, stride)) > 0)
                {
                    data.Append(new string(buffer, 0, count));
                }
            }
            return data.ToString();
        }

        public static string GetHttpDate()
        {
            // Setting the Culture will ensure we get a proper HTTP Date.
            string date = DateTime.UtcNow.ToString( "ddd, dd MMM yyyy HH:mm:ss ", System.Globalization.CultureInfo.InvariantCulture ) + "GMT";
            return date;
        }

    }
}
