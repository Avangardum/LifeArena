using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Avangardum.LifeArena.Server.IntegrationTests;

[TestFixture]
public class GameControllerIntegrationTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private HttpClient _httpClient;
    #pragma warning restore CS8618
    
    [SetUp]
    public void Setup()
    {
        var factory = new WebApplicationFactory<Program>();
        _httpClient = factory.CreateClient();
    }
    
    [Test]
    public async Task GetStateReturnsOk()
    {
        var response = await _httpClient.GetAsync("api/game/getState");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}