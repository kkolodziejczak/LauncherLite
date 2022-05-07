using System.Diagnostics;

namespace LauncherLite
{
    /// <summary>
    /// Used to handle processes.
    /// </summary>
    public interface IProcessService
    {
        /// <summary>
        /// Starts a process resource by specifying the name of the application and a set of command line arguments.
        /// </summary>
        /// <param name="fileName">The name of a document or application file to run in the process.</param>
        /// <param name="args">The command-line arguments to pass when starting the process.</param>
        /// <returns>A new <see cref="Process"/> that is associated with a process resource, or <see cref="null"/> if no process resource is started.</returns>
        Process? Start(string fileName, IEnumerable<string> args);

        /// <summary>
        /// Gets a new <see cref="Process"/> component and associates it with the currently active process.
        /// </summary>
        /// <returns>A new <see cref="Process"/> component associated with the process resource that is running the calling application.</returns>
        Process GetCurrentProcess();
    }

    internal class DefaultProcessService : IProcessService
    {
        /// <inheritdoc/>
        public Process? Start(string fileName, IEnumerable<string> args)
            => Process.Start(fileName, args);

        /// <inheritdoc/>
        public Process GetCurrentProcess()
            => Process.GetCurrentProcess();
    }
}
