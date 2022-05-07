namespace LauncherLite
{
    /// <summary>
    /// Used to download new versions of applications.
    /// </summary>
    public interface INewDownloader
    {
        /// <summary>
        /// Used to download new launcher.
        /// </summary>
        /// <param name="launcherDest">Open stream where new launcher should be saved to.</param>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><c>true</c> if download was successful. Otherwise returns <c>false</c>.</returns>
        Task<bool> DownloadLauncherAsync(Stream launcherDest, CancellationToken cancellationToken);

        /// <summary>
        /// Used to download new application.
        /// </summary>
        /// <param name="applicationDest">Open stream where new application should be saved to.</param>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><c>true</c> if download was successful. Otherwise returns <c>false</c>.</returns>
        Task<bool> DownloadApplicationAsync(Stream applicationDest, CancellationToken cancellationToken);
    }
}