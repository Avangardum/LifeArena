namespace Avangardum.LifeArena.Server.Helpers;

public static class TwoDimensionalArrayHelper 
{
    public static List<List<T>> ToListOfLists<T>(this T[,] array)
    {
        var result = new List<List<T>>();
        for (var rowIndex = array.GetLength(1) - 1; rowIndex >= 0; rowIndex--)
        {
            var row = new List<T>();
            for (var columnIndex = 0; columnIndex < array.GetLength(0); columnIndex++)
            {
                row.Add(array[columnIndex, rowIndex]);
            }
            result.Add(row);
        }
        return result;
    }
}