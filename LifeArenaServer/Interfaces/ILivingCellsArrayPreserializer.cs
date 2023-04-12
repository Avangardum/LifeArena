namespace Avangardum.LifeArena.Server.Interfaces;

/// <summary>
/// Converts a 2D array of booleans to a serializable object and vice versa.
/// </summary>
public interface ILivingCellsArrayPreserializer
{
    object Preserialize(bool[,] livingCells);
    bool[,] Despreserialize(object preserializedLivingCells);
}