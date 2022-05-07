using LauncherLite;

internal class VersionChecker : IVersionCheck
{
    public Task<bool> IsApplicationTheNewestVersionAsync(Version current, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsLauncherTheNewestVersionAsync(Version current, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}