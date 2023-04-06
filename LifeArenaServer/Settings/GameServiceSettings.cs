namespace Avangardum.LifeArena.Server.Settings;

public class GameServiceSettings
{
    public int MaxCellsPerPlayerPerGeneration { get; set; }
    public TimeSpan NextGenerationInterval { get; set; }
}