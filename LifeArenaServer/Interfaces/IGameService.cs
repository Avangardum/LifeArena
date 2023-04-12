namespace Avangardum.LifeArena.Server.Interfaces;

public interface IGameService
{
    public event EventHandler GenerationChanged;
    
    bool[,] LivingCells { get; }
    int Generation { get; }
    int MaxCellsPerPlayerPerTurn { get; }
    TimeSpan TimeUntilNextGeneration { get; }
    
    void AddCell(int x, int y, string playerId);
    int GetCellsLeftForPlayer(string playerId);
}