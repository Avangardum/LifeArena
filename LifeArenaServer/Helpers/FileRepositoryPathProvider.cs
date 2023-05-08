using System.Diagnostics;
using System.Runtime.InteropServices;
using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.Helpers;

public class FileRepositoryPathProvider : IFileRepositoryPathProvider
{
    public FileRepositoryPathProvider()
    {
        string fileRepositoryParentPath;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            fileRepositoryParentPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var homePath = Environment.GetEnvironmentVariable("HOME");
            Debug.Assert(homePath != null);
            fileRepositoryParentPath = Path.Combine(homePath, "AppData");
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
        
        FileRepositoryPath = Path.Combine(fileRepositoryParentPath, "LifeArenaServer");
    }
    
    public string FileRepositoryPath { get; }
}