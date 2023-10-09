using Microsoft.AspNetCore.Components;

namespace LifeArenaBlazorClient.Shared;

public partial class HelpWindow
{
    private bool _isVisible = true;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            StateHasChanged();
        }
    }

    private void OnCloseClick()
    {
        IsVisible = false;
    }
}