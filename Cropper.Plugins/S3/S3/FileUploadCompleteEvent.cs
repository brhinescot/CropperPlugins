using System;

namespace Cropper.SendToS3.S3
{
	public class FileUploadCompleteEventArgs : EventArgs
	{
		object _tag;
		string _key;

		public FileUploadCompleteEventArgs(string key)
		{
			_key = key;
		}

		public object Tag
		{
			get { return _tag; }
			set { _tag = value; }
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}
	}

	public delegate void FileUploadCompleteEventHandler(object sender, FileUploadCompleteEventArgs e);
}
