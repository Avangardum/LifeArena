using Avangardum.LifeArena.Server.Controllers;
using Avangardum.LifeArena.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LifeArena.Server.Tests;

[TestFixture]
public class GameControllerTests
{
    private class MockGameService : IGameService
    {
        public event EventHandler? GenerationChanged;
        public bool[,] LivingCells => new[,] { { true, false }, { false, true } };
        public int Generation => 42;
        public int MaxCellsPerPlayerPerTurn => 10;
        public TimeSpan TimeUntilNextGeneration => TimeSpan.FromSeconds(5);
        
        public void AddCell(int x, int y, string playerId)
        {
            throw new NotImplementedException();
        }

        public int GetCellsLeftForPlayer(string playerId)
        {
            return playerId == Player1Id ? 5 : 10;
        }
    }
    
    private class MockUserIdProvider : IUserIdProvider
    {
        public string UserId { get; set; } = "Anonymous";
    }
    
    private const string Player1Id = "John Doe";
    private const string Player2Id = "Joe Mama";
    
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private MockGameService _gameService;
    private MockUserIdProvider _userIdProvider;
    private GameController _gameController;
    #pragma warning restore CS8618
    
    [SetUp]
    public void Setup()
    {
        _gameService = new MockGameService();
        _userIdProvider = new MockUserIdProvider();
        _gameController = new GameController(_gameService, _userIdProvider);
    }

    [Test]
    public void GetGameStateReturnsGameStateResponseWithDataFromGameService([Values(Player1Id, Player2Id)] string playerId)
    {
        var expectedResponse = new GameStateResponse(_gameService.LivingCells, _gameService.Generation, 
            _gameService.TimeUntilNextGeneration, _gameService.GetCellsLeftForPlayer(playerId), _gameService.MaxCellsPerPlayerPerTurn);
        _userIdProvider.UserId = playerId;
        var actualResponse = (_gameController.GetState() as OkObjectResult)?.Value as GameStateResponse;
        Assert.That(actualResponse, Is.Not.Null);
        Assert.That(actualResponse!.LivingCells, Is.EqualTo(expectedResponse.LivingCells));
        Assert.That(actualResponse.Generation, Is.EqualTo(expectedResponse.Generation));
        Assert.That(actualResponse.TimeUntilNextGeneration, Is.EqualTo(expectedResponse.TimeUntilNextGeneration));
        Assert.That(actualResponse.CellsLeft, Is.EqualTo(expectedResponse.CellsLeft));
        Assert.That(actualResponse.MaxCellsPerPlayerPerTurn, Is.EqualTo(expectedResponse.MaxCellsPerPlayerPerTurn));
    }
}