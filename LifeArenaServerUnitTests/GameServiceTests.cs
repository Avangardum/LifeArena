using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.Extensions.Options;

namespace Avangardum.LifeArena.Server.UnitTests;

[TestFixture]
public class GameServiceTests
{
    private const string Player1Id = "John Doe";
    private const string Player2Id = "Joe Mama";
    
    private class MockCoreGameModel : ICoreGameModel
    {
        public bool[,] LivingCells { get; set; } = new bool[100, 100];
        public int Generation { get; set; }
        public List<List<object>> SetCellStateCallHistory { get; } = new();
        public int NextGenerationCallCount { get; private set; }

        public void SetCellState(int x, int y, bool isAlive)
        {
            SetCellStateCallHistory.Add(new List<object> { x, y, isAlive });
            LivingCells[x, y] = isAlive;
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
    
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private MockCoreGameModel _coreGameModel;
    private GameServiceSettings _settings;
    private IGameService _gameService;
    #pragma warning restore CS8618

    [SetUp]
    public void Setup()
    {
        var options = new TestGameServiceSettingsOptions();
        _settings = options.Value;
        _coreGameModel = new MockCoreGameModel();
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
        _gameService.AddCell(5, 7, Player1Id);
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
        for (int i = 0; i < _settings.MaxCellsPerPlayerPerGeneration * 2; i++)
        {
            _gameService.AddCell(i, i, Player1Id);
        }
        Assert.That(_coreGameModel.SetCellStateCallHistory.Count, Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration));
        
        // The second player tries to add more cells than allowed, only the allowed number should be added. 
        // Now there should be 2x the allowed number of cells.
        for (int i = 0; i < _settings.MaxCellsPerPlayerPerGeneration * 2; i++)
        {
            _gameService.AddCell(i, i + 1, Player2Id);
        }
        Assert.That(_coreGameModel.SetCellStateCallHistory.Count, Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration * 2));
        
        // In the next generation, the first player should be able to add the allowed number of cells again.
        // Now there should be 3x the allowed number of cells.
        await Task.Delay(_settings.NextGenerationInterval);
        for (int i = 0; i < _settings.MaxCellsPerPlayerPerGeneration * 2; i++)
        {
            _gameService.AddCell(i, i + 2, Player1Id);
        }
        Assert.That(_coreGameModel.SetCellStateCallHistory.Count, Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration * 3));
    }

    [Test]
    public void GetCellsLeftForPlayerReturnsAmountOfCellsLeft()
    {
        Assert.That(_gameService.GetCellsLeftForPlayer(Player1Id), Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration));
        _gameService.AddCell(0, 0, Player1Id);
        Assert.That(_gameService.GetCellsLeftForPlayer(Player1Id), Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration - 1));
    }
    
    [Test]
    public async Task TimeUntilNextGenerationReturnsTimeUntilNextGeneration()
    {
        Assert.That(_gameService.TimeUntilNextGeneration.TotalSeconds, 
            Is.EqualTo(_settings.NextGenerationInterval.TotalSeconds).Within(10).Percent);
        await Task.Delay(_settings.NextGenerationInterval / 2);
        Assert.That(_gameService.TimeUntilNextGeneration.TotalSeconds, 
            Is.EqualTo(_settings.NextGenerationInterval.TotalSeconds / 2).Within(10).Percent);
        await Task.Delay(_settings.NextGenerationInterval);
        Assert.That(_gameService.TimeUntilNextGeneration.TotalSeconds, 
            Is.EqualTo(_settings.NextGenerationInterval.TotalSeconds / 2).Within(10).Percent);
    }
    
    [Test]
    public async Task GenerationChangedEventIsRaisedOnGenerationChange()
    {
        var wasGenerationChangedEventRaised = false;
        _gameService.GenerationChanged += (_, _) => wasGenerationChangedEventRaised = true;
        await Task.Delay(_settings.NextGenerationInterval * 0.8);
        Assert.That(wasGenerationChangedEventRaised, Is.False);
        await Task.Delay(_settings.NextGenerationInterval * 0.5);
        Assert.That(wasGenerationChangedEventRaised, Is.True);
    }

    [Test]
    public void TryingToAddCellThatIsAlreadyAliveDoesNotDecreaseCellLeft()
    {
        _gameService.AddCell(0, 0, Player1Id);
        _gameService.AddCell(0, 0, Player1Id);
        Assert.That(_gameService.GetCellsLeftForPlayer(Player1Id), Is.EqualTo(_settings.MaxCellsPerPlayerPerGeneration - 1));
    }
    
    [Test]
    public void TryingToAddCellOutsideOfGridThrowsException()
    {
        Assert.That(() => _gameService.AddCell(-1, 0, Player1Id), 
            Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => _gameService.AddCell(0, -1, Player1Id), 
            Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => _gameService.AddCell(_gameService.LivingCells.GetLength(0), 0, Player1Id), 
            Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => _gameService.AddCell(0, _gameService.LivingCells.GetLength(1), Player1Id), 
            Throws.InstanceOf<ArgumentOutOfRangeException>());
    }
}