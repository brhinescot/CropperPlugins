using System;

namespace Cropper.SendToS3
{
    public class PutDataEventArgs : EventArgs
    {
        int _bytesSent;
        long _totalBytesSent;
        long _totalByteCount;
        string _key;
        object _tag;

        public PutDataEventArgs(string key, int bytesSent, long totalBytesSent, long totalByteCount)
        {
            _key = key;
            _bytesSent = bytesSent;
            _totalBytesSent = totalBytesSent;
            _totalByteCount = totalByteCount;
        }

        public string Key
        {
            get { return _key; }
        }

        public int BytesSent
        {
            get { return _bytesSent; }
        }

        public long TotalBytesSent
        {
            get { return _totalBytesSent; }
        }

        public long TotalByteCount
        {
            get { return _totalByteCount; }
        }

        public int PercentComplete
        {
            get { return Convert.ToInt32(Math.Round(((double)_totalBytesSent / (double)_totalByteCount)*100, 0)); }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
    }

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

    public delegate void PutDataEventHandler(object sender, PutDataEventArgs e);
    public delegate void FileUploadCompleteEventHandler(object sender, FileUploadCompleteEventArgs e);
}
