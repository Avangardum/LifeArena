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
            return 42;
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
    public void GetsDailyActiveUsersCountFromManager()
    {
        Assert.That(_userActivityController.DailyActiveUsers(), Is.EqualTo(42));
    }
}