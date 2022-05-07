using LauncherLite;

internal class VersionGetter : IVersionGetter
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