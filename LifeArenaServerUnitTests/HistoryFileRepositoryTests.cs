using Avangardum.LifeArena.Server.Helpers;
using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;
using Newtonsoft.Json.Linq;

namespace Avangardum.LifeArena.Server.UnitTests;

[TestFixture]
public class HistoryFileRepositoryTests
{
    private const string HistoryDirectoryPath = "History";
    private const string HistoryFileExtension = ".json";

    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;
    private IHistoryRepository _repository;
    #pragma warning restore CS8618

    [SetUp]
    public void SetUp()
    {
        _livingCellsArrayPreserializer = new LivingCellsArrayPreserializer();
        _repository = new HistoryFileRepository(_livingCellsArrayPreserializer);
        
        if (Directory.Exists(HistoryDirectoryPath))
        {
            Directory.Delete(HistoryDirectoryPath);
        }
    }
    
    [Test]
    public void SavesSnapshotToFile()
    {
        var livingCells = new bool[2, 2];
        livingCells[1, 1] = true;
        var generation = 7;
        var snapshot = new GameSnapshot(livingCells, generation);
        _repository.SaveSnapshot(snapshot);
        var filePath = Path.Combine(HistoryDirectoryPath, generation.ToString(), HistoryFileExtension);
        var rawJson = File.ReadAllText(filePath).Replace(" ", "").Replace("\n", "");
        var json = new JObject(rawJson);
        var fileLivingCells = json["livingCells"]?.ToObject<List<string>>();
        var fileGeneration = json["generation"]?.ToObject<int>();
        var expectedFileLivingCells = new List<string> { ".0", ".." };
        Assert.That(fileLivingCells, Is.EqualTo(expectedFileLivingCells));
        Assert.That(fileGeneration, Is.EqualTo(generation));
    }
}