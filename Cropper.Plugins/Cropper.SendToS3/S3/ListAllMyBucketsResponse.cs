using System;
using System.Collections;
using System.Net;
using System.Text;
using System.Xml;

namespace Cropper.SendToS3.S3
{
	public class ListAllMyBucketsResponse : Response
	{
		private Owner owner;
		public Owner Owner 
		{
			get 
			{
				return owner;
			}
		}

		private ArrayList buckets;
		public ArrayList Buckets 
		{
			get 
			{
				return buckets;
			}
		}

		public ListAllMyBucketsResponse( WebRequest request ) :
			base(request)
		{
			buckets = new ArrayList();
			string rawBucketXML = Utils.slurpInputStream( response.GetResponseStream() );

			XmlDocument doc = new XmlDocument();
			doc.LoadXml( rawBucketXML );
			foreach (XmlNode node in doc.ChildNodes)
			{
				if (node.Name.Equals("ListAllMyBucketsResult"))
				{
					foreach (XmlNode child in node.ChildNodes)
					{
						if (child.Name.Equals("Owner"))
						{
							owner = new Owner(child);
						}
						else if (child.Name.Equals("Buckets"))
						{
							foreach (XmlNode bucket in child.ChildNodes)
							{
								if (bucket.Name.Equals("Bucket"))
								{
									buckets.Add(new Bucket(bucket));
								}
							}
						}
					}
				}
			}
		}
	}
}
