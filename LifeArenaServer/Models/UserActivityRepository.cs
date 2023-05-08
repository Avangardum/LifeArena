using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.Models;

public class UserActivityRepository : IUserActivityRepository
{
    private const string UserActivityDirectoryRelativePath = "UserActivity";

    private readonly string _userActivityDirectoryPath;

    public UserActivityRepository(IFileRepositoryPathProvider fileRepositoryPathProvider)
    {
        _userActivityDirectoryPath = Path.Combine(fileRepositoryPathProvider.FileRepositoryPath, UserActivityDirectoryRelativePath);
        if (!Directory.Exists(_userActivityDirectoryPath))
        {
            Directory.CreateDirectory(_userActivityDirectoryPath);
        }
    }

    public void SaveDailyActiveUsers(DateOnly date, HashSet<string> activeUsers)
    {
        var filePath = GetFilePathForDate(date);
        File.WriteAllLines(filePath, activeUsers);
    }

    public HashSet<string> LoadDailyActiveUsers(DateOnly date)
    {
        var filePath = GetFilePathForDate(date);
        if (!File.Exists(filePath))
        {
            return new HashSet<string>();
        }
        return new HashSet<string>(File.ReadAllLines(filePath));
    }

    private string GetFilePathForDate(DateOnly date) => Path.Combine(_userActivityDirectoryPath, $"{date:yyyy-MMM-dd}.txt");
}