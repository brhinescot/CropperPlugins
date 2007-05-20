namespace AltmanSoftware.Cropper.FlickrOutput
{
    using System;

    public class PhotoSetItem
    {
        public PhotoSetItem(string id, string title)
        {
            this._id = id;
            this._title = title;
        }

        public override string ToString()
        {
            return this.Title;
        }


        public string Id
        {
            get
            {
                return this._id;
            }
        }

        public string Title
        {
            get
            {
                return this._title;
            }
        }


        private string _id;
        private string _title;
    }
}

