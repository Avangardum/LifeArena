using System.Reflection;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Avangardum.LifeArena.UnityClient.Editor
{
    public static class SynchronizationContextUtils
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    OnPlayModeExit();
                }
            };
        }
 
        private static void OnPlayModeExit()
        {
            KillCurrentSynchronizationContext();
        }
 
        /// <summary>
        /// Kills the current synchronization context after exiting play mode to avoid Tasks continuing to run.
        /// </summary>
        private static void KillCurrentSynchronizationContext()
        {
            var synchronizationContext = SynchronizationContext.Current;
 
            var constructor = synchronizationContext
                .GetType()
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(int) }, null);
 
            if (constructor == null)
            {
                return;
            }
 
            object newContext = constructor.Invoke(new object[] { Thread.CurrentThread.ManagedThreadId });
            SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);
        }
    }
}