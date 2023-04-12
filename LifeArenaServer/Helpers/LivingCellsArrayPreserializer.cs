using System.Text;
using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.Helpers;

public class LivingCellsArrayPreserializer : ILivingCellsArrayPreserializer
{
    const char LivingCellChar = '1';
    const char DeadCellChar = '0';
    
    public object Preserialize(bool[,] livingCells)
    {
        var result = new List<string>();
        for (var y = livingCells.GetLength(1) - 1; y >= 0; y--)
        {
            var stringBuilder = new StringBuilder();
            for (var x = 0; x < livingCells.GetLength(0); x++)
            {
                if (livingCells[x, y])
                {
                    stringBuilder.Append(LivingCellChar);
                }
                else
                {
                    stringBuilder.Append(DeadCellChar);
                }
            }
            result.Add(stringBuilder.ToString());
        }
        return result;
    }

    public bool[,] Despreserialize(object preserializedLivingCells)
    {
        var preserializedLivingCellsList = (List<string>)preserializedLivingCells;
        var result = new bool[preserializedLivingCellsList[0].Length, preserializedLivingCellsList.Count];
        for (var y = 0; y < preserializedLivingCellsList.Count; y++)
        {
            var row = preserializedLivingCellsList[preserializedLivingCellsList.Count - 1 - y];
            for (var x = 0; x < row.Length; x++)
            {
                result[x, y] = row[x] == LivingCellChar;
            }
        }
        return result;
    }
}