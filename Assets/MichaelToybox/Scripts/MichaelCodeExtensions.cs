using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//REFERENCES:
//https://forum.unity.com/threads/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code.241052/
//
public static class MichaelCodeExtensions 
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        var count = list.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    /// <summary>
    /// Clones the object passed in
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IList<T> CloneList<T>(this IList<T> list)
    {
        List<T> cloneList = new List<T>();
        for (var i = 0; i < list.Count; ++i)
        {
            cloneList.Add(list[i]);
        }

        return cloneList;
    }
}
