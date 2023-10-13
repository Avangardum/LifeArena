using System;
using System.Collections;
using System.Threading.Tasks;
using Avangardum.AvangardumUnityUtilityLib;
using UnityEngine;

namespace Avangardum.LifeArena.UnityClient.Helpers
{
    public static class AsyncHelper
    {
        /// <summary>
        /// Alternative to Task.Delay() that works in WebGL
        /// </summary>
        public static Task Delay(TimeSpan delay)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            CoroutineHelper.StartCoroutine(DelayCoroutine());
            return taskCompletionSource.Task;
            
            IEnumerator DelayCoroutine()
            {
                yield return new WaitForSeconds((float) delay.TotalSeconds);
                taskCompletionSource.SetResult(null);
            }
        }
    }
}