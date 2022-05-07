using Serilog;
using System.Diagnostics;
using System.IO.Abstractions;

namespace LauncherLite
{
    public class Launcher
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

        private string _tempApplicationPath
            => Path.Combine(_tempDirectory, _applicationName);

        private string _tempLauncherPath
            => Path.Combine(_tempDirectory, _launcherName);

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

        private string ThrowIfFileDoesntExist(string path)
        {
            if (!_fileSystem.File.Exists(path))
            {
                throw new FileNotFoundException("This file is neccessary for the launcher to work correctly.", path);
            }

            return path;
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
        public Task<int> StartAsync()
            => StartAsync(CancellationToken.None);

        public async Task<int> StartAsync(CancellationToken cancellationToken)
        {
            if (LauncherWasJustUpdated())
            {
                UpdateLauncherFile();
            }

            return await IsLauncherTheNewestVersionAsync(cancellationToken)
                    ? await StartApplicationAsync(cancellationToken)
                    : await DownloadNewLauncherVersionAsync(cancellationToken);
        }

        private bool LauncherWasJustUpdated()
        {
            return _fileSystem.File.Exists(_tempLauncherPath);
        }

        private bool UpdateLauncherFile()
        {
            if (Delete(_tempLauncherPath))
            {
                return true;
            }
            if (!Delete(_launcherPath))
            {
                _fileSystem.File.Move(_tempLauncherPath, _launcherPath);
            }
            return _fileSystem.File.Exists(_launcherPath)
               && !_fileSystem.File.Exists(_tempLauncherPath);
        }

        private async Task<bool> IsLauncherTheNewestVersionAsync(CancellationToken cancellationToken)
        {
            var launcherVersion = await _getter.GetLauncherAsync(cancellationToken);
            return await _checker.IsLauncherTheNewestVersionAsync(launcherVersion, cancellationToken);
        }

        private async Task<int> StartApplicationAsync(CancellationToken cancellationToken)
            => await IsApplicationTheNewestVersionAsync(cancellationToken)
                ? LaunchApplication()
                : await DownloadNewApplicationVersionAsync(cancellationToken);

        private async Task<int> DownloadNewLauncherVersionAsync(CancellationToken cancellationToken)
            => await WasNewLauncherDownloadedAsync(cancellationToken)
                ? LaunchNewLauncher()
                : ReportError("Error occured during downloading of the new launcher.");

        private async Task<bool> IsApplicationTheNewestVersionAsync(CancellationToken cancellationToken)
        {
            var applicationVersion = await _getter.GetApplicationAsync(cancellationToken);
            return await _checker.IsApplicationTheNewestVersionAsync(applicationVersion, cancellationToken);
        }

        private async Task<bool> WasNewLauncherDownloadedAsync(CancellationToken cancellationToken)
        {
            using var newLauncherStream = _fileSystem.File.Create(_tempLauncherPath);
            await _downloader.DownloadLauncherAsync(newLauncherStream, cancellationToken);
            return _fileSystem.File.Exists(_tempLauncherPath)
                && IsNotEmpty(_tempLauncherPath);
        }

        private async Task<int> DownloadNewApplicationVersionAsync(CancellationToken cancellationToken)
            => await WasNewApplicationDownloadedAsync(cancellationToken)
                ? ReplaceOldApplication()
                    ? LaunchApplication()
                    : ReportError("Error occured during replacing old application with new one.")
                : ReportError("Error occured during downloading of the new application.");

        private async Task<bool> WasNewApplicationDownloadedAsync(CancellationToken cancellationToken)
        {
            using var newApplicationStream = _fileSystem.File.Create(_tempApplicationPath);
            await _downloader.DownloadApplicationAsync(newApplicationStream, cancellationToken);
            return _fileSystem.File.Exists(_tempApplicationPath)
                && IsNotEmpty(_tempApplicationPath);
        }

        private bool ReplaceOldApplication()
        {
            if (Delete(_applicationPath))
            {
                return Copy(_tempApplicationPath, _applicationPath);
            }

            return false;
        }

        private bool Delete(string path)
        {
            _fileSystem.File.Delete(path);
            return !_fileSystem.File.Exists(path);
        }

        private bool Copy(string source, string destination)
        {
            _fileSystem.File.Copy(source, destination, true);
            return _fileSystem.File.Exists(destination)
               && !_fileSystem.File.Exists(source);
        }

        private bool IsNotEmpty(string path)
            => _fileSystem.FileInfo.FromFileName(path).Length > 0;

        private int LaunchNewLauncher()
            => Process.Start(_tempLauncherPath) != null
                ? Ok()
                : ReportError("Error while trying to start new launcher.");

        private int LaunchApplication()
            => Process.Start(_applicationPath) != null
                ? Ok()
                : ReportError("Error while trying to start application.");

        private int ReportError(string message)
        {
            _logger?.Error(message);
            return 1;
        }

        private int Ok()
                => 0;
    }

    public interface IVersionCheck
    {
        Task<bool> IsLauncherTheNewestVersionAsync(Version current, CancellationToken cancellationToken);
        Task<bool> IsApplicationTheNewestVersionAsync(Version current, CancellationToken cancellationToken);
    }

    public interface IVersionGetter
    {
        Task<Version> GetLauncherAsync(CancellationToken cancellationToken);
        Task<Version> GetApplicationAsync(CancellationToken cancellationToken);
    }

    public interface INewDownloader
    {
        Task<bool> DownloadLauncherAsync(Stream destination, CancellationToken cancellationToken);
        Task<bool> DownloadApplicationAsync(Stream destination, CancellationToken cancellationToken);
        //TODO: how to know where it was saved/downloaded?? unpackted etc?
    }
}