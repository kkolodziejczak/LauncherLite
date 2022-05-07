namespace LauncherLite
{
    /// <summary>
    /// Used to get current version.
    /// </summary>
    public interface ICurrentVersionGetter
    {
        /// <summary>
        /// Returns current version of the launcher.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><see cref="Version"/> of the launcher.</returns>
        Task<Version> GetLauncherAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Returns current version of the application. If the application is not found returns <c>null</c>.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token that will indicate end of the execution.</param>
        /// <returns><see cref="Version"/> of the application, if file is not found returns <c>null</c>.</returns>
        Task<Version?> GetApplicationAsync(CancellationToken cancellationToken);
    }
}