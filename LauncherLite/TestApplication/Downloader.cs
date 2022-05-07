using LauncherLite;

internal class Downloader : INewDownloader
{
    public Task<bool> DownloadApplicationAsync(Stream destination, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DownloadLauncherAsync(Stream destination, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}