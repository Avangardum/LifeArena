using System;
using Avangardum.LifeArena.UnityClient.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Avangardum.LifeArena.UnityClient.Views
{
    public class WindowManager : MonoBehaviour, IWindowManager
    {
        [SerializeField] private GameObject _serverUnavailableWindow;
        [SerializeField] private GameObject _noInternetConnectionWindow;
        [SerializeField] private GameObject _helpWindow;
        [SerializeField] private GameObject _closeHelpButton;

        private void Awake()
        {
            _closeHelpButton.GetComponent<Button>().onClick.AddListener(OnCloseHelpButtonClicked);
        }

        public bool IsServerUnavailableWindowVisible
        {
            set => _serverUnavailableWindow.SetActive(value);
        }

        public bool IsNoInternetConnectionWindowVisible 
        {
            set => _noInternetConnectionWindow.SetActive(value);
        }

        public bool IsHelpWindowVisible 
        {
            set => _helpWindow.SetActive(value);
        }

        private void OnCloseHelpButtonClicked()
        {
            IsHelpWindowVisible = false;
        }
    }
}