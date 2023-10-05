using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace LifeArenaBlazorClient.Shared;

public partial class TimeUntilNextGenerationHeaderSection
{
    /// <summary>
    /// If the timer tries to increase by less than this amount (due to network latency), ignore it to avoid
    /// jittering. If the timer tries to increase by more or equal than this amount (due to generation change),
    /// allow it. 
    /// </summary>
    private static readonly TimeSpan MinTimeToAllowIncreasingTimer = TimeSpan.FromSeconds(1);
    
    private const string White = "white";
    private const string Black = "black";
    
    private Stopwatch _stopwatch = new();
    private TimeSpan _timeUntilNextGeneration;
    
    [Parameter]
    public required int Generation { get; set; }
    
    [Parameter]
    public required TimeSpan NextGenerationInterval { get; set; }
    
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

    private string TimeUntilNextGenerationClockStyle
    {
        get
        {
            var firstColor = Generation % 2 == 0 ? White : Black;
            var secondColor = Generation % 2 == 0 ? Black : White;
            var firstColorPercentage = TimeUntilNextGeneration / NextGenerationInterval;
            return $"background: conic-gradient({firstColor} {1 - firstColorPercentage:P}, {secondColor} 0)";
        }
    }
    
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

    public TimeUntilNextGenerationHeaderSection()
    {
        Console.WriteLine("TimeUntilNextGenerationHeaderSection constructor");
    }
}