using LauncherLite;

internal class VersionGetter : ICurrentVersionGetter
{
    public Task<Version> GetApplicationAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Version> GetLauncherAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}