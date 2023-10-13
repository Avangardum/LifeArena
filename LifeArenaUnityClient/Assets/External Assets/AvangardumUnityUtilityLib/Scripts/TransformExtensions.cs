using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Avangardum.AvangardumUnityUtilityLib
{
    public static class TransformExtensions
    {
        public static List<Transform> GetChildren(this Transform transform) => transform.Cast<Transform>().ToList();
    }
}