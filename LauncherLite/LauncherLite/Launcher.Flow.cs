using System.Diagnostics;

namespace LauncherLite
{
    public partial class Launcher
    {
        private string _tempApplicationPath
            => Path.Combine(_tempDirectory, _applicationName);

        private string _tempLauncherPath
            => Path.Combine(_tempDirectory, _launcherName);

        /// <summary>
        /// Starts the update flow. If there are any new versions available they will be downloaded.
        /// </summary>
        /// <returns>0 if there were no issues. Otherwise returns 1.</returns>
        public Task<int> StartAsync()
                => StartAsync(CancellationToken.None);

        /// <summary>
        /// Starts the update flow. If there are any new versions available they will be downloaded.
        /// </summary>
        /// <param name="cancellationToken">Used to determine if process should stop.</param>
        /// <returns>0 if there were no issues. Otherwise returns 1.</returns>
        public async Task<int> StartAsync(CancellationToken cancellationToken)
        {
            CheckRequirements();
            if (LauncherWasJustUpdated())
            {
                DeleteTempLauncherFile();
            }

            return await IsLauncherTheNewestVersionAsync(cancellationToken)
                    ? await StartApplicationAsync(cancellationToken)
                    : await DownloadNewLauncherAsync(cancellationToken);
        }

        private void CheckRequirements()
        {
            if (_downloader == null)
            {
                throw new NullReferenceException("You need to set 'downloader' in order to use launcher.");
            }
            if (_versionChecker == null)
            {
                throw new NullReferenceException("You need to set 'version checker' in order to use launcher.");
            }
        }

        private bool LauncherWasJustUpdated()
            => _fileSystem.File.Exists(_tempLauncherPath);

        private bool DeleteTempLauncherFile()
        {
            if (Delete(_tempLauncherPath))
            {
                return true;
            }
            // Delete failed, we are currently the new version.
            if (!Delete(_launcherPath))
            {
                _fileSystem.File.Move(_tempLauncherPath, _launcherPath);
            }
            return _fileSystem.File.Exists(_launcherPath)
               && !_fileSystem.File.Exists(_tempLauncherPath);
        }

        private async Task<bool> IsLauncherTheNewestVersionAsync(CancellationToken cancellationToken)
        {
            var launcherVersion = await _versionGetter.GetLauncherAsync(cancellationToken);
            return await _versionChecker.IsLauncherTheNewestVersionAsync(launcherVersion, cancellationToken);
        }

        private async Task<int> StartApplicationAsync(CancellationToken cancellationToken)
            => await IsApplicationTheNewestVersionAsync(cancellationToken)
                ? StartApplication()
                : await DownloadNewApplicationVersionAsync(cancellationToken);

        private async Task<int> DownloadNewLauncherAsync(CancellationToken cancellationToken)
            => await WasNewLauncherDownloadedAsync(cancellationToken)
                ? StartNewLauncher()
                : ReportError("Error occurred during downloading of the new launcher.");

        private async Task<bool> IsApplicationTheNewestVersionAsync(CancellationToken cancellationToken)
        {
            var applicationVersion = await _versionGetter.GetApplicationAsync(cancellationToken);
            return await _versionChecker.IsApplicationTheNewestVersionAsync(applicationVersion, cancellationToken);
        }

        private async Task<bool> WasNewLauncherDownloadedAsync(CancellationToken cancellationToken)
        {
            using var newLauncherStream = _fileSystem.File.Create(_tempLauncherPath);
            await _downloader.DownloadLauncherAsync(newLauncherStream, cancellationToken);
            return _fileSystem.File.Exists(_tempLauncherPath)
                && IsFileIsNotEmpty(_tempLauncherPath);
        }

        private async Task<int> DownloadNewApplicationVersionAsync(CancellationToken cancellationToken)
            => await WasNewApplicationDownloadedAsync(cancellationToken)
                ? WasOldApplicationReplaced()
                    ? StartApplication()
                    : ReportError("Error occurred during replacing old application with new one.")
                : ReportError("Error occurred during downloading of the new application.");

        private async Task<bool> WasNewApplicationDownloadedAsync(CancellationToken cancellationToken)
        {
            using var newApplicationStream = _fileSystem.File.Create(_tempApplicationPath);
            await _downloader.DownloadApplicationAsync(newApplicationStream, cancellationToken);
            return _fileSystem.File.Exists(_tempApplicationPath)
                && IsFileIsNotEmpty(_tempApplicationPath);
        }

        private bool WasOldApplicationReplaced()
            => Delete(_applicationPath)
                ? Copy(_tempApplicationPath, _applicationPath)
                : false;

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

        private bool IsFileIsNotEmpty(string path)
            => _fileSystem.File.Exists(path)
            && _fileSystem.FileInfo.FromFileName(path).Length > 0;

        private int StartNewLauncher()
            => Process.Start(_tempLauncherPath, new List<string>(Args) { _launcherPathParameter, _launcherPath }) != null
                ? Ok()
                : ReportError("Error while trying to start new launcher.");

        private int StartApplication()
            => Process.Start(_applicationPath, Args) != null
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
}
