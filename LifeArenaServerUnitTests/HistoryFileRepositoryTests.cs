using Avangardum.LifeArena.Server.Helpers;
using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;
using Newtonsoft.Json.Linq;

namespace Avangardum.LifeArena.Server.UnitTests;

[TestFixture]
public class HistoryFileRepositoryTests
{
    private const string HistoryFileExtension = ".json";

    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;
    private IHistoryRepository _repository;
    private IFileRepositoryPathProvider _fileRepositoryPathProvider;
    private string _historyDirectoryPath;
    #pragma warning restore CS8618

    [SetUp]
    public void SetUp()
    {
        _fileRepositoryPathProvider = new FileRepositoryPathProvider();
        DeleteFileRepository();
        _livingCellsArrayPreserializer = new LivingCellsArrayPreserializer();
        _repository = new HistoryFileRepository(_livingCellsArrayPreserializer, _fileRepositoryPathProvider);
        _historyDirectoryPath = Path.Combine(_fileRepositoryPathProvider.FileRepositoryPath, "History");
    }

    [TearDown]
    public void TearDown()
    {
        DeleteFileRepository();
    }

    private void DeleteFileRepository()
    {
        if (Directory.Exists(_fileRepositoryPathProvider.FileRepositoryPath))
        {
            Directory.Delete(_fileRepositoryPathProvider.FileRepositoryPath, true);
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
        var filePath = Path.Combine(_historyDirectoryPath, generation + HistoryFileExtension);
        var rawJson = File.ReadAllText(filePath).Replace(" ", "").Replace("\n", "");
        var json = JObject.Parse(rawJson);
        var fileLivingCells = json["livingCells"]?.ToObject<List<string>>();
        var fileGeneration = json["generation"]?.ToObject<int>();
        var expectedFileLivingCells = new List<string> { ".0", ".." };
        Assert.That(fileLivingCells, Is.EqualTo(expectedFileLivingCells));
        Assert.That(fileGeneration, Is.EqualTo(generation));
    }

    [Test]
    public void LoadsSnapshotFile()
    {
        var fileLivingCells = new bool[2, 2];
        fileLivingCells[1, 0] = true;
        var fileGeneration = 13;
        var json = new JObject
        {
            new JProperty("livingCells", new List<string> { "..", ".0" }),
            new JProperty("generation", fileGeneration)
        };
        var filePath = Path.Combine(_historyDirectoryPath, fileGeneration + HistoryFileExtension);
        File.WriteAllText(filePath, json.ToString());

        var snapshot = _repository.LoadSnapshot(fileGeneration);
        Assert.That(snapshot.LivingCells, Is.EqualTo(fileLivingCells));
        Assert.That(snapshot.Generation, Is.EqualTo(fileGeneration));
    }

    [Test]
    public void LastSnapshotGenerationReturnsLastSnapshotGeneration()
    {
        File.WriteAllText(Path.Combine(_historyDirectoryPath, "1" + HistoryFileExtension), "");
        File.WriteAllText(Path.Combine(_historyDirectoryPath, "2" + HistoryFileExtension), "");
        File.WriteAllText(Path.Combine(_historyDirectoryPath, "5" + HistoryFileExtension), "");
        
        Assert.That(_repository.LastSnapshotGeneration, Is.EqualTo(5));
    }
    
    [Test]
    public void LastSnapshotGenerationReturnsNullIfNoSnapshots()
    {
        Assert.That(_repository.LastSnapshotGeneration, Is.EqualTo(null));
    }
}