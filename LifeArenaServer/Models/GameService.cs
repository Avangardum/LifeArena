using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.Extensions.Options;

namespace Avangardum.LifeArena.Server.Models;

public class GameService : IGameService
{
    private readonly ICoreGameModel _coreGameModel;
    private readonly GameServiceSettings _settings;
    private readonly Task _generationLoopTask;
    private readonly Dictionary<string, int> _cellsPlacedInThisGenerationCountByPlayer = new();

    public GameService(ICoreGameModel coreGameModel, IOptions<GameServiceSettings> settings)
    {
        _coreGameModel = coreGameModel;
        _settings = settings.Value;
        _generationLoopTask = GenerationLoop();
    }

    public bool[,] LivingCells => _coreGameModel.LivingCells;
    
    public int Generation => _coreGameModel.Generation;
    
    public int MaxCellsPerPlayerPerTurn => _settings.MaxCellsPerPlayerPerGeneration;
    
    public void AddCell(int x, int y, string playerId)
    {
        if (!_cellsPlacedInThisGenerationCountByPlayer.ContainsKey(playerId))
        {
            _cellsPlacedInThisGenerationCountByPlayer[playerId] = 0;
        }
        if (_cellsPlacedInThisGenerationCountByPlayer[playerId] >= _settings.MaxCellsPerPlayerPerGeneration)
        {
            return;
        }
        
        _coreGameModel.SetCellState(x, y, true);
        _cellsPlacedInThisGenerationCountByPlayer[playerId]++;
    }
    
    private async Task GenerationLoop()
    {
        while (true)
        {
            await Task.Delay(_settings.NextGenerationInterval);

            NextGeneration();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private void NextGeneration()
    {
        _coreGameModel.NextGeneration();
        _cellsPlacedInThisGenerationCountByPlayer.Clear();
    }
}