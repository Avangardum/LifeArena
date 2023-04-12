using Avangardum.LifeArena.Server.Extensions;
using Avangardum.LifeArena.Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Avangardum.LifeArena.Server.Controllers;

public class GameController : ApiController
{
    private IGameService _gameService;
    private IUserIdProvider _userIdProvider;

    public GameController(IGameService gameService, IUserIdProvider userIdProvider)
    {
        _gameService = gameService;
        _userIdProvider = userIdProvider;
    }

    [HttpGet]
    public IActionResult GetState()
    {
        var playerId = _userIdProvider.UserId;
        var response = new GameStateResponse(_gameService.LivingCells.ToListOfLists(), _gameService.Generation, 
            _gameService.TimeUntilNextGeneration, _gameService.GetCellsLeftForPlayer(playerId), 
            _gameService.MaxCellsPerPlayerPerTurn);
        return new OkObjectResult(response);
    }
}