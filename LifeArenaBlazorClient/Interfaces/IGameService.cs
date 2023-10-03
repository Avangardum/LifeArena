using Avangardum.LifeArena.Shared;

namespace LifeArenaBlazorClient.Interfaces;

public interface IGameService
{
    Task<GameState> GetGameStateAsync();
}