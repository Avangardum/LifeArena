using Avangardum.LifeArena.Server.Controllers;
using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.UnitTests;

public class UserActivityControllerTests
{
    private class MockUserActivityManager : IUserActivityManager
    {
        public void ReportUserActivity(string userId, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public int GetDailyActiveUsersCount(DateOnly date)
        {
            return date.Day;
        }
    }
    
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private MockUserActivityManager _userActivityManager;
    private UserActivityController _userActivityController;
    #pragma warning restore CS8618
    
    [SetUp]
    public void SetUp()
    {
        _userActivityManager = new MockUserActivityManager();
        _userActivityController = new UserActivityController(_userActivityManager);
    }
    
    [Test]
    public void GetsDailyActiveUsersCountFromManager([Values(1, 2, 3, 5, 8)] int day)
    {
        var date = new DateOnly(2000, 1, day);
        Assert.That(_userActivityController.DailyActiveUsers(date), Is.EqualTo(date.Day));
    }

    [Test]
    public void GetsDailyActiveUsersCountForEveryDayInMonth()
    {
        var date = new DateOnly(2000, 1, 1);
        var dailyActiveUsersForMonth = _userActivityController.DailyActiveUsersForMonth(date);
        var expectedDailyActiveUsersForMonth = Enumerable.Range(1, 31)
            .ToDictionary(x => new DateOnly(2000, 1, x), day => day);
        Assert.That(dailyActiveUsersForMonth, Is.EqualTo(expectedDailyActiveUsersForMonth));
    }
}