using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public static Inventory Instance { get; private set; } = new Inventory();

    private Dictionary<string, int> items;

    public Inventory()
    {
        items = new Dictionary<string, int>();
    }

    public void AddItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName]++;
        }
        else
        {
            items.Add(itemName, 1);
        }
    }

    public bool UseItem(string itemName)
    {
        if (items.ContainsKey(itemName) && items[itemName] > 0)
        {
            items[itemName]--;
            return true;
        }

        return false;
    }

    public int GetItemCount(string itemName)
    {
        return items.ContainsKey(itemName) ? items[itemName] : 0;
    }
    public List<string> GetKeys()
    {
        return new List<string>(items.Keys);
    }

    public bool HasItem(string itemName)
    {
        return items.ContainsKey(itemName) && items[itemName] > 0;
    }
}
