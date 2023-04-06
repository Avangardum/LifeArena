namespace Avangardum.LifeArena.Server.Interfaces;

public interface IGameService
{
    public bool[,] LivingCells { get; }
    public int Generation { get; }
    public int MaxCellsPerPlayerPerTurn { get; }
    
    public void AddCell(int x, int y, string playerId);
}