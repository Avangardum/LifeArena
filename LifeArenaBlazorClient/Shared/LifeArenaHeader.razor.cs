using System.Diagnostics;

namespace LifeArenaBlazorClient.Shared;

public partial class LifeArenaHeader
{
    /// <summary>
    /// If the timer tries to increase by less than this amount (due to network latency), ignore it to avoid
    /// jittering. If the timer tries to increase by more or equal than this amount (due to generation change),
    /// allow it. 
    /// </summary>
    private static readonly TimeSpan MinTimeToAllowIncreasingTimer = TimeSpan.FromSeconds(1);
    
    private Stopwatch _stopwatch = new();
    private TimeSpan _timeUntilNextGeneration;

    public TimeSpan NextGenerationInterval { get; set; }
    public int Generation { get; set; }

    public TimeSpan TimeUntilNextGeneration
    {
        get => _timeUntilNextGeneration;
        set
        {
            var deltaTime = value - _timeUntilNextGeneration;
            if (TimeSpan.Zero < deltaTime && deltaTime < MinTimeToAllowIncreasingTimer) return;
            _timeUntilNextGeneration = value;
            if (_timeUntilNextGeneration < TimeSpan.Zero) _timeUntilNextGeneration = TimeSpan.Zero;
        }
    }

    public void InvokeStateHasChanged() => StateHasChanged();

    protected override async Task OnInitializedAsync()
    {
        await TimerUpdateLoop();
    }

    private async Task TimerUpdateLoop()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(20));
            TimeUntilNextGeneration -= _stopwatch.Elapsed;
            _stopwatch.Restart();
            StateHasChanged();
        }
        // ReSharper disable once FunctionNeverReturns
    }
}