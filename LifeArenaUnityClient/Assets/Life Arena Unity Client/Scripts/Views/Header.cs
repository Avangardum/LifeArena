using System;
using Avangardum.LifeArena.UnityClient.Data;
using Avangardum.LifeArena.UnityClient.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Avangardum.LifeArena.UnityClient.Views
{
    public class Header : MonoBehaviour, IHeader
    {
        [SerializeField] private Button _helpButton;
        [SerializeField] private TextMeshProUGUI _generationText;
        [SerializeField] private TextMeshProUGUI _timeUntilNextGenerationText;
        [SerializeField] private Image _nextGenerationTimerClockFilling;
        [SerializeField] private TextMeshProUGUI _cellsLeftText;
        [SerializeField] private Slider _zoomSlider;

        private int _generation;
        private TimeSpan _timeUntilNextGeneration;
        
        public event EventHandler<ZoomPercentageChangedEventArgs> ZoomPercentageChanged;
        public event EventHandler HelpClicked;

        public int Generation
        {
            private get => _generation;
            set
            {
                _generation = value;
                _generationText.text = _generation.ToString();
            }
        }

        public TimeSpan NextGenerationInterval { private get; set; }

        public TimeSpan TimeUntilNextGeneration
        {
            private get => _timeUntilNextGeneration;
            set
            {
                Assert.AreNotEqual(TimeSpan.Zero, NextGenerationInterval);

                // Sometimes due to network latency, the new TimeUntilNextGeneration is a bit greater than the local value.
                // In this case, ignore it and continue using the local value to prevent timer jitter.
                var timerJitterPreventionThreshold = TimeSpan.FromSeconds(0.3);
                if (value > _timeUntilNextGeneration && value - _timeUntilNextGeneration < timerJitterPreventionThreshold) 
                    return;
                
                _timeUntilNextGeneration = value;
                _timeUntilNextGenerationText.text = _timeUntilNextGeneration.TotalSeconds.ToString("F1");
                var fillAmount = Mathf.InverseLerp(0, (float)NextGenerationInterval.TotalSeconds, 
                    (float)_timeUntilNextGeneration.TotalSeconds);
                if (Generation % 2 == 1)
                {
                    fillAmount = 1 - fillAmount;
                }
                _nextGenerationTimerClockFilling.fillAmount = fillAmount;
                _nextGenerationTimerClockFilling.fillClockwise = Generation % 2 == 1;
            }
        }

        public int CellsLeft
        {
            set => _cellsLeftText.text = value.ToString();
        }

        public float ZoomPercentage
        {
            get => _zoomSlider.value;
            set => _zoomSlider.value = value;
        }

        private void Awake()
        {
            _zoomSlider.onValueChanged.AddListener(OnZoomSliderValueChanged);
            _helpButton.onClick.AddListener(OnHelpButtonClicked);
        }

        private void Update()
        {
            if (TimeUntilNextGeneration == TimeSpan.Zero) return; // Didn't receive GameState yet

            var timeUntilNextGeneration = TimeUntilNextGeneration - TimeSpan.FromSeconds(Time.deltaTime);
            if (timeUntilNextGeneration < TimeSpan.Zero)
            {
                timeUntilNextGeneration = TimeSpan.Zero;
            }
            TimeUntilNextGeneration = timeUntilNextGeneration;
        }

        private void OnZoomSliderValueChanged(float value)
        {
            ZoomPercentageChanged?.Invoke(this, new ZoomPercentageChangedEventArgs(value));
        }

        private void OnHelpButtonClicked()
        {
            HelpClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}