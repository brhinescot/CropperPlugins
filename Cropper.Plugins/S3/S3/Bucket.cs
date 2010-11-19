using System;
using System.Xml;

namespace Cropper.SendToS3.S3
{
	public class Bucket
	{
		private string name;
		public string Name
		{
			get
			{
				return name;
			}
		}

		private DateTime creationDate;
		public DateTime CreationDate
		{
			get
			{
				return creationDate;
			}
		}

		public Bucket(string name, DateTime creationDate)
		{
			this.name = name;
			this.creationDate = creationDate;
		}

		public Bucket(XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name.Equals("Name"))
				{
					name = Utils.getXmlChildText(child);
				}
				else if (child.Name.Equals("CreationDate"))
				{
					string strDate = Utils.getXmlChildText(child);
					creationDate = Utils.parseDate(strDate);
				}
			}
		}
	}
}
