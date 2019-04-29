using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    public static Dictionary<V, K> Reverse<K, V>(this Dictionary<K, V> source)
    {
        Dictionary<V, K> Result = new Dictionary<V, K>();
        foreach (var Value in source)
        {
            Result[Value.Value] = Value.Key;
        }
        return Result;
    }

    public static T ChooseRandom<T>(this T[] source)
    {
        int index = Random.Range(0, source.Length);
        return source[index];
    }

    public static Vector3 ComponentwiseDivide(this Vector3 divisor, Vector3 dividend)
    {
        return new Vector3(divisor.x / dividend.x, divisor.y / dividend.y, divisor.z / dividend.z);
    }
}
