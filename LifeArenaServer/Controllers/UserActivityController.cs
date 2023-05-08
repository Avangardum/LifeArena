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
    public int DailyActiveUsers(DateOnly date)
    {
        return _userActivityManager.GetDailyActiveUsersCount(date);
    }
}