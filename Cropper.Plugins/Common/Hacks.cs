// Hacks.cs
//
// Part of CropperPlugins.
//
// Utility methods.
//

namespace CropperPlugins.Common
{
    using System;
    using System.Collections.Generic;

    public static class Hacks
    {
        /// <summary>
        ///   Put settings into the static property within Cropper.Core, via
        ///   reflection.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This is necessary because in some cases settings (such
        ///     as an email address, user name) may change during normal
        ///     execution of a plugin, without opening the options form,
        ///     and there's no way for the plugin to communicate that
        ///     back to Cropper before Cropper saves its settings.  This
        ///     is most common for authentication information. A plugin
        ///     may prompt for username/password, and then want to store
        ///     the username, in the course of a screen capture.
        ///   </para>
        ///   <para>
        ///     When Cropper starts, it reads the config file and stores all
        ///     the plugin settings "objects".  If it finds no settings
        ///     stored for a particular plugin, as will occur when the user
        ///     uses a plugin the very first time, no settings object is
        ///     stored for that plugin at all.  Also, there's no way for the
        ///     plugin to give Cropper the default settings for the plugin,
        ///     unless the options form is opened for that plugin.
        ///   </para>
        ///   <para>
        ///     This method works around that.
        ///   </para>
        ///   <para>
        ///     The first time a user uses a plugin that may prompt for
        ///     authentication information, such as the Picasa plugin,
        ///     or the Imgur plugin, he enters his authn information.
        ///     We want to store that in the persistent store.
        ///     (Each plugin needs to use a CachedSettings singleton
        ///     to insure that only one copy of that information gets stored,
        ///     regardless how many plugin instances get created. That's
        ///     not relevant here.)  What's necessary is to put the
        ///     settings instance for this plugin into the array of
        ///     settings objects for all plugins.  There's no public
        ///     method for doing so, therefore this plugin relies on
        ///     Reflection to do it.
        ///   </para>
        ///   <para>
        ///     This needs to happen only once, ever. After the settings
        ///     object gets inserted into the array, it's enough for the
        ///     plugin to update its singleton to enable Cropper.Core to
        ///     get the accurate authn information and store it at
        ///     shutdown time.
        ///   </para>
        /// </remarks>
        internal static void BootstrapSettings(object pluginSettingsObject)
        {
            Tracing.Trace("Utils::BootstrapSettings");

            var stype = pluginSettingsObject.GetType();
            var ctype = Type.GetType("Fusion8.Cropper.Core.Configuration, Cropper.Core");

            if (ctype == null)
            {
                Tracing.Trace("Utils::BootstrapSettings no type!");
                return;
            }

            // get the value of the named public static property on the
            // Configuration type.
            var settings = ctype.GetProperty("Current").GetValue(null, null);

            // now, get the value of a public instance property on *that*
            var ps = settings.GetType().GetProperty("PluginSettings");
            object[] psettings = (object[]) ps.GetValue(settings, null);

            if (psettings == null)
            {
                // There are no plugin settings at all.  Seems likely that
                // this is the first time Cropper has been run, ever.
                // Insert a new 1-elt array containing settings for this
                // plugin.
                psettings = new object[1];
                psettings[0] = pluginSettingsObject;
            }
            else
            {
                foreach (object o in psettings)
                {
                    if (o.GetType() == stype)
                        return; // nothing to do
                }
                // create a new array, with all the existing plugin settings
                // instances, along with our settings instance.
                var list = new List<object>(psettings);
                list.Add(pluginSettingsObject);
                psettings = list.ToArray();
            }

            Tracing.Trace("Utils::BootstrapSettings  put PluginSettings");
            // set the value of the PluginSettings property
            ps.SetValue(settings, psettings, null);
        }
    }
}
