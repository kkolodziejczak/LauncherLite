using System.Diagnostics;

namespace LauncherLite
{
    /// <summary>
    /// Used to get information about file version.
    /// </summary>
    public interface IFileVersionInfo
    {
        /// <summary>
        /// Returns a System.Diagnostics.FileVersionInfo representing the version information
        ///     associated with the specified file.
        /// </summary>
        /// <param name="filePath">The fully qualified path and name of the file to retrieve the version information for.</param>
        /// <returns>A <see cref="FileVersionInfo"/> information about the file. If
        ///     the file did not contain version information, the <see cref="FileVersionInfo"/>
        ///     contains only the name of the file requested.</returns>
        FileVersionInfo GetVersionInfo(string filePath);
    }

    internal class DefaultFileVersionInfo : IFileVersionInfo
    {
        /// <inheritdoc/>
        public FileVersionInfo GetVersionInfo(string filePath)
            => FileVersionInfo.GetVersionInfo(filePath);
    }
}
