namespace Avangardum.LifeArena.Server.Interfaces;

public interface IGameModel
{
    bool[,] LivingCells { get; }
    int Generation { get; }
    void SetCellState(int x, int y, bool isAlive);
    void NextGeneration();
}