namespace Avangardum.LifeArena.Server.Controllers;

public record GameStateResponse(object PreserializedLivingCells, int Generation, TimeSpan TimeUntilNextGeneration, 
    int CellsLeft, int MaxCellsPerPlayerPerGeneration);