using Avangardum.LifeArena.Server.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Avangardum.LifeArena.Server.Models;

public class HistoryFileRepository : IHistoryRepository
{
    private const string HistoryDirectoryPath = "History";
    private const string HistoryFileExtension = ".json";
    private const string LivingCellsPropertyName = "livingCells";
    private const string GenerationPropertyName = "generation";

    private ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;

    public HistoryFileRepository(ILivingCellsArrayPreserializer livingCellsArrayPreserializer)
    {
        _livingCellsArrayPreserializer = livingCellsArrayPreserializer;
        
        if (!Directory.Exists(HistoryDirectoryPath))
        {
            Directory.CreateDirectory(HistoryDirectoryPath);
        }
    }
    
    public int LastSnapshotGeneration { get; }
    
    public void SaveSnapshot(GameSnapshot snapshot)
    {
        var preserializedLivingCells = _livingCellsArrayPreserializer.Preserialize(snapshot.LivingCells);
        var json = new JObject
        {
            new JProperty(LivingCellsPropertyName, preserializedLivingCells),
            new JProperty(GenerationPropertyName, snapshot.Generation)
        };
        var path = Path.Combine(HistoryDirectoryPath, snapshot.Generation.ToString(), HistoryFileExtension);
        File.WriteAllText(path, json.ToString());
    }

    public GameSnapshot LoadSnapshot(int generation)
    {
        throw new NotImplementedException();
    }
}