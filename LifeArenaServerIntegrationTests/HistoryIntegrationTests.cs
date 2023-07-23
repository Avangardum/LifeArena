using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Avangardum.LifeArena.Server.IntegrationTests;

[TestFixture]
public class HistoryIntegrationTests
{
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private GameServiceSettings _gameServiceSettings;
    private string _historyDirectoryPath;
    #pragma warning restore CS8618

    private string? LastSnapshotFile =>
        Directory.GetFiles(_historyDirectoryPath)
            .Order()
            .LastOrDefault();

            [SetUp]
    public void Setup()
    {
        var factory = new WebApplicationFactory<Program>();
        _gameServiceSettings = factory.Services.GetRequiredService<IOptions<GameServiceSettings>>().Value;
        var fileRepositoryPath = factory.Services.GetRequiredService<IFileRepositoryPathProvider>().FileRepositoryPath;
        _historyDirectoryPath = Path.Combine(fileRepositoryPath, "History");
    }

    [Test]
    public async Task SnapshotIsSavedOnGenerationChanged()
    {
        var initialLastSnapshotFile = LastSnapshotFile;
        await Task.Delay(_gameServiceSettings.NextGenerationInterval * 1.2);
        Assert.That(LastSnapshotFile, Is.Not.EqualTo(initialLastSnapshotFile));
    }
}