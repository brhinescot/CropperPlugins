using Fusion8.Cropper.Extensibility;
using Microsoft.Office.OneNote;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Cropper.SendToOneNote
{
    public class OneNote : IPersistableImageFormat
    {
        private IPersistableOutput _output;
        private const string DESCRIPTION = "Send to OneNote";
        private const string EXTENSION = "png";

        public event ImageFormatClickEventHandler ImageFormatClick;

        public void Connect(IPersistableOutput persistableOutput)
        {
            if (persistableOutput == null)
            {
                throw new ArgumentNullException("persistableOutput");
            }
            this._output = persistableOutput;
            this._output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
        }

        public void Disconnect()
        {
            this._output.ImageCaptured -= new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            ImageFormatEventArgs args1 = new ImageFormatEventArgs();
            args1.ClickedMenuItem = (MenuItem)sender; 
            args1.ImageOutputFormat = this; 
            this.ImageFormatClick.Invoke(sender, args1);
        }

        private void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            MemoryStream stream1 = new MemoryStream();
            e.FullSizeImage.Save(stream1, ImageFormat.Png);
            this.SendToOneNote(stream1.ToArray(), "Cropper Capture");
        }

        private void SendToOneNote(byte[] imageData, string title)
        {
            Page page1 = new Page("Side Notes.one", title);
            OutlineObject obj1 = new OutlineObject();
            ImageContent content1 = new ImageContent(imageData);
            HtmlContent content2 = new HtmlContent("<font size=small>Capture taken at " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "</font>");
            obj1.AddContent(content1);
            obj1.AddContent(content2);
            page1.AddObject(obj1);
            page1.Commit();
            page1.NavigateTo();
        }


        public string Description
        {
            get
            {
                return "Send to OneNote";
            }
        }

        public string Extension
        {
            get
            {
                return "png";
            }
        }

        public IPersistableImageFormat Format
        {
            get
            {
                return this;
            }
        }

        public MenuItem Menu
        {
            get
            {
                MenuItem item1 = new MenuItem();
                item1.RadioCheck = true;
                item1.Text = "Send to OneNote";
                item1.Click += new EventHandler(this.menuItem_Click);
                return item1;
            }
        }
    }
}

