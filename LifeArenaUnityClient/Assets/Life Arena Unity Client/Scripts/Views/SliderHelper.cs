using UnityEngine;
using UnityEngine.UI;

namespace Avangardum.LifeArena.UnityClient.Views
{
    [RequireComponent(typeof(Slider))]
    public class SliderHelper : MonoBehaviour
    {
        private Slider _slider;
        
        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
        
        public void AddValue(float value)
        {
            _slider.value += value;
        }
    }
}