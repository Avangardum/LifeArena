namespace Avangardum.LifeArena.Server.Interfaces;

public interface ICoreGameModel
{
    bool[,] LivingCells { get; }
    int Generation { get; }
    
    void SetCellState(int x, int y, bool isAlive);
    void NextGeneration();
}