using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.Models;

public class GameModel : IGameModel
{
    private bool[,] _livingCells;
    private FieldWrapAroundMode _fieldWrapAroundMode;
    
    public GameModel(bool[,] livingCells, int generation, FieldWrapAroundMode fieldWrapAroundMode)
    {
        _livingCells = livingCells;
        Generation = generation;
        _fieldWrapAroundMode = fieldWrapAroundMode;
    }

    public bool[,] LivingCells => (bool[,])_livingCells.Clone();
    public int Generation { get; private set; }

    public void SetCellState(int x, int y, bool isAlive)
    {
        _livingCells[x, y] = isAlive;
    }

    public void NextGeneration()
    {
        Generation++;
        
        var newLivingCells = new bool[_livingCells.GetLength(0), _livingCells.GetLength(1)];
        for (var x = 0; x < _livingCells.GetLength(0); x++)
        {
            for (var y = 0; y < _livingCells.GetLength(1); y++)
            {
                var livingNeighbours = GetLivingNeighboursCount(x, y);
                if (_livingCells[x, y])
                {
                    newLivingCells[x, y] = livingNeighbours is 2 or 3;
                }
                else
                {
                    newLivingCells[x, y] = livingNeighbours is 3;
                }
            }
        }
        _livingCells = newLivingCells;
    }

    private int GetLivingNeighboursCount(int originX, int originY)
    {
        var livingNeighbourCount = 0;
        for (int i = originX - 1; i <= originX + 1; i++)
        {
            for (int j = originY - 1; j <= originY + 1; j++)
            {
                var neighbourX = i;
                var neighbourY = j;
                
                if (neighbourX == originX && neighbourY == originY)
                {
                    continue;
                }

                if (_fieldWrapAroundMode.HasFlag(FieldWrapAroundMode.Horizontal))
                {
                    if (neighbourX == -1)
                    {
                        neighbourX = _livingCells.GetLength(0) - 1;
                    }
                    else if (neighbourX == _livingCells.GetLength(0))
                    {
                        neighbourX = 0;
                    }
                }
                
                if (_fieldWrapAroundMode.HasFlag(FieldWrapAroundMode.Vertical))
                {
                    if (neighbourY == -1)
                    {
                        neighbourY = _livingCells.GetLength(1) - 1;
                    }
                    else if (neighbourY == _livingCells.GetLength(1))
                    {
                        neighbourY = 0;
                    }
                }

                if (neighbourX < 0 || neighbourX >= _livingCells.GetLength(0) || 
                    neighbourY < 0 || neighbourY >= _livingCells.GetLength(1))
                {
                    continue;
                }
                if (_livingCells[neighbourX, neighbourY])
                {
                    livingNeighbourCount++;
                }
            }
        }

        return livingNeighbourCount;
    }
}