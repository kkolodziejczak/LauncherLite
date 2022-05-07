namespace LauncherLite
{
    public interface INewDownloader
    {
        Task<bool> DownloadLauncherAsync(Stream destination, CancellationToken cancellationToken);
        Task<bool> DownloadApplicationAsync(Stream destination, CancellationToken cancellationToken);
        //TODO: how to know where it was saved/downloaded?? unpackted etc?
    }
}