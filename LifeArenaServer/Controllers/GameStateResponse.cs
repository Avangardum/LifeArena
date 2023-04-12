namespace Avangardum.LifeArena.Server.Controllers;

public record GameStateResponse(List<List<bool>> LivingCells, int Generation, TimeSpan TimeUntilNextGeneration, 
    int CellsLeft, int MaxCellsPerPlayerPerGeneration);