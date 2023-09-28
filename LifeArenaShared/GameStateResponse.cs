namespace Avangardum.LifeArena.Shared;

public record GameStateResponse(List<string> LivingCells, int Generation, TimeSpan TimeUntilNextGeneration, 
    TimeSpan NextGenerationInterval, int CellsLeft, int MaxCellsPerPlayerPerGeneration);