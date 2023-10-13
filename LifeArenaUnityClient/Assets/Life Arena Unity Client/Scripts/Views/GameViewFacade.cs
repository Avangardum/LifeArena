using System;
using Avangardum.LifeArena.UnityClient.Data;
using Avangardum.LifeArena.UnityClient.Interfaces;
using Zenject;

namespace Avangardum.LifeArena.UnityClient.Views
{
    /// <summary>
    /// Provides a centralized interface for interacting with the game view as a whole.
    /// </summary>
    public class GameViewFacade : IGameViewFacade
    {
        private IFieldView _fieldView;
        private IWindowManager _windowManager;
        private IHeader _header;
        
        public event EventHandler<CellClickedEventArgs> CellClicked;

        public GameState GameState
        {
            set
            {
                _fieldView.LivingCells = value.LivingCells;
                
                _windowManager.IsNoInternetConnectionWindowVisible = false;
                _windowManager.IsServerUnavailableWindowVisible = false;
                
                _header.Generation = value.Generation;
                _header.NextGenerationInterval = value.NextGenerationInterval;
                _header.TimeUntilNextGeneration = value.TimeUntilNextGeneration;
                _header.CellsLeft = value.CellsLeft;
            }
        }
        
        [Inject]
        public void Inject(IFieldView fieldView, IWindowManager windowManager, IHeader header)
        {
            _fieldView = fieldView;
            _windowManager = windowManager;
            _header = header;
            
            _fieldView.CellClicked += OnCellClicked;
        }
        
        public void ShowNoInternetConnectionMessage()
        {
            _windowManager.IsServerUnavailableWindowVisible = false;
            _windowManager.IsNoInternetConnectionWindowVisible = true;
        }

        public void ShowServerUnavailableMessage()
        {
            _windowManager.IsNoInternetConnectionWindowVisible = false;
            _windowManager.IsServerUnavailableWindowVisible = true;
        }
        
        private void OnCellClicked(object sender, CellClickedEventArgs e)
        {
            CellClicked?.Invoke(this, e);
        }
    }
}