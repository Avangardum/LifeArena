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

    [HttpGet]
    public Dictionary<DateOnly, int> DailyActiveUsersForMonth(DateOnly date)
    {
        return Enumerable.Range(1, DateTime.DaysInMonth(date.Year, date.Month))
            .ToDictionary(day => new DateOnly(date.Year, date.Month, day), 
                day => _userActivityManager.GetDailyActiveUsersCount(new DateOnly(date.Year, date.Month, day)));
    }
}