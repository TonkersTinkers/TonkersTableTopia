using System;
using System.Collections.Generic;
using UnityEngine;

internal static class Ux_TableStyleSync
{
    public static void Sync<T>(ref int count, ref List<T> list, Func<T> create)
    {
        count = Mathf.Max(1, count);
        list ??= new List<T>(count);

        while (list.Count < count)
            list.Add(create());

        if (list.Count > count)
            list.RemoveRange(count, list.Count - count);
    }
}