namespace Avangardum.LifeArena.Server.Interfaces;

public interface IUserActivityManager
{
    void ReportUserActivity(string userId, DateOnly date);
    int GetDailyActiveUsersCount(DateOnly date);
}