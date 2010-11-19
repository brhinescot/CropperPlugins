using System;
using System.Collections;
using System.Text;
using System.IO;

namespace Cropper.SendToS3.S3
{
	public class S3Object
	{
		private Stream _stream;
		public Stream Stream
		{
			get
			{
				return _stream;
			}
		}

		private SortedList _metadata;
		public SortedList Metadata 
		{
			get 
			{
				return _metadata;
			}
		}

		public S3Object(Stream stream, SortedList metaData)
		{
			_stream = stream;
			_metadata = metaData;
		}
	}
}
