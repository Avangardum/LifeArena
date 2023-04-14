using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.Extensions.Options;

namespace Avangardum.LifeArena.Server.Models;

public class CoreGameModelFactory : ICoreGameModelFactory
{
    private CoreGameModelSettings _coreGameModelSettings;
    private IHistoryRepository _historyRepository;
    
    public CoreGameModelFactory(IOptions<CoreGameModelSettings> coreGameModelSettingsOptions, IHistoryRepository historyRepository)
    {
        _coreGameModelSettings = coreGameModelSettingsOptions.Value;
        _historyRepository = historyRepository;
    }
    
    public ICoreGameModel CreateCoreGameModel()
    {
        if (_historyRepository.LastSnapshotGeneration is { } lastSnapshotGeneration)
            return LoadGame(lastSnapshotGeneration);
        else
            return NewGame();
    }

    private ICoreGameModel NewGame()
    {
        var livingCells = new bool[_coreGameModelSettings.FieldWidth, _coreGameModelSettings.FieldHeight];
        return new CoreGameModel(livingCells, 0, _coreGameModelSettings.FieldWrapAroundMode);
    }
    
    private ICoreGameModel LoadGame(int generation)
    {
        var snapshot = _historyRepository.LoadSnapshot(generation);
        return new CoreGameModel(snapshot.LivingCells, snapshot.Generation, _coreGameModelSettings.FieldWrapAroundMode);
    }
}