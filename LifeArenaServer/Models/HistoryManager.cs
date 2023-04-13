using Avangardum.LifeArena.Server.Interfaces;

namespace Avangardum.LifeArena.Server.Models;

public class HistoryManager : IHistoryManager
{
    private IGameService _gameService;
    private IHistoryRepository _repository;

    public HistoryManager(IGameService gameService, IHistoryRepository repository)
    {
        _gameService = gameService;
        _repository = repository;

        gameService.GenerationChanged += OnGenerationChanged;
    }

    public int LastSnapshotGeneration => _repository.LastSnapshotGeneration;

    public GameSnapshot GetSnapshot(int generation) => _repository.LoadSnapshot(generation);

    private void OnGenerationChanged(object? sender, EventArgs e)
    {
        var snapshot = new GameSnapshot(_gameService.LivingCells, _gameService.Generation);
        _repository.SaveSnapshot(snapshot);
    }
}