namespace Avangardum.LifeArena.Server.Controllers;

public record GameStateResponse(object LivingCells, int Generation, TimeSpan TimeUntilNextGeneration, 
    int CellsLeft, int MaxCellsPerPlayerPerGeneration);