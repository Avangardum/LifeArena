using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.Extensions.Options;

namespace LifeArenaServerTests;

[TestFixture]
public class GameServiceTests
{
    private class StubCoreGameModel : ICoreGameModel
    {
        public bool[,] LivingCells { get; set; } = new bool[100, 100];
        public int Generation { get; set; }
        public List<List<object>> SetCellStateCallHistory { get; } = new();
        public int NextGenerationCallCount { get; private set; }

        public void SetCellState(int x, int y, bool isAlive)
        {
            SetCellStateCallHistory.Add(new List<object> { x, y, isAlive });
        }

        public void NextGeneration()
        {
            NextGenerationCallCount++;
        }
    }
    
    private class TestGameServiceSettingsOptions : IOptions<GameServiceSettings>
    {
        public GameServiceSettings Value { get; } = new GameServiceSettings
        {
            MaxCellsPerPlayerPerGeneration = 10,
            NextGenerationInterval = TimeSpan.FromSeconds(0.1),
        };
    }
    
    private StubCoreGameModel _coreGameModel;
    private GameServiceSettings _settings;
    private IGameService _gameService;

    [SetUp]
    public void Setup()
    {
        var options = new TestGameServiceSettingsOptions();
        _settings = options.Value;
        _coreGameModel = new StubCoreGameModel();
        _gameService = new GameService(_coreGameModel, options);
    }

    [Test]
    public void ReturnsLivingCellsFromCoreGameModel()
    {
        _coreGameModel.LivingCells[0, 0] = true;
        _coreGameModel.LivingCells[1, 1] = true;
        _coreGameModel.LivingCells[2, 2] = true;
        Assert.That(_gameService.LivingCells, Is.EqualTo(_coreGameModel.LivingCells));
    }
    
    [Test]
    public void ReturnsGenerationFromCoreGameModel()
    {
        _coreGameModel.Generation = 123;
        Assert.That(_gameService.Generation, Is.EqualTo(_coreGameModel.Generation));
    }
    
    [Test]
    public void ReturnsMaxCellsPerPlayerPerTurnFromSettings()
    {
        Assert.That(_gameService.MaxCellsPerPlayerPerTurn, Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration));
    }
    
    [Test]
    public void AddCellCallsCoreGameModelSetCellState()
    {
        _gameService.AddCell(5, 7, "John Doe");
        var expectedCallHistory = new List<List<object>>
        {
            new List<object> { 5, 7, true },
        };
        Assert.That(_coreGameModel.SetCellStateCallHistory, Is.EqualTo(expectedCallHistory));
    }
    
    [Test]
    public async Task NextGenerationCalledInIntervals()
    {
        await Task.Delay(_settings.NextGenerationInterval / 2);
        int expectedNextGenerationCallCount = 0;
        for (int i = 0; i < 3; i++)
        {
            await Task.Delay(_settings.NextGenerationInterval);
            expectedNextGenerationCallCount++;
            Assert.That(_coreGameModel.NextGenerationCallCount, Is.EqualTo(expectedNextGenerationCallCount));
        }
    }
    
    [Test]
    public async Task LimitedNumberOfCellsCanBeAddedPerPlayerPerGeneration()
    {
        // The first player tries to add more cells than allowed, only the allowed number should be added.
        await Task.Delay(_settings.NextGenerationInterval / 2);
        var player1Id = "John Doe";
        for (int i = 0; i < _settings.MaxCellsPerPlayerPerGeneration * 2; i++)
        {
            _gameService.AddCell(i, i, player1Id);
        }
        Assert.That(_coreGameModel.SetCellStateCallHistory.Count, Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration));
        
        // The second player tries to add more cells than allowed, only the allowed number should be added. 
        // Now there should be 2x the allowed number of cells.
        var player2Id = "Joe Mama";
        for (int i = 0; i < _settings.MaxCellsPerPlayerPerGeneration * 2; i++)
        {
            _gameService.AddCell(i + 1, i + 1, player2Id);
        }
        Assert.That(_coreGameModel.SetCellStateCallHistory.Count, Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration * 2));
        
        // In the next generation, the first player should be able to add the allowed number of cells again.
        // Now there should be 3x the allowed number of cells.
        await Task.Delay(_settings.NextGenerationInterval);
        for (int i = 0; i < _settings.MaxCellsPerPlayerPerGeneration * 2; i++)
        {
            _gameService.AddCell(i, i, player1Id);
        }
        Assert.That(_coreGameModel.SetCellStateCallHistory.Count, Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration * 3));
    }
}