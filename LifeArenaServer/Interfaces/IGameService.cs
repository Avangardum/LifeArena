namespace Avangardum.LifeArena.Server.Interfaces;

public interface IGameService
{
    public event EventHandler GenerationChanged;
    
    bool[,] LivingCells { get; }
    int Generation { get; }
    int MaxCellsPerPlayerPerGeneration { get; }
    TimeSpan TimeUntilNextGeneration { get; }
    TimeSpan NextGenerationInterval { get; }
    
    void AddCell(int x, int y, string playerId);
    int GetCellsLeftForPlayer(string playerId);
}