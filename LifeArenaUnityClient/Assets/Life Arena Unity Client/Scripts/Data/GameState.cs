using System;

namespace Avangardum.LifeArena.UnityClient.Data
{
    public record GameState(bool[,] LivingCells, int Generation, TimeSpan TimeUntilNextGeneration, 
        TimeSpan NextGenerationInterval, int CellsLeft, int MaxCellsPerPlayerPerGeneration);
}