#region License Information

#endregion

#region Using Directives

using Fusion8.Cropper.Extensibility;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#endregion

namespace CropperPlugins
{
    public class SendToEmailFormat : IPersistableImageFormat, IConfigurablePlugin, IDisposable
	{
		private IPersistableOutput output;
		private string description;
		private bool isDisposed;
        private const bool hostInOptions = true;
		private const string FORMAT_NAME = "Send To Email";
        private Options configurationForm;
        private EmailOutputSettings settings;

		public event ImageFormatClickEventHandler ImageFormatClick;

		public SendToEmailFormat()
		{
			this.description = FORMAT_NAME;
		}

		private void SaveImage(Stream stream, Image image)
		{
			image.Save(stream, ImageFormat.Png);
		}

		public void Connect(IPersistableOutput persistableOutput)
		{
			if (persistableOutput == null)
			{
				throw new ArgumentNullException("persistableOutput");
			}
			this.output = persistableOutput;
			this.output.ImageCaptured += new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
			this.output.ImageCapturing += new ImageCapturingEventHandler(this.persistableOutput_ImageCapturing);
		}

		public void Disconnect()
		{
			this.output.ImageCaptured -= new ImageCapturedEventHandler(this.persistableOutput_ImageCaptured);
			this.output.ImageCapturing -= new ImageCapturingEventHandler(this.persistableOutput_ImageCapturing);
		}

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
			}
		}

		private void MenuItemClick(object sender, EventArgs e)
		{
			ImageFormatEventArgs args = new ImageFormatEventArgs();
			args.ClickedMenuItem = (MenuItem) sender;
			args.ImageOutputFormat = this;
			this.OnImageFormatClick(sender, args);
		}

		private void OnImageFormatClick(object sender, ImageFormatEventArgs e)
		{
			if (this.ImageFormatClick != null)
			{
				this.ImageFormatClick(sender, e);
			}
		}

		private void persistableOutput_ImageCaptured(object sender, ImageCapturedEventArgs e)
		{
			this.output.FetchOutputStream(new StreamHandler(this.SaveImage), e.ImageNames.FullSize, e.FullSizeImage);
            FileInfo file = new FileInfo(e.ImageNames.FullSize);
            string subject = replaceTokens(PluginSettings.Subject, file);
            string body = replaceTokens(PluginSettings.Message, file);
            MapiMailMessage message = new MapiMailMessage(subject, body);
            message.Files.Add(e.ImageNames.FullSize);
            message.ShowDialog();
		}

        private string replaceTokens(string input, FileInfo file)
        {
            if (!file.Exists)
                throw new System.IO.FileNotFoundException("Temporary file not found", file.FullName);

            string output = input.Replace("$NAME$", file.Name);
            output = output.Replace("$SIZE$", GetFileSizeStringFromContentLength(file.Length));
            output = output.Replace("$OPERATINGSYSTEM$", System.Environment.OSVersion.VersionString);

            foreach (Match m in Regex.Matches(output, @"\$(?<TOKEN>.+?)\$",RegexOptions.Multiline))
            {
                if(m.Groups["TOKEN"] != null)
                    output = output.Replace(m.Value,
                        System.Environment.GetEnvironmentVariable(m.Groups["TOKEN"].Value));
            }
            return output;
        }

        //http://weblogs.asp.net/jamauss/archive/2005/10/25/428482.aspx
        public static string GetFileSizeStringFromContentLength(long ContentLength)
        {
            if (ContentLength > 1048575)
            {
                return String.Format("{0:0.00 mb}", (Convert.ToDecimal((Convert.ToDouble(ContentLength) / 1024) / 1024)));
            }
            else
            {
                return String.Format("{0:0.00 kb}", (Convert.ToDecimal(Convert.ToDouble(ContentLength) / 1024)));
            }
        }

		private void persistableOutput_ImageCapturing(object sender, ImageCapturingEventArgs e)
		{
		}


		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public string Extension
		{
			get
			{
				return "Png";
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
				MenuItem item = new MenuItem();
				item.RadioCheck = true;
				item.Text = FORMAT_NAME;
				item.Click += new EventHandler(this.MenuItemClick);
				return item;
			}
		}

        #region IConfigurablePlugin Implementation

        /// <summary>
        /// Gets the plug-ins impementation of the <see cref="BaseConfigurationForm"/> used 
        /// for setting plug-in specific options.
        /// </summary>
        public BaseConfigurationForm ConfigurationForm
        {
            get
            {
                if (configurationForm == null)
                {
                    configurationForm = new Options();
                    configurationForm.OptionsSaved += HandleConfigurationFormOptionsSaved;
                    configurationForm.Format = PluginSettings.Format;
                    configurationForm.ImageQuality = PluginSettings.ImageQuality;
                }
                return configurationForm;
            }
        }

        /// <summary>
        /// Gets a value indicating if the <see cref="ConfigurationForm"/> is to be hosted 
        /// in the options dialog or shown in its own dialog window.
        /// </summary>
        public bool HostInOptions
        {
            get { return hostInOptions; }
        }

        /// <summary>
        /// Gets or sets an object containing the plug-in's settings.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is set during startup with the settings contained in the applications
        /// configuration file.</para>
        /// <para>
        /// The object returned by this property is serialized into the applications configuration
        /// file on shutdown.</para></remarks>
        public object Settings
        {
            get { return PluginSettings; }
            set { PluginSettings = value as EmailOutputSettings; }
        }

        // Helper property for IConfigurablePlugin Implementation
        private EmailOutputSettings PluginSettings
        {
            get
            {
                if (settings == null)
                    settings = new EmailOutputSettings();
                return settings;
            }
            set { settings = value; }
        }

        private void HandleConfigurationFormOptionsSaved(object sender, EventArgs e)
        {
            PluginSettings.Format = configurationForm.Format;
            PluginSettings.ImageQuality = configurationForm.ImageQuality;
            PluginSettings.Subject = configurationForm.Subject;
            PluginSettings.Message = configurationForm.Message;
        }

        #endregion
    }
}