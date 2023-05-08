using Avangardum.LifeArena.Server.Helpers;
using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;

namespace Avangardum.LifeArena.Server.UnitTests;

public class UserActivityRepositoryTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private IFileRepositoryPathProvider _fileRepositoryPathProvider;
    private IUserActivityRepository _userActivityRepository;
    #pragma warning restore CS8618
    
    [SetUp]
    public void Setup()
    {
        _fileRepositoryPathProvider = new FileRepositoryPathProvider();
        if (Directory.Exists(_fileRepositoryPathProvider.FileRepositoryPath))
        {
            Directory.Delete(_fileRepositoryPathProvider.FileRepositoryPath, true);
        }
        _userActivityRepository = new UserActivityRepository(_fileRepositoryPathProvider);
    }

    [Test]
    public void SavesAndLoadsDailyActiveUsers()
    {
        var date = new DateOnly(2000, 1, 1);
        var activeUsers = new HashSet<string> {"John Doe", "Joe Mama"};
        _userActivityRepository.SaveDailyActiveUsers(date, activeUsers);
        var loadedActiveUsers = _userActivityRepository.LoadDailyActiveUsers(date);
        Assert.That(loadedActiveUsers, Is.EquivalentTo(activeUsers));
    }
}