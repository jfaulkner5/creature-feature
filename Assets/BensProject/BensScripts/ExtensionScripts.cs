using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionScripts {
     
    /// <summary>
    /// Will only add if the list does not already contain the object. will log warning if the item has already been added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    public static void AddSafe<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }
        else
        {
            Debug.LogWarning("List Already Contains");
        }      
    }

    /// <summary>
    /// Will only add if the list does not already contain the object. if logWarning true will log warning if the item has already been added.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    public static void AddSafe<T>(this List<T> list, T item,bool logWarning)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
        }
        else
        {
            if (logWarning)
            {
                Debug.LogWarning("List Already Contains");
            }            
        }
    }
}
