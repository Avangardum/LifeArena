namespace Avangardum.LifeArena.Server.Extensions;

public static class TwoDimensionalArrayExtensions 
{
    public static List<List<T>> ToListOfLists<T>(this T[,] array)
    {
        var result = new List<List<T>>();
        for (var rowIndex = 0; rowIndex < array.GetLength(1); rowIndex++)
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