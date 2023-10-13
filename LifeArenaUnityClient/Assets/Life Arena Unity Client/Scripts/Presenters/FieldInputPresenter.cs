using Avangardum.LifeArena.UnityClient.Interfaces;
using Avangardum.LifeArena.UnityClient.Views;
using UnityEngine;

namespace Avangardum.LifeArena.UnityClient.Presenters
{
    public class FieldInputPresenter
    {
        private readonly IInputManager _inputManager;
        private readonly IFieldView _fieldView;

        public FieldInputPresenter(IInputManager inputManager, IFieldView fieldView)
        {
            _inputManager = inputManager;
            _fieldView = fieldView;
            
            _inputManager.FieldMovementRequested += OnFieldMovementRequested;
            _inputManager.FieldZoomPercentageChangeRequested += OnFieldZoomPercentageChangeRequested;
        }

        private void OnFieldMovementRequested(object sender, IInputManager.FieldMovementRequestedEventArgs e)
        {
            _fieldView.Move(e.Movement);
        }

        private void OnFieldZoomPercentageChangeRequested(object sender, IInputManager.FieldZoomPercentageChangeRequestedEventArgs e)
        {
            _fieldView.ZoomFocusPointMode = ZoomFocusPointMode.Mouse;
            _fieldView.ZoomPercentage += e.ZoomPercentageChange;
        }
    }
}