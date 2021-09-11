using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSet<T> : ScriptableObject
{
    public List<T> items = new List<T>();
    public void Initialize()
    {
        items.Clear();
    }

    public int Length()
    {
        return items.Count;
    }

    public T GetItemIndex(int index)
    {
        return items[index];
    }

    public void AddToList(T thingToAdd)
    {
        if (!items.Contains(thingToAdd))
            items.Add(thingToAdd);
    }

    public void RemoveFromList(T thingToRemove)
    {
        if (items.Contains(thingToRemove))
            items.Remove(thingToRemove);
    }
}
