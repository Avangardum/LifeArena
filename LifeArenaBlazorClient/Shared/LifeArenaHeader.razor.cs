using System.Diagnostics;

namespace LifeArenaBlazorClient.Shared;

public partial class LifeArenaHeader
{
    private TimeUntilNextGenerationHeaderSection _timeUntilNextGenerationHeaderSection = null!;
    
    public TimeSpan NextGenerationInterval { get; set; }
    public int Generation { get; set; }
    public int CellsLeft { get; set; }

    public TimeSpan TimeUntilNextGeneration
    {
        set => _timeUntilNextGenerationHeaderSection.TimeUntilNextGeneration = value;
    }

    public void InvokeStateHasChanged() => StateHasChanged();
}