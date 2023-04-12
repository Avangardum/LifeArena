using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.Extensions.Options;

namespace Avangardum.LifeArena.Server.Models;

public class CoreGameModelFactory
{
    private CoreGameModelSettings _coreGameModelSettings;
    
    public CoreGameModelFactory(IOptions<CoreGameModelSettings> coreGameModelSettingsOptions)
    {
        _coreGameModelSettings = coreGameModelSettingsOptions.Value;
    }
    
    public ICoreGameModel CreateCoreGameModel()
    {
        return CreateNewCoreGameModel();
    }

    private ICoreGameModel CreateNewCoreGameModel()
    {
        var livingCells = new bool[_coreGameModelSettings.FieldWidth, _coreGameModelSettings.FieldHeight];
        return new CoreGameModel(livingCells, 0, _coreGameModelSettings.FieldWrapAroundMode);
    }
}