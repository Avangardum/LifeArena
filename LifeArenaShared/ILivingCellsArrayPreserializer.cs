namespace Avangardum.LifeArena.Shared;

/// <summary>
/// Converts a 2D array of booleans to a serializable list of strings and vice versa.
/// </summary>
public interface ILivingCellsArrayPreserializer
{
    List<string> Preserialize(bool[,] livingCells);
    bool[,] Depreserialize(List<string> preserializedLivingCells);
}