using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;

namespace Avangardum.LifeArena.Server.UnitTests;

[TestFixture]
public class HistoryManagerTests
{
    private class MockGameService : IGameService
    {
        public event EventHandler? GenerationChanged;

        public bool[,] LivingCells { get; } = new bool[100, 100];
        public int Generation { get; set; }
        public int MaxCellsPerPlayerPerTurn => 0;
        public TimeSpan TimeUntilNextGeneration => default;

        public void AddCell(int x, int y, string playerId)
        {
            throw new NotImplementedException();
        }

        public int GetCellsLeftForPlayer(string playerId)
        {
            throw new NotImplementedException();
        }
        
        public void InvokeOnGenerationChanged() => GenerationChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private class MockHistoryRepository : IHistoryRepository
    {
        public int? LastSnapshotGeneration { get; set; }
        public List<GameSnapshot> SaveSnapshotCallHistory { get; } = new();

        public void SaveSnapshot(GameSnapshot snapshot)
        {
            SaveSnapshotCallHistory.Add(snapshot);
        }

        public GameSnapshot LoadSnapshot(int generation)
        {
            return new GameSnapshot(new bool[100, 100], generation);
        }
    }

    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private MockGameService _gameService;
    private MockHistoryRepository _repository;
    private IHistoryManager _historyManager;
    #pragma warning restore CS8618

    [SetUp]
    public void Setup()
    {
        _gameService = new MockGameService();
        _repository = new MockHistoryRepository();
        _historyManager = new HistoryManager(_gameService, _repository);
    }

    [Test]
    public void LastSnapshotGenerationReturnsValueFromRepository([Values(5, 77, 90, null)] int value)
    {
        _repository.LastSnapshotGeneration = value;
        Assert.That(_historyManager.LastSnapshotGeneration, Is.EqualTo(value));
    }

    [Test]
    public void GetSnapshotGetsSnapshotFromRepository([Values(5, 77, 90)] int generation)
    {
        var snapshot = _historyManager.GetSnapshot(generation);
        Assert.That(snapshot.Generation, Is.EqualTo(generation));
    }

    [Test]
    public void SavesSnapshotToRepositoryOnGenerationChanged()
    {
        _gameService.LivingCells[5, 5] = true;
        _gameService.Generation = 4;
        _gameService.InvokeOnGenerationChanged();
        Assert.That(_repository.SaveSnapshotCallHistory, Has.Exactly(1).Items);
        var snapshot = _repository.SaveSnapshotCallHistory.Single();
        Assert.That(snapshot.Generation, Is.EqualTo(_gameService.Generation));
        Assert.That(snapshot.LivingCells, Is.EqualTo(_gameService.LivingCells));
    }
}