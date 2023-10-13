using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Avangardum.AvangardumUnityUtilityLib
{
    public static class CoroutineExtensions
    {
        public static Task ToTask(this Coroutine coroutine)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            CoroutineHelper.StartCoroutine(AwaitOuterCoroutineCoroutine(taskCompletionSource));
            return taskCompletionSource.Task;

            IEnumerator AwaitOuterCoroutineCoroutine(TaskCompletionSource<object> completionSource)
            {
                yield return coroutine;
                completionSource.SetResult(null);
            }
        }
    }
}