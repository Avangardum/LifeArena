using System;
using Avangardum.LifeArena.UnityClient.Data;

namespace Avangardum.LifeArena.UnityClient.Interfaces
{
    public interface IHeader
    {
        event EventHandler<ZoomPercentageChangedEventArgs> ZoomPercentageChanged; 
        event EventHandler HelpClicked;

        int Generation { set; }
        TimeSpan NextGenerationInterval { set; }
        TimeSpan TimeUntilNextGeneration { set; }
        int CellsLeft { set; }
        float ZoomPercentage { get; set; }
    }
}