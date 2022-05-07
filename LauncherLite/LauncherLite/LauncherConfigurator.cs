using Serilog;
using System.Diagnostics;
using System.IO.Abstractions;
using static LauncherLite.LauncherConfigurator;

namespace LauncherLite
{
    /// <summary>
    /// Used to create configured <see cref="Launcher"/>.
    /// </summary>
    public class LauncherConfigurator : IDownloadConfiguration,
            IVersionCheckConfiguration,
            IOptionalConfiguration,
            IApplicationPathsConfiguration
    {
        private readonly Launcher _launcher;
        private readonly string[] _args;
        private string _launcherPath;
        private string _applicationPath;
        private static IFileSystem _fileSystem;

        internal LauncherConfigurator(string[] args)
        {
            _args = args;
            _fileSystem = new FileSystem();
            _launcherPath =
                _fileSystem.Path.Combine(
                    _fileSystem.Directory.GetCurrentDirectory(),
                    Process.GetCurrentProcess().StartInfo.FileName);
        }

        /// <summary>
        /// Specifies <paramref name="args"/> used to start the application.
        /// </summary>
        /// <param name="fileSystem">File system to use in the launcher flow.</param>
        /// <returns></returns>
        public static IApplicationPathsConfiguration UseArgs(string[] args)
            => new LauncherConfigurator(args);

        public interface IApplicationPathsConfiguration
        {
            /// <summary>
            /// Specifies where application is.
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            IDownloadConfiguration UseApplicationDirectory(string path);
        }

        /// <inheritdoc/>
        public IDownloadConfiguration UseApplicationDirectory(string path)
        {
            _applicationPath = path;
            return this;
        }

        public interface IDownloadConfiguration
        {
            /// <summary>
            /// Sets downloader to use during the flow.
            /// </summary>
            /// <param name="downloader">Downloader to use to download new versions of the applications.</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="downloader"/> is null.</exception>
            IVersionCheckConfiguration UseDownloader(INewDownloader downloader);
        }

        /// <inheritdoc/>
        public IVersionCheckConfiguration UseDownloader(INewDownloader downloader)
        {
            _launcher.UseDownloader(downloader);
            return this;
        }

        public interface IVersionCheckConfiguration
        {
            /// <summary>
            /// Sets version checker.
            /// </summary>
            /// <param name="checker">Checker used to check for new versions.</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="checker"/> is null.</exception>
            IOptionalConfiguration UseVersionChecker(IVersionCheck checker);
        }

        /// <inheritdoc/>
        public IOptionalConfiguration UseVersionChecker(IVersionCheck checker)
        {
            _launcher.UseVersionChecker(checker);
            return this;
        }

        public interface IOptionalConfiguration
        {
            /// <summary>
            /// Sets logger to use.
            /// </summary>
            /// <param name="logger">Logger to use during the flow.</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
            IOptionalConfiguration UseLogger(ILogger logger);

            /// <summary>
            /// Specifies file system to use.
            /// </summary>
            /// <param name="fileSystem"></param>
            /// <returns></returns>
            IOptionalConfiguration UseFileSystem(IFileSystem fileSystem);

            /// <summary>
            /// Overrides default version getter.
            /// </summary>
            /// <param name="versionGetter">Getter used to get current application versions.</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="versionGetter"/> is null.</exception>
            IOptionalConfiguration UseVersionGetter(ICurrentVersionGetter versionGetter);

            /// <summary>
            /// Overrides launcher path to use.
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            IOptionalConfiguration UseLauncherFilePath(string path);

            /// <summary>
            /// Finalizes configuration process creating <see cref="Launcher"/> instance.
            /// </summary>
            /// <returns></returns>
            Launcher Create();
        }

        /// <inheritdoc/>
        public Launcher Create()
        {
            return new Launcher(_args, _launcherPath, _applicationPath, _fileSystem);
        }

        /// <inheritdoc/>
        public IOptionalConfiguration UseLogger(ILogger logger)
        {
            _launcher.UseLogger(logger);
            return this;
        }

        /// <inheritdoc/>
        public IOptionalConfiguration UseFileSystem(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            return this;
        }

        /// <inheritdoc/>
        IOptionalConfiguration IOptionalConfiguration.UseLauncherFilePath(string path)
        {
            _launcherPath = path;
            return this;
        }

        /// <inheritdoc/>
        public IOptionalConfiguration UseVersionGetter(ICurrentVersionGetter versionGetter)
        {
            _launcher.UseVersionGetter(versionGetter);
            return this;
        }
    }
}
