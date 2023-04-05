using Avangardum.LifeArena.Server.Models;

namespace Avangardum.LifeArena.Server.Settings;

public class CoreGameModelSettings
{
    public int FieldWidth { get; set; }
    public int FieldHeight { get; set; }
    public FieldWrapAroundMode FieldWrapAroundMode { get; set; }
}