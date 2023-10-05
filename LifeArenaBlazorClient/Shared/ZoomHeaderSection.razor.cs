namespace LifeArenaBlazorClient.Shared;

public partial class ZoomHeaderSection
{
    private const int ZoomSliderMaxValue = 100;
    private const double ButtonZoomStep = 0.1;
    
    private double _zoomPercentage;
    
    public event EventHandler? ZoomChangedWithHeader;
    
    public double ZoomPercentage
    {
        get => _zoomPercentage;
        set
        {
            _zoomPercentage = Math.Clamp(value, 0, 1);
            StateHasChanged();
        }
    }
    
    private int ZoomSliderValue
    {
        get => (int)(ZoomPercentage * ZoomSliderMaxValue);
        set
        {
            ZoomPercentage = (double)value / ZoomSliderMaxValue;
            ZoomChangedWithHeader?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnMinusClick()
    {
        ZoomPercentage -= ButtonZoomStep;
        ZoomChangedWithHeader?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnPlusClick()
    {
        ZoomPercentage += ButtonZoomStep;
        ZoomChangedWithHeader?.Invoke(this, EventArgs.Empty);
    }
}