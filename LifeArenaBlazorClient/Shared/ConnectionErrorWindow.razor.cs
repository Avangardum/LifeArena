namespace LifeArenaBlazorClient.Shared;

public partial class ConnectionErrorWindow
{
    private bool _isVisible;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            StateHasChanged();
        }
    }
}