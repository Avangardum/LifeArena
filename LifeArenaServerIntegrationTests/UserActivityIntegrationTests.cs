using Avangardum.LifeArena.Server.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Avangardum.LifeArena.Server.IntegrationTests;

public class UserActivityIntegrationTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private HttpClient _httpClient;
    #pragma warning restore CS8618
    
    [SetUp]
    public void Setup()
    {
        var fileRepositoryPath = new FileRepositoryPathProvider().FileRepositoryPath;
        if (Directory.Exists(fileRepositoryPath))
        {
            Directory.Delete(fileRepositoryPath, true);
        }
        var serverFactory = new WebApplicationFactory<Program>();
        _httpClient = serverFactory.CreateClient();
    }

    [Test]
    public async Task DailyActiveUsersIncreasesAfterGetGameState()
    {
        var dailyActiveUsers = await GetDailyActiveUsers();
        Assert.That(dailyActiveUsers, Is.EqualTo(0));
        await _httpClient.GetAsync("Api/Game/GetState");
        dailyActiveUsers = await GetDailyActiveUsers();
        Assert.That(dailyActiveUsers, Is.EqualTo(1));
    }

    private async Task<int> GetDailyActiveUsers()
    {
        var response = await _httpClient.GetAsync("Api/UserActivity/DailyActiveUsers");
        var text = await response.Content.ReadAsStringAsync();
        return int.Parse(text);
    }
}