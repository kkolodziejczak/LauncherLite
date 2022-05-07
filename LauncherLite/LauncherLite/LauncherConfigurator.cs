using Serilog;
using System.IO.Abstractions;
using static LauncherLite.LauncherConfigurator;

namespace LauncherLite
{
    /// <summary>
    /// Used to create configured <see cref="Launcher"/>.
    /// </summary>
    public class LauncherConfigurator : IApplicationPathsConfiguration,
            IVersionCheckConfiguration,
            IDownloadConfiguration,
            IOptionalConfiguration
    {
        private readonly string[] _args;
        private string _launcherPath;
        private string _applicationPath;
        private IFileSystem _fileSystem;
        private IProcessService _processService;
        private INewDownloader _downloader;
        private IVersionCheck _checker;
        private ILogger? _logger;
        private ICurrentVersionGetter? _versionGetter;

        internal LauncherConfigurator(string[] args)
        {
            _args = args;
            _fileSystem = new FileSystem();
            _processService = new DefaultProcessService();
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
            _downloader = downloader;
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
            _checker = checker;
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
            /// Overrides default process service.
            /// </summary>
            /// <param name="processService">Process service used to start new processes.</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="processService"/> is null.</exception>
            IOptionalConfiguration UseProcessService(IProcessService processService);

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
            if (string.IsNullOrEmpty(_launcherPath))
            {
                //TODO: If path is relative?
                _launcherPath =
                    _fileSystem.Path.Combine(
                        _fileSystem.Directory.GetCurrentDirectory(),
                        _processService.GetCurrentProcess().StartInfo.FileName);
            }
            var launcher = new Launcher(_args, _launcherPath, _applicationPath, _fileSystem);
            launcher.UseProcessService(_processService);
            launcher.UseDownloader(_downloader);
            launcher.UseVersionChecker(_checker);

            if (_logger != null)
            {
                launcher.UseLogger(_logger);
            }

            if (_versionGetter != null)
            {
                launcher.UseVersionGetter(_versionGetter);
            }

            return launcher;
        }

        /// <inheritdoc/>
        public IOptionalConfiguration UseLogger(ILogger logger)
        {
            _logger = logger;
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
            _versionGetter = versionGetter;
            return this;
        }

        /// <inheritdoc/>
        public IOptionalConfiguration UseProcessService(IProcessService processService)
        {
            _processService = processService;
            return this;
        }
    }
}
