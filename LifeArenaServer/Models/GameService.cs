using Avangardum.LifeArena.Server.Interfaces;
using Avangardum.LifeArena.Server.Settings;
using Microsoft.Extensions.Options;

namespace Avangardum.LifeArena.Server.Models;

public class GameService : IGameService
{
    private readonly ICoreGameModel _coreGameModel;
    private readonly GameServiceSettings _settings;
    private readonly Dictionary<string, int> _cellsPlacedInThisGenerationCountByPlayer = new();
    private DateTime _lastGenerationStartTime = DateTime.Now;

    public GameService(ICoreGameModel coreGameModel, IOptions<GameServiceSettings> settings)
    {
        _coreGameModel = coreGameModel;
        _settings = settings.Value;
        _ = GenerationLoop();
    }

    public event EventHandler? GenerationChanged;
    
    public bool[,] LivingCells => _coreGameModel.LivingCells;
    public int Generation => _coreGameModel.Generation;
    public int MaxCellsPerPlayerPerGeneration => _settings.MaxCellsPerPlayerPerGeneration;
    public TimeSpan TimeUntilNextGeneration => _lastGenerationStartTime + _settings.NextGenerationInterval - DateTime.Now;
    public TimeSpan NextGenerationInterval => _settings.NextGenerationInterval;

    private TimeSpan TimeFromGenerationChange
    {
        get
        {
            var nextGenerationStartTime = _lastGenerationStartTime + _settings.NextGenerationInterval;
            var timeFromLastGenerationStart = DateTime.Now - _lastGenerationStartTime;
            var timeFromNextGenerationStart = nextGenerationStartTime - DateTime.Now;
            var timeFromGenerationChange = Generation > 0
                ? new List<TimeSpan> { timeFromLastGenerationStart, timeFromNextGenerationStart }.MinBy(t => t)
                : timeFromNextGenerationStart;
            return timeFromGenerationChange;
        }
    }

    public void AddCell(int x, int y, string playerId)
    {
        if (x < 0 || x >= _coreGameModel.LivingCells.GetLength(0) || 
            y < 0 || y >= _coreGameModel.LivingCells.GetLength(1))
        {
            throw new ArgumentOutOfRangeException();
        }

        if (TimeFromGenerationChange <= _settings.MaxTimeFromGenerationChangeToIgnoreAddCell)
        {
            return;
        }
        
        if (_coreGameModel.LivingCells[x, y])
        {
            return;
        }

        _cellsPlacedInThisGenerationCountByPlayer.TryAdd(playerId, 0);
        if (_cellsPlacedInThisGenerationCountByPlayer[playerId] >= _settings.MaxCellsPerPlayerPerGeneration)
        {
            return;
        }
        
        _coreGameModel.SetCellState(x, y, true);
        _cellsPlacedInThisGenerationCountByPlayer[playerId]++;
    }

    public int GetCellsLeftForPlayer(string playerId)
    {
        return _cellsPlacedInThisGenerationCountByPlayer.TryGetValue(playerId, out var cellsPlaced)
            ? _settings.MaxCellsPerPlayerPerGeneration - cellsPlaced
            : _settings.MaxCellsPerPlayerPerGeneration;
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
        _lastGenerationStartTime = DateTime.Now;
        GenerationChanged?.Invoke(this, EventArgs.Empty);
    }
}