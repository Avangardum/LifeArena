namespace LifeArenaBlazorClient.Shared;

public partial class SquareButtonsHeaderSection
{
    public event EventHandler? HelpClicked;
    
    private void OnHelpClicked()
    {
        HelpClicked?.Invoke(this, EventArgs.Empty);
    }
}