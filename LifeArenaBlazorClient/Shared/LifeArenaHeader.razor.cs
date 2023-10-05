using System.Diagnostics;

namespace LifeArenaBlazorClient.Shared;

public partial class LifeArenaHeader
{
    private TimeUntilNextGenerationHeaderSection _timeUntilNextGenerationHeaderSection = null!;
    private ZoomHeaderSection _zoomHeaderSection = null!;
    
    public event EventHandler? ZoomChangedWithHeader;
    
    public TimeSpan NextGenerationInterval { get; set; }
    public int Generation { get; set; }
    public int CellsLeft { get; set; }

    public TimeSpan TimeUntilNextGeneration
    {
        set => _timeUntilNextGenerationHeaderSection.TimeUntilNextGeneration = value;
    }

    public double ZoomPercentage
    {
        get => _zoomHeaderSection.ZoomPercentage;
        set => _zoomHeaderSection.ZoomPercentage = value;
    }

    public void InvokeStateHasChanged() => StateHasChanged();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender) OnAfterFirstRender();
    }

    private void OnAfterFirstRender()
    {
        _zoomHeaderSection.ZoomChangedWithHeader += OnZoomHeaderSectionOnZoomChangedWithHeader;
    }

    private void OnZoomHeaderSectionOnZoomChangedWithHeader(object? sender, EventArgs e)
    {
        ZoomChangedWithHeader?.Invoke(this, EventArgs.Empty);
    }
}