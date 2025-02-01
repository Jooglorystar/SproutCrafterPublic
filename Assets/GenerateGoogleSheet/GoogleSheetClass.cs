using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>You must approach through `GoogleSheetManager.SO<GoogleSheetSO>()`</summary>
public class GoogleSheetSO : ScriptableObject
{
	public List<Item> ItemList;
	public List<Resource> ResourceList;
	public List<Seed> SeedList;
	public List<Crop> CropList;
	public List<Fish> FishList;
}

[Serializable]
public class Item
{
	public int itemCode;
	public string itemName;
	public string itemDescription;
	public Sprite itemSprite;
	public int buyPrice;
	public int sellPrice;
	public bool isCanStack;
	public int maxStack;
	public GameObject dropItemPrefab;
	public ItemType ItemType;
}

[Serializable]
public class Resource
{
	public int itemCode;
	public string itemName;
	public string itemDescription;
	public Sprite itemSprite;
	public int buyPrice;
	public int sellPrice;
	public bool isCanStack;
	public int maxStack;
	public GameObject dropItemPrefab;
	public ItemType ItemType;
	public int dropAmount;
	public string requiredTool;
	public int maxGrowthStage;
	public int growTime;
	public int health;
}

[Serializable]
public class Seed
{
	public int itemCode;
	public string itemName;
	public string itemDescription;
	public Sprite itemSprite;
	public int buyPrice;
	public int sellPrice;
	public bool isCanStack;
	public int maxStack;
	public GameObject dropItemPrefab;
	public ItemType ItemType;
	public int linkedSeed;
	public int maxGrowthStage;
	public int growTime;
	public ESeason plantingSeasons;
	public TileType tileType;
	public int minYield;
	public int maxYield;
}

[Serializable]
public class Crop
{
	public int itemCode;
	public string itemName;
	public string itemDescription;
	public Sprite itemSprite;
	public int buyPrice;
	public int sellPrice;
	public bool isCanStack;
	public int maxStack;
	public GameObject dropItemPrefab;
	public ItemType ItemType;
}

[Serializable]
public class Fish
{
	public int itemCode;
	public string itemName;
	public string itemDescription;
	public Sprite itemSprite;
	public int sellPrice;
	public bool isCanStack;
	public int maxStack;
	public GameObject dropItemPrefab;
	public ItemType ItemType;
}

