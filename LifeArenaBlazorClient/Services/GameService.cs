using System.Net.Http.Json;
using Avangardum.LifeArena.Shared;
using LifeArenaBlazorClient.Interfaces;

namespace LifeArenaBlazorClient.Services;

public class GameService : IGameService
{
    private const string GameApiUrl = "https://localhost:7206/Api/Game/";
    
    private readonly HttpClient _httpClient;
    private readonly ILivingCellsArrayPreserializer _livingCellsArrayPreserializer;

    public GameService(HttpClient httpClient, ILivingCellsArrayPreserializer livingCellsArrayPreserializer)
    {
        _httpClient = httpClient;
        _livingCellsArrayPreserializer = livingCellsArrayPreserializer;
    }
    
    public async Task<GameState> GetGameStateAsync()
    {
        const string url = GameApiUrl + "GetState";
        var gameStateResponse = (await _httpClient.GetFromJsonAsync<GameStateResponse>(url))!;
        var livingCells = _livingCellsArrayPreserializer.Depreserialize(gameStateResponse.LivingCells);
        return new GameState(livingCells, gameStateResponse.Generation, gameStateResponse.TimeUntilNextGeneration,
            gameStateResponse.NextGenerationInterval, gameStateResponse.CellsLeft,
            gameStateResponse.MaxCellsPerPlayerPerGeneration);
    }
}