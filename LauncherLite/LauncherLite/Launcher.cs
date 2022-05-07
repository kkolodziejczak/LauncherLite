using Serilog;
using System.IO.Abstractions;

namespace LauncherLite
{
    public partial class Launcher
    {
        private const string _launcherPathParameter = "-launcher-path-before-update";

        private readonly string _launcherPath;
        private readonly string _applicationPath;
        private readonly IFileSystem _fileSystem;

        private string _tempApplicationPath;
        private DefaultFileVersionInfo _fileVersion;
        private string _tempLauncherPath;

        private string _tempDirectory;
        private IVersionCheck? _versionChecker;
        private ICurrentVersionGetter _versionGetter;
        private INewDownloader? _downloader;
        private ILogger? _logger;
        private IProcessService _processService;

        /// <summary>
        /// Arguments used during launcher start.
        /// </summary>
        public string[] Args { get; private set; }

        /// <summary>
        /// Used to handle update flow of the application.
        /// </summary>
        /// <param name="launcherPath">Path where launcher is located.</param>
        /// <param name="applicationPath">Path where application should be located.</param>
        /// <param name="fileSystem">File system to use with the launcher.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="launcherPath"/> does not exist.</exception>
        internal Launcher(string[] args, string launcherPath, string applicationPath, IFileSystem fileSystem)
        {
            Args = args;
            _fileSystem = fileSystem;
            _tempDirectory = _fileSystem.Path.GetTempPath();

            if (TryGetOldLauncherPath(Args) is string oldLauncherPath)
            {
                _launcherPath = ThrowIfFileDoesntExist(oldLauncherPath);
            }
            else
            {
                _launcherPath = ThrowIfFileDoesntExist(launcherPath);
            }
            var launcherName = _fileSystem.Path.GetFileName(_launcherPath);
            _tempLauncherPath = _fileSystem.Path.Combine(_tempDirectory, launcherName);

            _applicationPath = applicationPath;
            var applicationName = _fileSystem.Path.GetFileName(_applicationPath);
            _tempApplicationPath = _fileSystem.Path.Combine(_tempDirectory, applicationName);

            _fileVersion = new DefaultFileVersionInfo();
            // TODO: add posibility to override FileVersion.!
            _versionGetter = new DefaultVersionGetter(_launcherPath, _applicationPath, _fileVersion);
            _processService = new DefaultProcessService();
        }

        /// <summary>
        /// Overrides temporary directory that <see cref="Launcher"/> will use during the flow.
        /// </summary>
        /// <param name="path">Path to the new temporary directory.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
        internal Launcher UseTempDirectory(string path)
        {
            _tempDirectory = path ?? throw new ArgumentNullException(nameof(path));
            return this;
        }

        internal Launcher UseLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        internal Launcher UseDownloader(INewDownloader downloader)
        {
            _downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
            return this;
        }

        internal Launcher UseVersionGetter(ICurrentVersionGetter getter)
        {
            _versionGetter = getter ?? throw new ArgumentNullException(nameof(getter));
            return this;
        }

        internal Launcher UseVersionChecker(IVersionCheck checker)
        {
            _versionChecker = checker ?? throw new ArgumentNullException(nameof(checker));
            return this;
        }

        internal Launcher UseProcessService(IProcessService processService)
        {
            _processService = processService ?? throw new ArgumentNullException(nameof(processService));
            return this;
        }

        private string ThrowIfFileDoesntExist(string path)
        {
            if (!_fileSystem.File.Exists(path))
            {
                throw new FileNotFoundException("This file is necessary for the launcher to work correctly.", path);
            }

            return path;
        }

        private string? TryGetOldLauncherPath(string[] args)
        {
            var launcherPathParamIndex = Array.IndexOf(args, _launcherPathParameter);
            if (launcherPathParamIndex == -1)
            {
                return null;
            }

            var path = string.Empty;
            if (args.Length > launcherPathParamIndex)
            {
                path = args[launcherPathParamIndex + 1];
            }

            if (_fileSystem.File.Exists(path))
            {
                //TODO: handle wrong parameters.
            }

            _logger?.Warning("There was last launcher path but there was no value provided");
            return null;
        }

    }
}