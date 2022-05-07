using LauncherLite;

static Task<int> Main(string[] args)
{
    var launcherPath = Path.Combine(Directory.GetCurrentDirectory(), args[0]);
    var applicationPath = Path.Combine(launcherPath, "Application");

    var launcher = LauncherConfigurator
        .UseArgs(args)
        .UseApplicationDirectory(applicationPath)
        .UseDownloader(new Downloader())
        .UseVersionChecker(new VersionChecker())
        .Create();

    //TODO: Add status, so that You could display what currently is going on.
    //TODO: check if application path is relative path then combine so it is correct.
    //TODO: Switch process launcher to interface for easier testing.

    return launcher.StartAsync();
}