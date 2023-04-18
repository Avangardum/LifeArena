using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Avangardum.LifeArena.Server.Controllers;

public class GameController : ApiController
{
    private IGameService _gameService;
    private IUserIdProvider _userIdProvider;
    private ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;

    public GameController(IGameService gameService, IUserIdProvider userIdProvider, 
        ILivingCellsArrayPreserializer livingCellsArrayPreserializer)
    {
        _gameService = gameService;
        _userIdProvider = userIdProvider;
        _livingCellsArrayPreserializer = livingCellsArrayPreserializer;
    }

    [HttpGet]
    public IActionResult GetState()
    {
        var playerId = _userIdProvider.UserId;
        var preserializedLivingCells = _livingCellsArrayPreserializer.Preserialize(_gameService.LivingCells);
        var response = new GameStateResponse(preserializedLivingCells, _gameService.Generation, 
            _gameService.TimeUntilNextGeneration, _gameService.GetCellsLeftForPlayer(playerId), 
            _gameService.MaxCellsPerPlayerPerTurn);
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