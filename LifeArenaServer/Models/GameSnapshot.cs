namespace Avangardum.LifeArena.Server.Models;

public record GameSnapshot(bool[,] LivingCells, int Generation);