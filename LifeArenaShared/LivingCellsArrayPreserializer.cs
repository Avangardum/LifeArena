using System.Text;

namespace Avangardum.LifeArena.Shared;

public class LivingCellsArrayPreserializer : ILivingCellsArrayPreserializer
{
    const char LivingCellChar = '0';
    const char DeadCellChar = '.';
    
    public List<string> Preserialize(bool[,] livingCells)
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

    public bool[,] Depreserialize(List<string> preserializedLivingCells)
    {
        var result = new bool[preserializedLivingCells[0].Length, preserializedLivingCells.Count];
        for (var y = 0; y < preserializedLivingCells.Count; y++)
        {
            var row = preserializedLivingCells[preserializedLivingCells.Count - 1 - y];
            for (var x = 0; x < row.Length; x++)
            {
                result[x, y] = row[x] == LivingCellChar;
            }
        }
        return result;
    }
}