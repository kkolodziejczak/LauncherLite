using System.Diagnostics;

namespace LauncherLite
{
    internal class DefaultVersionGetter : ICurrentVersionGetter
    {
        private readonly string _launcherPath;
        private readonly string _applicationPath;

        public DefaultVersionGetter(string launcherPath, string applicationPath)
        {
            _launcherPath = launcherPath;
            _applicationPath = applicationPath;
        }

        public Task<Version?> GetApplicationAsync(CancellationToken cancellationToken)
            => GerVersion(_applicationPath);

        public Task<Version?> GetLauncherAsync(CancellationToken cancellationToken)
            => GerVersion(_launcherPath);

        private Task<Version?> GerVersion(string path)
        {
            try
            {
                var info = FileVersionInfo.GetVersionInfo(path);
                return Task.FromResult(new Version(info.FileVersion ?? "0.0.0"));
            }
            catch (FileNotFoundException)
            {
                return Task.FromResult<Version?>(null);
            }
        }
    }
}
