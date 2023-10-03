namespace Avangardum.LifeArena.Shared;

public record GameState(bool[,] LivingCells, int Generation, TimeSpan TimeUntilNextGeneration, 
    TimeSpan NextGenerationInterval, int CellsLeft, int MaxCellsPerPlayerPerGeneration);