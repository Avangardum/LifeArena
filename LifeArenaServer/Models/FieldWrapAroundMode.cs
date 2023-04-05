namespace Avangardum.LifeArena.Server.Models;

[Flags]
public enum FieldWrapAroundMode
{
    None = 0,
    Horizontal = 1 << 0,
    Vertical = 1 << 1,
    All = Horizontal | Vertical
}