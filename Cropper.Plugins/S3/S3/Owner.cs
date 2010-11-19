using System;
using System.Xml;

namespace Cropper.SendToS3.S3
{
	public class Owner
	{
		private string id;
		public String Id 
		{
			get 
			{
				return id;
			}
		}

		private string displayName;
		public string DisplayName 
		{
			get 
			{
				return displayName;
			}
		}

		public Owner( string id, string displayName )
		{
			this.id = id;
			this.displayName = displayName;
		}

		public Owner(XmlNode node)
		{
			foreach (XmlNode child in node.ChildNodes)
			{
				if (child.Name.Equals("ID"))
				{
					id = Utils.getXmlChildText(child);
				}
				else if (child.Name.Equals("DisplayName"))
				{
					displayName = Utils.getXmlChildText(child);
				}
			}
		}
	}
}
