using Avangardum.LifeArena.Shared;

namespace LifeArenaBlazorClient.Interfaces;

public interface IGameService
{
    Task<GameState> GetGameStateAsync();
    Task<GameState> AddCell(int x, int y);
}