using Avangardum.LifeArena.Server.Models;

namespace Avangardum.LifeArena.Server.Interfaces;

public interface IHistoryRepository
{
    int? LastSnapshotGeneration { get; }
    
    void SaveSnapshot(GameSnapshot snapshot);
    GameSnapshot LoadSnapshot(int generation);
}