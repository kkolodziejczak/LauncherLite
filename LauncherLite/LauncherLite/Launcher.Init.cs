using Serilog;
using System.IO.Abstractions;

namespace LauncherLite
{
    public partial class Launcher
    {
        private string _tempDirectory = Path.GetTempPath();

        private readonly string _launcherPath;
        private readonly string _launcherName;
        private readonly string _applicationPath;
        private readonly string _applicationName;
        private readonly IFileSystem _fileSystem;
        private IVersionCheck? _checker;
        private IVersionGetter? _getter;
        private INewDownloader? _downloader;
        private ILogger? _logger;

        public Launcher(string launcherPath, string applicationPath)
            : this(launcherPath, applicationPath, new FileSystem())
        { }

        public Launcher(string launcherPath, string applicationPath, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _launcherPath = ThrowIfFileDoesntExist(launcherPath);
            _launcherName = Path.GetFileName(launcherPath);
            _applicationPath = ThrowIfFileDoesntExist(applicationPath);
            _applicationName = Path.GetFileName(applicationPath);
        }


        public Launcher UseTempDirectory(string path)
        {
            _tempDirectory = path ?? throw new ArgumentNullException(nameof(path));
            return this;
        }

        public Launcher UseLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        public Launcher UseVersionGetter(INewDownloader downloader)
        {
            _downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
            return this;
        }

        public Launcher UseVersionGetter(IVersionGetter getter)
        {
            _getter = getter ?? throw new ArgumentNullException(nameof(getter));
            return this;
        }

        public Launcher UseVersionChecker(IVersionCheck checker)
        {
            _checker = checker ?? throw new ArgumentNullException(nameof(checker));
            return this;
        }

        private string ThrowIfFileDoesntExist(string path)
        {
            if (!_fileSystem.File.Exists(path))
            {
                throw new FileNotFoundException("This file is neccessary for the launcher to work correctly.", path);
            }

            return path;
        }

    }
}