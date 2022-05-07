namespace LauncherLite
{
    /// <summary>
    /// Used to check if there is new version available.
    /// </summary>
    public interface IVersionCheck
    {
        /// <summary>
        /// Checks if launcher has new version.
        /// </summary>
        /// <param name="current">Version of the launcher that is running.</param>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><c>true</c> if there is no new version. Otherwise returns <c>false</c>.</returns>
        Task<bool> IsLauncherTheNewestVersionAsync(Version current, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if there is a new version for the application to download.
        /// <para>
        /// Note: <paramref name="current"/> can be null, in that case there is no application locally yet.
        /// </para>
        /// </summary>
        /// <param name="current">Version of the application that is downloaded.</param>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><c>true</c> if there is no new version. Otherwise returns <c>false</c>.</returns>
        Task<bool> IsApplicationTheNewestVersionAsync(Version? current, CancellationToken cancellationToken);
    }
}