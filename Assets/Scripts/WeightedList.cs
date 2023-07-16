using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedList<T>
{
    class ListItem
    {
        public T item;
        public int cumulativeWeight;
    }

    [SerializeField]
    private List<ListItem> _list;

    public WeightedList() {
        _list = new List<ListItem>();
    }

    public void AddItem(T item, int weight)
    {
        int cumulativeWeight = _list.Count > 0 ? _list[^1].cumulativeWeight + weight : weight;
        ListItem newEntry = new ListItem();
        newEntry.item = item;
        newEntry.cumulativeWeight = cumulativeWeight;

        _list.Add(newEntry);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public T getItem()
    {
        int index = Random.Range(0, _list[^1].cumulativeWeight);
        foreach (ListItem item in _list)
        {
            if (index < item.cumulativeWeight)
            {
                return item.item;
            }
        }

        // This will return null
        return default;
    }
}
