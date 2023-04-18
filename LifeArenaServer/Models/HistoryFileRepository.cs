using System.Diagnostics;
using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Shared;
using Newtonsoft.Json.Linq;

namespace Avangardum.LifeArena.Server.Models;

public class HistoryFileRepository : IHistoryRepository
{
    private const string HistoryFileExtension = ".json";
    private const string LivingCellsPropertyName = "livingCells";
    private const string GenerationPropertyName = "generation";

    private readonly ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;
    private readonly string _historyDirectoryPath;

    public HistoryFileRepository(ILivingCellsArrayPreserializer livingCellsArrayPreserializer, 
        IFileRepositoryPathProvider fileRepositoryPathProvider)
    {
        _livingCellsArrayPreserializer = livingCellsArrayPreserializer;
        _historyDirectoryPath = Path.Combine(fileRepositoryPathProvider.FileRepositoryPath, "History");
        
        if (!Directory.Exists(fileRepositoryPathProvider.FileRepositoryPath))
        {
            Directory.CreateDirectory(fileRepositoryPathProvider.FileRepositoryPath);
        }
        if (!Directory.Exists(_historyDirectoryPath))
        {
            Directory.CreateDirectory(_historyDirectoryPath);
        }
    }

    public int? LastSnapshotGeneration
    {
        get
        {
            var snapshotFiles = Directory.GetFiles(_historyDirectoryPath);
            if (!snapshotFiles.Any())
            {
                return null;
            }
            var snapshotFileGenerations = snapshotFiles
                .Select(Path.GetFileNameWithoutExtension)
                .Select(int.Parse!)
                .ToList();
            var lastSnapshotGeneration = snapshotFileGenerations.Max();
            return lastSnapshotGeneration;
        }
    }
    
    public void SaveSnapshot(GameSnapshot snapshot)
    {
        var preserializedLivingCells = _livingCellsArrayPreserializer.Preserialize(snapshot.LivingCells);
        var json = new JObject
        {
            new JProperty(LivingCellsPropertyName, preserializedLivingCells),
            new JProperty(GenerationPropertyName, snapshot.Generation)
        };
        var path = Path.Combine(_historyDirectoryPath, snapshot.Generation + HistoryFileExtension);
        File.WriteAllText(path, json.ToString());
    }

    public GameSnapshot LoadSnapshot(int generation)
    {
        var path = Path.Combine(_historyDirectoryPath, generation + HistoryFileExtension);
        var rawJson = File.ReadAllText(path);
        var json = JObject.Parse(rawJson);
        var preserializedLivingCells = json[LivingCellsPropertyName]?.ToObject<List<string>>();
        Debug.Assert(preserializedLivingCells != null);
        var livingCells = _livingCellsArrayPreserializer.Despreserialize(preserializedLivingCells);
        return new GameSnapshot(livingCells, generation);
    }
}