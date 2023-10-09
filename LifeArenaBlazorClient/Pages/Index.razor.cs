using System.Diagnostics;
using Avangardum.LifeArena.Shared;
using LifeArenaBlazorClient.Interfaces;
using LifeArenaBlazorClient.Shared;
using Microsoft.AspNetCore.Components;

namespace LifeArenaBlazorClient.Pages;

public partial class Index
{
    private static readonly TimeSpan MinDelayBetweenUpdates = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan DelayBeforeFirstUpdate = TimeSpan.FromMilliseconds(10);
    private static readonly TimeSpan ConnectionErrorTimeToShowError = TimeSpan.FromSeconds(3);

    private readonly Stopwatch _connectionErrorStopwatch = new();
    
    private LifeArenaHeader _lifeArenaHeader = null!;
    private LifeArenaBody _lifeArenaBody = null!;
    private HelpWindow _helpWindow = null!;
    private ConnectionErrorWindow _connectionErrorWindow = null!;

    [Inject]
    public required IGameService GameService { private get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await UpdateGameStateLoop();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender) OnAfterFirstRender();
    }

    private void OnAfterFirstRender()
    {
        _lifeArenaBody.ZoomChangedWithWheel += (_, _) => SetHeaderZoomPercentageToBodyZoomPercentage();
        _lifeArenaHeader.ZoomChangedWithHeader += (_, _) => SetBodyZoomPercentageToHeaderZoomPercentage();
        SetHeaderZoomPercentageToBodyZoomPercentage();

        _lifeArenaHeader.HelpClicked += OnHelpClicked;
    }

    private void OnHelpClicked(object? sender, EventArgs e)
    {
        _helpWindow.IsVisible = true;
    }

    private async Task UpdateGameStateLoop()
    {
        await Task.Delay(DelayBeforeFirstUpdate);
        while (true)
        {
            var delay = Task.Delay(MinDelayBetweenUpdates);
            await UpdateGameState();
            await delay;
        }
        // ReSharper disable once FunctionNeverReturns
    }
    
    private async Task UpdateGameState()
    {
        GameState gameState;
        try
        {
            gameState = await GameService.GetGameStateAsync();
        }
        catch (HttpRequestException)
        {
            _connectionErrorStopwatch.Start();
            Console.WriteLine(_connectionErrorStopwatch.Elapsed);
            if (_connectionErrorStopwatch.Elapsed > ConnectionErrorTimeToShowError)
            {
                _connectionErrorWindow.IsVisible = true;
            }
            return;
        }

        _connectionErrorStopwatch.Reset();
        _connectionErrorWindow.IsVisible = false;
        _lifeArenaHeader.NextGenerationInterval = gameState.NextGenerationInterval;
        _lifeArenaHeader.TimeUntilNextGeneration = gameState.TimeUntilNextGeneration;
        _lifeArenaHeader.Generation = gameState.Generation;
        _lifeArenaHeader.CellsLeft = gameState.CellsLeft;
        _lifeArenaHeader.InvokeStateHasChanged();
        
        _lifeArenaBody.LivingCells = gameState.LivingCells;
        _lifeArenaBody.InvokeStateHasChanged();
    }

    private void SetHeaderZoomPercentageToBodyZoomPercentage()
    {
        _lifeArenaHeader.ZoomPercentage = _lifeArenaBody.ZoomPercentage;
    }

    private void SetBodyZoomPercentageToHeaderZoomPercentage()
    {
        _lifeArenaBody.SetZoomPercentageWithHeaderAsync(_lifeArenaHeader.ZoomPercentage);
    }
}