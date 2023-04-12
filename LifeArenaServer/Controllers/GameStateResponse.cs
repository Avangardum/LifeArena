namespace Avangardum.LifeArena.Server.Controllers;

public record GameStateResponse(bool[,] LivingCells, int Generation, TimeSpan TimeUntilNextGeneration, int CellsLeft, 
    int MaxCellsPerPlayerPerTurn);