namespace LauncherLite
{
    public interface IVersionCheck
    {
        Task<bool> IsLauncherTheNewestVersionAsync(Version current, CancellationToken cancellationToken);
        Task<bool> IsApplicationTheNewestVersionAsync(Version current, CancellationToken cancellationToken);
    }
}