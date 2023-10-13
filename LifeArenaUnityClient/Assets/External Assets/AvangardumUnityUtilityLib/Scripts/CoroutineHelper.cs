using System;
using System.Collections;
using UnityEngine;

namespace Avangardum.AvangardumUnityUtilityLib
{
    public static class CoroutineHelper
    {
        public class CoroutineRunner : MonoBehaviour
        {
            bool _isQuitting;
            
            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
                Application.quitting += () => _isQuitting = true;
            }

            private void OnDestroy()
            {
                if (!_isQuitting) Debug.LogError("CoroutineRunner was destroyed. This shouldn't happen.");
            }
        }
        
        private static CoroutineRunner _runner;
        
        static CoroutineHelper()
        {
            _runner = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
        }
        
        public static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return _runner.StartCoroutine(coroutine);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            _runner.StopCoroutine(coroutine);
        }

        public static Coroutine Invoke(Action action, float time)
        {
            return _runner.StartCoroutine(InvokeCoroutine(action, time));
        }
        
        public static Coroutine InvokeRepeating(Action action, float time, float repeatRate)
        {
            return _runner.StartCoroutine(InvokeRepeatingCoroutine(action, time, repeatRate));
        }

        private static IEnumerator InvokeCoroutine(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }
        
        private static IEnumerator InvokeRepeatingCoroutine(Action action, float time, float repeatRate)
        {
            yield return new WaitForSeconds(time);
            while (true)
            {
                action();
                yield return new WaitForSeconds(repeatRate);
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
