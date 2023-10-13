using System;
using Avangardum.LifeArena.UnityClient.Data;
using Avangardum.LifeArena.UnityClient.Views;
using UnityEngine;

namespace Avangardum.LifeArena.UnityClient.Interfaces
{
    public interface IFieldView
    {
        event EventHandler<CellClickedEventArgs> CellClicked;
        event EventHandler<ZoomChangedEventArgs> ZoomChanged;

        bool[,] LivingCells { set; }
        float ZoomPercentage { get; set; }
        ZoomFocusPointMode ZoomFocusPointMode { set; }
        
        void Move(Vector2 movement);
    }
}