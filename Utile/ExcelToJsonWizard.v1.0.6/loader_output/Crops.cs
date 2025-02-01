using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Crops
{
    /// <summary>
    /// cropID
    /// </summary>
    public int key;

    /// <summary>
    /// GrowthStage
    /// </summary>
    public int growthStage;

}
public class CropsLoader
{
    public List<Crops> ItemsList { get; private set; }
    public Dictionary<int, Crops> ItemsDict { get; private set; }

    public CropsLoader(string path = "JSON/Crops")
    {
        string jsonData;
        jsonData = Resources.Load<TextAsset>(path).text;
        ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
        ItemsDict = new Dictionary<int, Crops>();
        foreach (var item in ItemsList)
        {
            ItemsDict.Add(item.key, item);
        }
    }

    [Serializable]
    private class Wrapper
    {
        public List<Crops> Items;
    }

    public Crops GetByKey(int key)
    {
        if (ItemsDict.ContainsKey(key))
        {
            return ItemsDict[key];
        }
        return null;
    }
    public Crops GetByIndex(int index)
    {
        if (index >= 0 && index < ItemsList.Count)
        {
            return ItemsList[index];
        }
        return null;
    }
}
