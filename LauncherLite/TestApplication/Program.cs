using LauncherLite;
using System.IO.Abstractions;

static Task<int> Main(string[] args)
{
    return new Launcher("./", "./")
        .UseVersionGetter(new VersionGetter())
        .UseVersionChecker(new VersionChecker())
        .StartAsync();
}