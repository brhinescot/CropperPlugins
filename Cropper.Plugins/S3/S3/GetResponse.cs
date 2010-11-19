using System;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;

namespace Cropper.SendToS3.S3
{
	public class GetResponse : Response
	{
		private S3Object obj;
		public S3Object Object 
		{
			get 
			{
				return obj;
			}
		}

		public GetResponse( WebRequest request ) :
			base(request)
		{
			SortedList metadata = extractMetadata( response );
			this.obj = new S3Object( response.GetResponseStream(), metadata );
		}

		private static SortedList extractMetadata( WebResponse response )
		{
			SortedList metadata = new SortedList();
			foreach ( string key in response.Headers.Keys ) 
			{
				if ( key == null ) continue;
				if ( key.StartsWith( Utils.METADATA_PREFIX ) ) 
				{
					metadata.Add( key.Substring( Utils.METADATA_PREFIX.Length ), response.Headers[ key ] );
				}
			}
			return metadata;
		}
	}
}
