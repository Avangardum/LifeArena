using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Avangardum.LifeArena.Server.Controllers;

public class GameController : ApiController
{
    private IGameService _gameService;
    private IUserIdProvider _userIdProvider;
    private ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;
    private IUserActivityManager _userActivityManager;

    public GameController(IGameService gameService, IUserIdProvider userIdProvider, 
        ILivingCellsArrayPreserializer livingCellsArrayPreserializer, IUserActivityManager userActivityManager)
    {
        _gameService = gameService;
        _userIdProvider = userIdProvider;
        _livingCellsArrayPreserializer = livingCellsArrayPreserializer;
        _userActivityManager = userActivityManager;
    }

    [HttpGet]
    public IActionResult GetState()
    {
        var playerId = _userIdProvider.UserId;
        _userActivityManager.ReportUserActivity(playerId, DateOnly.FromDateTime(DateTime.UtcNow));
        var preserializedLivingCells = _livingCellsArrayPreserializer.Preserialize(_gameService.LivingCells);
        var response = new GameStateResponse
        (
            LivingCells: preserializedLivingCells,
            Generation: _gameService.Generation,
            TimeUntilNextGeneration: _gameService.TimeUntilNextGeneration,
            NextGenerationInterval: _gameService.NextGenerationInterval,
            CellsLeft: _gameService.GetCellsLeftForPlayer(playerId),
            MaxCellsPerPlayerPerGeneration: _gameService.MaxCellsPerPlayerPerGeneration
        );
        return new OkObjectResult(response);
    }

    [HttpPut]
    public IActionResult AddCell(int x, int y)
    {
        var playerId = _userIdProvider.UserId;
        try
        {
            _gameService.AddCell(x, y, playerId);
            return GetState();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
}