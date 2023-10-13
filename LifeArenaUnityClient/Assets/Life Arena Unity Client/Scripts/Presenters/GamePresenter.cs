using System;
using System.Threading.Tasks;
using Avangardum.LifeArena.UnityClient.Data;
using Avangardum.LifeArena.UnityClient.Exceptions;
using Avangardum.LifeArena.UnityClient.Helpers;
using Avangardum.LifeArena.UnityClient.Interfaces;
using UnityEngine;

namespace Avangardum.LifeArena.UnityClient.Presenters
{
    public class GamePresenter
    {
        private IServerFacade _serverFacade;
        private IGameViewFacade _gameView;
        private DateTime _lastGameStateUpdateTime;

        public GamePresenter(IServerFacade serverFacade, IGameViewFacade gameView)
        {
            _serverFacade = serverFacade;
            _gameView = gameView;
            
            _gameView.CellClicked += OnCellClicked;
            
            GetGameStateLoop();
        }

        private async void GetGameStateLoop()
        {
            var getGameStateDelay = TimeSpan.FromSeconds(0.2);
            var timerCheckDelay = TimeSpan.FromSeconds(0.04);
            
            while (true)
            {
                await GetGameState();

                while (DateTime.Now - _lastGameStateUpdateTime < getGameStateDelay)
                {
                    await AsyncHelper.Delay(timerCheckDelay);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private async Task GetGameState()
        {
            try
            {
                var gameState = await _serverFacade.GetGameState();
                _gameView.GameState = gameState;
            }
            catch (NoInternetConnectionException)
            {
                _gameView.ShowNoInternetConnectionMessage();
            }
            catch (ServerUnavailableException)
            {
                _gameView.ShowServerUnavailableMessage();
            }

            _lastGameStateUpdateTime = DateTime.Now;
        }

        private async void OnCellClicked(object sender, CellClickedEventArgs e)
        {
            await AddCell(e.X, e.Y);
        }

        private async Task AddCell(int x, int y)
        {
            try
            {
                var gameState = await _serverFacade.AddCell(x, y);
                _gameView.GameState = gameState;
            }
            catch (NoInternetConnectionException)
            {
                _gameView.ShowNoInternetConnectionMessage();
            }
            catch (ServerUnavailableException)
            {
                _gameView.ShowServerUnavailableMessage();
            }
            
            _lastGameStateUpdateTime = DateTime.Now;
        }
    }
}