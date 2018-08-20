using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendedTool {
    /// <summary>
    /// <Extension> Use for pick random element from list
    /// </summary>
    public static T RandomPick<T>(this List<T> list) {
        var index = Random.Range(0, list.Count);
        return list[index];
    }

    /// <summary>
    /// <Extension> Use for pick random element from array
    /// </summary>
    public static T RandomPick<T>(this T[] list) {
        var index = Random.Range(0, list.Length);
        return list[index];
    }

    /// <summary>
    /// <Extension> Convert Vector3 to Vector2
    /// </summary>
    public static Vector2 ToVector2(this Vector3 a) {
        return new Vector2(a.x, a.y);
    }

    /// <summary>
    /// <Extension> Merge Vector3 with Vector2 (a + b)
    /// </summary>
    public static Vector3 MergeVector2 (this Vector3 a, Vector2 b) {
        return a + new Vector3(b.x, b.y, 0);
    }

    /// <summary>
    /// <Extension> Like Transform.LookAt but in 2D space
    /// </summary>
    public static void LookAt2D(this Transform obj, Vector3 Target) {
        var dir = obj.position - Target;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    /// <summary>
    /// <extension> Return world position reference from Transform
    /// </summary>
    public static Vector3 getLocalPositionOfTransform(this Transform transform, Vector3 position) {
        return transform.position + (transform.forward * position.z) + (transform.right * position.x) + (transform.up * position.y);
    }

    /// <summary>
    /// <Extension> Sin function for curve movement
    /// </summary>
    public static float SinFomular(float distance, float x, float amplitude = 2) {
        return Mathf.Sin(x * Mathf.PI / distance) * amplitude;
    }
}
