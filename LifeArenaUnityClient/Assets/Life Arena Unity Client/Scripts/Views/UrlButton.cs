using UnityEngine;
using UnityEngine.UI;

namespace Avangardum.LifeArena.UnityClient.Views
{
    [RequireComponent(typeof(Button))]
    public class UrlButton : MonoBehaviour
    {
        [SerializeField] private string _url;

        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            Application.OpenURL(_url);
        }
    }
}