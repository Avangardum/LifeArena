using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;

namespace Avangardum.LifeArena.Server.UnitTests;

[TestFixture]
public class UserActivityManagerTests
{
    private class MockUserActivityRepository : IUserActivityRepository
    {
        public Dictionary<DateOnly, HashSet<string>> ActiveUsersByDate { get; set; } = new();
        public int SaveDailyActiveUsersCallCount { get; private set; }

        public void SaveDailyActiveUsers(DateOnly date, HashSet<string> activeUsers)
        {
            ActiveUsersByDate[date] = activeUsers;
            SaveDailyActiveUsersCallCount++;
        }

        public HashSet<string> LoadDailyActiveUsers(DateOnly date)
        {
            return ActiveUsersByDate.TryGetValue(date, out var activeUsers) ? activeUsers : new HashSet<string>();
        }
    }
    
    private const string User1Id = "John Doe";
    private const string User2Id = "Joe Mama";
    private static readonly DateOnly Day1 = new(2000, 1, 1);
    private static readonly DateOnly Day2 = new(2000, 1, 2);

    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private MockUserActivityRepository _mockUserActivityRepository;
    private IUserActivityManager _userActivityManager;
    #pragma warning restore CS8618

    [SetUp]
    public void Setup()
    {
        _mockUserActivityRepository = new MockUserActivityRepository();
        _userActivityManager = new UserActivityManager(_mockUserActivityRepository);
    }
    
    [Test]
    public void ReportingDifferentUserActivityIncreasesDailyActiveUsersCount()
    {
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(0));
        _userActivityManager.ReportUserActivity(User1Id, Day1);
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(1));
        _userActivityManager.ReportUserActivity(User2Id, Day1);
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(2));
    }
    
    [Test]
    public void ReportingSameUserActivityDoesNotIncreaseDailyActiveUsersCount()
    {
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(0));
        _userActivityManager.ReportUserActivity(User1Id, Day1);
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(1));
        _userActivityManager.ReportUserActivity(User1Id, Day1);
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(1));
    }

    [Test]
    public void ReportingUserActivityAtDifferentDaysIncreasesCorrespondingCounters()
    {
        _userActivityManager.ReportUserActivity(User1Id, Day1);
        _userActivityManager.ReportUserActivity(User2Id, Day1);
        _userActivityManager.ReportUserActivity(User1Id, Day2);
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(2));
        Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day2), Is.EqualTo(1));
    }

    [Test]
    public void ReportingDifferentUserActivitySavesDataToRepository()
    {
        _userActivityManager.ReportUserActivity(User1Id, Day1);
        _userActivityManager.ReportUserActivity(User2Id, Day1);
        _userActivityManager.ReportUserActivity(User1Id, Day2);
        Assert.That(_mockUserActivityRepository.SaveDailyActiveUsersCallCount, Is.EqualTo(3));
        Assert.That(_mockUserActivityRepository.ActiveUsersByDate[Day1].Count, Is.EqualTo(2));
        Assert.That(_mockUserActivityRepository.ActiveUsersByDate[Day2].Count, Is.EqualTo(1));
        _userActivityManager.ReportUserActivity(User1Id, Day2);
        Assert.That(_mockUserActivityRepository.SaveDailyActiveUsersCallCount, Is.EqualTo(3));
    }

    [Test]
    public void LoadsDataFromRepository()
    {
        _mockUserActivityRepository.ActiveUsersByDate = new Dictionary<DateOnly, HashSet<string>>
        {
            { Day1, new HashSet<string> { User1Id, User2Id } },
            { Day2, new HashSet<string> { User1Id } }
        };
        Assert.Multiple(() =>
        {
            Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(2));
            Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day2), Is.EqualTo(1));
        });
        _userActivityManager.ReportUserActivity(User2Id, Day2);
        Assert.Multiple(() =>
        {
            Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day1), Is.EqualTo(2));
            Assert.That(_userActivityManager.GetDailyActiveUsersCount(Day2), Is.EqualTo(2));
        });
    }
}