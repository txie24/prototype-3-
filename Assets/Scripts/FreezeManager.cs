using System.Collections.Generic;
using UnityEngine;

public static class FreezeManager
{
    static readonly HashSet<object> watchers = new HashSet<object>();

    public static bool IsFrozen => watchers.Count > 0;

    public static void SetWatching(object key, bool isWatching)
    {
        if (isWatching) watchers.Add(key);
        else watchers.Remove(key);
    }
}
