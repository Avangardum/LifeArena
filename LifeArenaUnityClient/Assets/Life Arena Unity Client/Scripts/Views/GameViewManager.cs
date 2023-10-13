using System;
using Avangardum.LifeArena.UnityClient.Data;
using Avangardum.LifeArena.UnityClient.Interfaces;
using Zenject;

namespace Avangardum.LifeArena.UnityClient.Views
{
    /// <summary>
    /// Manages interactions between various game views.
    /// </summary>
    public class GameViewManager
    {
        private IFieldView _fieldView;
        private IWindowManager _windowManager;
        private IHeader _header;
        
        [Inject]
        public void Inject(IFieldView fieldView, IWindowManager windowManager, IHeader header)
        {
            _fieldView = fieldView;
            _windowManager = windowManager;
            _header = header;
            
            _fieldView.ZoomChanged += OnFieldZoomChanged;
            
            _header.ZoomPercentageChanged += OnHeaderZoomPercentageChanged;
            _header.HelpClicked += OnHelpClicked;
        }

        private void OnHeaderZoomPercentageChanged(object sender, ZoomPercentageChangedEventArgs e)
        {
            _fieldView.ZoomFocusPointMode = ZoomFocusPointMode.ScreenCenter;
            _fieldView.ZoomPercentage = e.ZoomPercentage;
        }
        
        private void OnFieldZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            _header.ZoomPercentage = e.ZoomPercentage;
        }
        
        private void OnHelpClicked(object sender, EventArgs e)
        {
            _windowManager.IsHelpWindowVisible = true;
        }
    }
}