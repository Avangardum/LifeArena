using System;
using UnityEngine;
using UnityEngine.UI;

namespace Avangardum.LifeArena.UnityClient.Views
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class CellView : MonoBehaviour
    {
        private const float MaxMouseMovementToRegisterClick = 10f;
        private const float MaxTimeToRegisterClick = 0.5f;
        private static readonly Color DeadColor = Color.black;
        private static readonly Color AliveColor = Color.white;
        
        [SerializeField] private Image _borderImage;
        
        private Image _image;
        private Button _button;
        private Vector2 _pointerPositionOnLastMouseButtonDown;
        private float _timeOnLastMouseButtonDown;

        public EventHandler Clicked;
        
        public bool IsAlive
        {
            get => _image.color == AliveColor;
            set => _image.color = value ? AliveColor : DeadColor;
        }
        
        public bool IsBorderVisible
        {
            set => _borderImage.enabled = value;
        }
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _pointerPositionOnLastMouseButtonDown = Input.mousePosition;
                _timeOnLastMouseButtonDown = Time.time;
            }
        }

        private void OnButtonClicked()
        {
            var mouseMovement = Vector2.Distance(_pointerPositionOnLastMouseButtonDown, Input.mousePosition);
            var timePassed = Time.time - _timeOnLastMouseButtonDown;
            if (IsAlive) return;
            if (timePassed > MaxTimeToRegisterClick) return;
            if (mouseMovement > MaxMouseMovementToRegisterClick) return;
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}