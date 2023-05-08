namespace Avangardum.LifeArena.Server.Interfaces;

public interface ICurrentDateProvider
{
    DateOnly CurrentDate { get; }
}