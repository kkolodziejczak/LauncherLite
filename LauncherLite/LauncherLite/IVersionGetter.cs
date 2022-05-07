namespace LauncherLite
{
    public interface IVersionGetter
    {
        Task<Version> GetLauncherAsync(CancellationToken cancellationToken);
        Task<Version> GetApplicationAsync(CancellationToken cancellationToken);
    }
}