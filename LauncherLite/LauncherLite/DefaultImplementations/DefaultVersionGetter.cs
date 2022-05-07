namespace LauncherLite
{
    /// <summary>
    /// Used to get current version.
    /// </summary>
    public interface ICurrentVersionGetter
    {
        /// <summary>
        /// Returns current version of the launcher.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><see cref="Version"/> of the launcher.</returns>
        Task<Version> GetLauncherAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns current version of the application. If the application is not found returns <c>null</c>.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><see cref="Version"/> of the application, if file is not found returns <c>null</c>.</returns>
        Task<Version?> GetApplicationAsync(CancellationToken cancellationToken);
    }

    internal class DefaultVersionGetter : ICurrentVersionGetter
    {
        private readonly string _launcherPath;
        private readonly string _applicationPath;
        private readonly IFileVersionInfo _fileInfo;

        public DefaultVersionGetter(string launcherPath, string applicationPath, IFileVersionInfo fileInfo)
        {
            _launcherPath = launcherPath;
            _applicationPath = applicationPath;
            _fileInfo = fileInfo;
        }

        /// <inheritdoc/>
        public Task<Version?> GetApplicationAsync(CancellationToken cancellationToken)
            => GerVersion(_applicationPath);

        /// <inheritdoc/>
        public Task<Version?> GetLauncherAsync(CancellationToken cancellationToken)
            => GerVersion(_launcherPath);

        private Task<Version?> GerVersion(string path)
        {
            try
            {
                var info = _fileInfo.GetVersionInfo(path);
                if(info.FileVersion == null)
                {
                    // TODO: file didn't contain version.
                    return null;
                }
                return Task.FromResult(new Version(info.FileVersion));
            }
            catch (FileNotFoundException)
            {
                return Task.FromResult<Version?>(null);
            }
        }
    }
}
