namespace Avangardum.LifeArena.Server.Interfaces;

public interface IUserActivityRepository
{
    void SaveDailyActiveUsers(DateOnly date, HashSet<string> activeUsers);
    HashSet<string> LoadDailyActiveUsers(DateOnly date);
}