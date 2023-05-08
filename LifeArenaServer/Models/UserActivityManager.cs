using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.Models;

public class UserActivityManager : IUserActivityManager
{
    private IUserActivityRepository _userActivityRepository;
    
    private readonly Dictionary<DateOnly, HashSet<string>> _activeUsersByDate = new();

    public UserActivityManager(IUserActivityRepository userActivityRepository)
    {
        _userActivityRepository = userActivityRepository;
    }

    public void ReportUserActivity(string userId, DateOnly date)
    {
        LoadUserActivityForDateIfNecessary(date);
        var wasSetChanged = _activeUsersByDate[date].Add(userId);
        if (wasSetChanged)
        {
            _userActivityRepository.SaveDailyActiveUsers(date, _activeUsersByDate[date]);
        }
    }

    public int GetDailyActiveUsersCount(DateOnly date)
    {
        LoadUserActivityForDateIfNecessary(date);
        return _activeUsersByDate[date].Count;
    }

    private void LoadUserActivityForDateIfNecessary(DateOnly date)
    {
        if (!_activeUsersByDate.ContainsKey(date))
        {
            _activeUsersByDate[date] = _userActivityRepository.LoadDailyActiveUsers(date);
        }
    }
}