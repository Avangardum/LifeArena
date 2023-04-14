using System.Net;
using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Avangardum.LifeArena.Server.IntegrationTests;

[TestFixture]
public class GameIntegrationTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private WebApplicationFactory<Program> _serverFactory;
    private HttpClient _httpClient;
    private GameServiceSettings _gameServiceSettings;
    private IGameService _gameService;
    #pragma warning restore CS8618
    
    [SetUp]
    public void Setup()
    {
        _serverFactory = new WebApplicationFactory<Program>();
        _httpClient = _serverFactory.CreateClient();
        _gameServiceSettings = _serverFactory.Services.GetRequiredService<IOptions<GameServiceSettings>>().Value;
        _gameService = _serverFactory.Services.GetRequiredService<IGameService>();
    }
    
    [Test]
    public async Task GetStateReturnsOk()
    {
        var response = await _httpClient.GetAsync("api/game/getState");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task NextGenerationTimerStartsEvenWithoutRequests()
    {
        var initialDelay = TimeSpan.FromSeconds(0.5);
        await Task.Delay(initialDelay);
        var response = await _httpClient.GetAsync("api/game/getState");
        var rawJson = await response.Content.ReadAsStringAsync();
        var parsedJson = JObject.Parse(rawJson);
        var rawTimeUntilNextGeneration = parsedJson["timeUntilNextGeneration"];
        Assert.That(rawTimeUntilNextGeneration, Is.Not.Null);
        var timeUntilNextGeneration = rawTimeUntilNextGeneration!.ToObject<TimeSpan>();
        var expectedTimeUntilNextGeneration = _gameServiceSettings.NextGenerationInterval - initialDelay;
        Assert.That(timeUntilNextGeneration.TotalSeconds, Is.EqualTo(expectedTimeUntilNextGeneration.TotalSeconds).
            Within(8).Percent);
    }
    
    [Test]
    public async Task CellsLeftAndMaxCellsAreSameAndMatchSettingsInitially()
    {
        var response = await _httpClient.GetAsync("api/game/getState");
        var rawJson = await response.Content.ReadAsStringAsync();
        var parsedJson = JObject.Parse(rawJson);
        var cellsLeft = parsedJson["cellsLeft"]?.ToObject<int>();
        var maxCellsPerPlayerPerGeneration = parsedJson["maxCellsPerPlayerPerGeneration"]?.ToObject<int>();
        var expected = _gameServiceSettings.MaxCellsPerPlayerPerGeneration;
        Assert.That(cellsLeft, Is.EqualTo(expected));
        Assert.That(maxCellsPerPlayerPerGeneration, Is.EqualTo(expected));
    }

    [Test]
    public async Task GameContinuesAfterServerRestart()
    {
        var generation = _gameService.Generation;
        await Task.Delay(_gameServiceSettings.NextGenerationInterval * 1.2);
        Assert.That(_gameService.Generation, Is.EqualTo(generation + 1));
        generation = _gameService.Generation;
        RestartServer();
        Assert.That(_gameService.Generation, Is.EqualTo(generation));
    }

    private void RestartServer()
    {
        _serverFactory.Dispose();
        _serverFactory = new WebApplicationFactory<Program>();
        _httpClient = _serverFactory.CreateClient();
    }
}