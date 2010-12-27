// IUpload.cs
//
// Part of CropperPlugins.
//
//
// Sun, 26 Dec 2010  17:21
//

namespace CropperPlugins.Common
{
    /// <summary>
    ///     This defines an interface that can be implemented by a
    ///     Plugin class to indicate explicitly to other plugins that it
    ///     can upload files to a service.  These other plugins may wish
    ///     to chain into plugins that can upload.  For example a plugin
    ///     that captures an animated gif may wish to chain to another
    ///     plugin that uploads the animated gif to a remote service.
    /// </summary>
    public interface IUpload
    {
        /// <summary>
        ///   Upload a file by the given name, to the plugin's service.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Typically this method might be called by the plugin
        ///     itself, in the course of its normal use, after it saves
        ///     an image to a file, in other words, from within the
        ///     scope of the ImageCaptured event handler.  However, not
        ///     all plugins that upload, save the image to a file first,
        ///     so this is not always appropriate.
        ///   </para>
        ///   <para>
        ///     This method should verify or validate authentication for
        ///     the photo or file hosting service, before attempting to
        ///     upload.
        ///   </para>
        /// </remarks>
        /// <param name="fileName>
        ///   the name of the file to upload.
        /// </param>
        void UploadFile(string fileName);
    }
}
