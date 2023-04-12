using System.Net;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Avangardum.LifeArena.Server.IntegrationTests;

[TestFixture]
public class GameControllerIntegrationTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private HttpClient _httpClient;
    private GameServiceSettings _gameServiceSettings;
    #pragma warning restore CS8618
    
    [SetUp]
    public void Setup()
    {
        var factory = new WebApplicationFactory<Program>();
        _httpClient = factory.CreateClient();
        _gameServiceSettings = factory.Services.GetRequiredService<IOptions<GameServiceSettings>>().Value;
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
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var rawJson = await response.Content.ReadAsStringAsync();
        var parsedJson = JObject.Parse(rawJson);
        var rawTimeUntilNextGeneration = parsedJson["timeUntilNextGeneration"];
        Assert.That(rawTimeUntilNextGeneration, Is.Not.Null);
        var timeUntilNextGeneration = rawTimeUntilNextGeneration!.ToObject<TimeSpan>();
        var expectedTimeUntilNextGeneration = _gameServiceSettings.NextGenerationInterval - initialDelay;
        Assert.That(timeUntilNextGeneration.TotalSeconds, Is.EqualTo(expectedTimeUntilNextGeneration.TotalSeconds).
            Within(8).Percent);
    }
}