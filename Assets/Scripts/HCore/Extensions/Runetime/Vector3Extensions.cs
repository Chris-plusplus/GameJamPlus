using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HCore.Extensions
{
    public static class Vector3Extensions
    {
        public static void Draw(this Vector3 point, Color color, float size = 0.3f)
        {
            Debug.DrawLine(point - Vector3.up * size, point + Vector3.up * size, color);
            Debug.DrawLine(point - Vector3.forward * size, point + Vector3.forward * size, color);
            Debug.DrawLine(point - Vector3.right * size, point + Vector3.right * size, color);
        }

        public static void Draw(this Vector3 point, Color color, float time, float size = 0.3f)
        {
            Debug.DrawLine(point - Vector3.up * size, point + Vector3.up * size, color, time);
            Debug.DrawLine(point - Vector3.forward * size, point + Vector3.forward * size, color, time);
            Debug.DrawLine(point - Vector3.right * size, point + Vector3.right * size, color, time);
        }
    }
}