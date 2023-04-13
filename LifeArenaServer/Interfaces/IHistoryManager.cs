using Avangardum.LifeArena.Server.Models;

namespace Avangardum.LifeArena.Server.Interfaces;

public interface IHistoryManager
{
    int LastSnapshotGeneration { get; }
    
    GameSnapshot GetSnapshot(int generation);
}