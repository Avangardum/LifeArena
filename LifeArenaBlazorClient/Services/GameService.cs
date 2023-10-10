using System.Net.Http.Json;
using Avangardum.LifeArena.Shared;
using LifeArenaBlazorClient.Interfaces;

namespace LifeArenaBlazorClient.Services;

public class GameService : IGameService
{
    private const string GameApiUrl = "https://lifearena.avangardum.net/Api/Game/";
    
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
        return GetGameStateFromGameStateResponse(gameStateResponse);
    }

    public async Task<GameState> AddCell(int x, int y)
    {
        var url = $"{GameApiUrl}AddCell?x={x}&y={y}";
        var response = await _httpClient.PutAsync(url, null);
        response.EnsureSuccessStatusCode();
        var gameStateResponse = (await response.Content.ReadFromJsonAsync<GameStateResponse>())!;
        return GetGameStateFromGameStateResponse(gameStateResponse);
    }

    private GameState GetGameStateFromGameStateResponse(GameStateResponse gameStateResponse)
    {
        var livingCells = _livingCellsArrayPreserializer.Depreserialize(gameStateResponse.LivingCells);
        return new GameState(livingCells, gameStateResponse.Generation, gameStateResponse.TimeUntilNextGeneration,
            gameStateResponse.NextGenerationInterval, gameStateResponse.CellsLeft,
            gameStateResponse.MaxCellsPerPlayerPerGeneration);
    }
}