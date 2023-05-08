using Avangardum.LifeArena.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Avangardum.LifeArena.Server.Controllers;

public class UserActivityController : ApiController
{
    private IUserActivityManager _userActivityManager;

    public UserActivityController(IUserActivityManager userActivityManager)
    {
        _userActivityManager = userActivityManager;
    }

    [HttpGet]
    public int DailyActiveUsers()
    {
        return _userActivityManager.GetDailyActiveUsersCount(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}