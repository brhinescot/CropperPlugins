using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Cropper.TFSWorkItem
{
    public class Settings
    {
        public Settings()
        {
            #if DEBUG
            string configFile = @"Cropper.TFSWorkItem.dll.config";
            #else
            string configFile = @".\plugins\Cropper.TFSWorkItem.dll.config";
            #endif
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = configFile;
            configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        }

        private Configuration configuration;

        private string GetAppSettingsKeyValue(string keyName)
        {
            if (configuration.AppSettings.Settings[keyName] == null)
            {
                configuration.AppSettings.Settings.Add(keyName, "");
                configuration.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            return configuration.AppSettings.Settings[keyName].Value;
        }

        private void SetAppSettingsKeyValue(string keyName, string keyValue)
        {
            if (configuration.AppSettings.Settings[keyName] != null)
            {
                configuration.AppSettings.Settings[keyName].Value = keyValue;
            }
            else
            {
                configuration.AppSettings.Settings.Add(keyName, keyValue);
            }
            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");
        }

        public string TeamServer
        {
            get 
            {
                return GetAppSettingsKeyValue(Constants.TeamServerConfigKey);
            }
            set 
            {
                SetAppSettingsKeyValue(Constants.TeamServerConfigKey, value);
            }
        }

        public string TeamProject
        {
            get
            {
                return GetAppSettingsKeyValue(Constants.TeamProjectConfigKey);
            }
            set
            {
                SetAppSettingsKeyValue(Constants.TeamProjectConfigKey, value);
            }
        }

        public string WorkItemType
        {
            get
            {
                return GetAppSettingsKeyValue(Constants.WorkItemTypeConfigKey);
            }
            set
            {
                SetAppSettingsKeyValue(Constants.WorkItemTypeConfigKey, value);
            }
        }

        public bool DoNotShowOptionsDialogAgain
        {
            get
            {
                return bool.Parse(GetAppSettingsKeyValue(Constants.DoNotShowOptionsDialogAgainConfigKey));
            }
            set
            {
                SetAppSettingsKeyValue(Constants.DoNotShowOptionsDialogAgainConfigKey, value.ToString());
            }
        }

        public string DefaultImageName
        {
            get
            {
                return GetAppSettingsKeyValue(Constants.DefaultImageNameConfigKey);
            }
            set
            {
                SetAppSettingsKeyValue(Constants.DefaultImageNameConfigKey, value);
            }
        }

        public string DefaultOutputExtension
        {
            get
            {
                return GetAppSettingsKeyValue(Constants.DefaultOutputExtensionConfigKey);
            }
            set
            {
                SetAppSettingsKeyValue(Constants.DefaultOutputExtensionConfigKey, value);
            }
        }

        public string DefaultAttachmentComment
        {
            get
            {
                return GetAppSettingsKeyValue(Constants.DefaultAttachmentCommentConfigKey);
            }
            set
            {
                SetAppSettingsKeyValue(Constants.DefaultAttachmentCommentConfigKey, value);
            }
        }

        public bool OpenImageInEditor
        {
            get
            {
                return bool.Parse(GetAppSettingsKeyValue(Constants.OpenImageInEditorConfigKey));
            }
            set
            {
                SetAppSettingsKeyValue(Constants.OpenImageInEditorConfigKey, value.ToString());
            }
        }

        public string ImageEditor
        {
            get
            {
                return GetAppSettingsKeyValue(Constants.ImageEditorConfigKey);
            }
            set
            {
                SetAppSettingsKeyValue(Constants.ImageEditorConfigKey, value);
            }
        }

    }
}
