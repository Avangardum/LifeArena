using Avangardum.LifeArena.Shared;
using LifeArenaBlazorClient.Interfaces;
using LifeArenaBlazorClient.Shared;
using Microsoft.AspNetCore.Components;

namespace LifeArenaBlazorClient.Pages;

public partial class Index
{
    private static readonly TimeSpan MinDelayBetweenUpdates = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan DelayBeforeFirstUpdate = TimeSpan.FromMilliseconds(10);
    
    private LifeArenaBody _lifeArenaBody = null!;
    
    [Inject]
    public required IGameService GameService { private get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await UpdateGameStateLoop();
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
        var gameStateResponse = await GameService.GetGameStateAsync();
        _lifeArenaBody.LivingCells = gameStateResponse.LivingCells;
        StateHasChanged();
    }
}