using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Models;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.Extensions.Options;

namespace Avangardum.LifeArena.Server.UnitTests;

[TestFixture]
public class CoreGameModelFactoryTests
{
    private class MockHistoryRepository : IHistoryRepository
    {
        public int? LastSnapshotGeneration { get; set; }
        
        public void SaveSnapshot(GameSnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public GameSnapshot LoadSnapshot(int generation)
        {
            var livingCells = new bool[100, 100];
            livingCells[generation, generation] = true;
            return new GameSnapshot(livingCells, generation);
        }
    }
    
    private class MockCoreGameModelSettingsOptions : IOptions<CoreGameModelSettings>
    {
        public CoreGameModelSettings Value { get; set; } = new CoreGameModelSettings
        {
            FieldWidth = 100,
            FieldHeight = 100,
            FieldWrapAroundMode = FieldWrapAroundMode.None
        };
    }
    
    #pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private MockHistoryRepository _historyRepository;
    private ICoreGameModelFactory _coreGameModelFactory;
    #pragma warning restore CS8618
    
    [SetUp]
    public void SetUp()
    {
        var coreGameModelSettingsOptions = new MockCoreGameModelSettingsOptions();
        _historyRepository = new MockHistoryRepository();
        _coreGameModelFactory = new CoreGameModelFactory(coreGameModelSettingsOptions, _historyRepository);
    }
    
    [Test]
    public void StartNewGameIfHistoryIsEmpty()
    {
        _historyRepository.LastSnapshotGeneration = null;
        var coreGameModel = _coreGameModelFactory.CreateCoreGameModel();
        Assert.That(coreGameModel.LivingCells, Is.All.False);
        Assert.That(coreGameModel.Generation, Is.EqualTo(0));
    }
    
    [Test]
    public void ContinuesLastSavedGameIfHistoryIsNotEmpty()
    {
        _historyRepository.LastSnapshotGeneration = 7;
        var coreGameModel = _coreGameModelFactory.CreateCoreGameModel();
        Assert.That(coreGameModel.LivingCells, Has.Exactly(1).True);
        Assert.That(coreGameModel.LivingCells[7, 7], Is.True);
        Assert.That(coreGameModel.Generation, Is.EqualTo(7));
    }
}