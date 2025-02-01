#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GoogleSheetSOConverter : MonoBehaviour
{
    public GoogleSheetSO googleSheetSO; // GoogleSheetSO 참조
    private string baseSavePath = "Assets/ScriptableObjectAssets/Item"; // ScriptableObject 저장 기본 경로

    [ContextMenu("Convert Google Sheet Data to ScriptableObjects")]
    public void SaveElementsAsScriptableObjects()
    {
        if (googleSheetSO == null)
        {
            Debug.LogError("GoogleSheetSO가 설정되지 않았습니다.");
            return;
        }

        // 각각의 리스트를 ScriptableObject로 변환하여 저장
        SaveItems(googleSheetSO.ItemList, "Items");
        SaveResources(googleSheetSO.ResourceList, "Resources");
        SaveSeeds(googleSheetSO.SeedList, "Seeds");
        SaveCrops(googleSheetSO.CropList, "Crops");
        SaveFish(googleSheetSO.FishList, "Fishs");
    }

    void SaveItems(List<Item> items, string folderName)
    {
        string savePath = $"{baseSavePath}/{folderName}";
        EnsureFolderExists(savePath);

        foreach (var item in items)
        {
            var newSO = ScriptableObject.CreateInstance<ItemSO>();
            newSO.itemCode = item.itemCode;
            newSO.itemName = item.itemName;
            newSO.itemDescription = item.itemDescription;
            newSO.itemSprite = item.itemSprite;
            newSO.buyPrice = item.buyPrice;
            newSO.sellPrice = item.sellPrice;
            newSO.isCanStack = item.isCanStack;
            newSO.maxStack = item.maxStack;
            newSO.dropItemPrefab = item.dropItemPrefab;
            newSO.itemType = item.ItemType;

            string assetPath = $"{savePath}/{item.itemName}.asset";
            AssetDatabase.CreateAsset(newSO, assetPath);
        }

        AssetDatabase.SaveAssets();
    }

    void SaveResources(List<Resource> resources, string folderName)
    {
        string savePath = $"{baseSavePath}/{folderName}";
        EnsureFolderExists(savePath);

        foreach (var resource in resources)
        {
            var newSO = ScriptableObject.CreateInstance<ResourceSO>();
            newSO.itemCode = resource.itemCode;
            newSO.itemName = resource.itemName;
            newSO.itemDescription = resource.itemDescription;
            newSO.itemSprite = resource.itemSprite;
            newSO.buyPrice = resource.buyPrice;
            newSO.sellPrice = resource.sellPrice;
            newSO.isCanStack = resource.isCanStack;
            newSO.maxStack = resource.maxStack;
            newSO.dropItemPrefab = resource.dropItemPrefab;
            newSO.itemType = resource.ItemType;
            newSO.dropAmount = resource.dropAmount;
            newSO.requiredTool = resource.requiredTool;
            newSO.maxGrowthStage = resource.maxGrowthStage;
            newSO.growTime = resource.growTime;
            newSO.health = resource.health;

            string assetPath = $"{savePath}/{resource.itemName}.asset";
            AssetDatabase.CreateAsset(newSO, assetPath);
        }

        AssetDatabase.SaveAssets();
    }

    void SaveSeeds(List<Seed> seeds, string folderName)
    {
        string savePath = $"{baseSavePath}/{folderName}";
        EnsureFolderExists(savePath);

        foreach (var seed in seeds)
        {
            var newSO = ScriptableObject.CreateInstance<SeedSO>();

            newSO.itemCode = seed.itemCode;
            newSO.itemName = seed.itemName;
            newSO.itemDescription = seed.itemDescription;
            newSO.itemSprite = seed.itemSprite;
            newSO.buyPrice = seed.buyPrice;
            newSO.sellPrice = seed.sellPrice;
            newSO.isCanStack = seed.isCanStack;
            newSO.maxStack = seed.maxStack;
            newSO.dropItemPrefab = seed.dropItemPrefab;
            newSO.itemType = seed.ItemType;

            newSO.linkedCropCode = seed.linkedSeed;
            newSO.maxGrowthStage = seed.maxGrowthStage;
            newSO.growTime = seed.growTime;
            newSO.plantingSeasons = seed.plantingSeasons;
            newSO.tileType = seed.tileType;
            newSO.minYield = seed.minYield;
            newSO.maxYield = seed.maxYield;

            string assetPath = $"{savePath}/{seed.itemName}.asset";
            AssetDatabase.CreateAsset(newSO, assetPath);
        }

        AssetDatabase.SaveAssets();
    }

    void SaveCrops(List<Crop> crops, string folderName)
    {
        string savePath = $"{baseSavePath}/{folderName}";
        EnsureFolderExists(savePath);

        foreach (var crop in crops)
        {
            var newSO = ScriptableObject.CreateInstance<CropsSO>();
            newSO.itemCode = crop.itemCode;
            newSO.itemName = crop.itemName;
            newSO.itemDescription = crop.itemDescription;
            newSO.itemSprite = crop.itemSprite;
            newSO.buyPrice = crop.buyPrice;
            newSO.sellPrice = crop.sellPrice;
            newSO.isCanStack = crop.isCanStack;
            newSO.maxStack = crop.maxStack;
            newSO.dropItemPrefab = crop.dropItemPrefab;
            newSO.itemType = crop.ItemType;

            string assetPath = $"{savePath}/{crop.itemName}.asset";
            AssetDatabase.CreateAsset(newSO, assetPath);
        }

        AssetDatabase.SaveAssets();
    }

    void SaveFish(List<Fish> fishes, string folderName)
    {
        string savePath = $"{baseSavePath}/{folderName}";
        EnsureFolderExists(savePath);

        foreach (var fish in fishes)
        {
            var newSO = ScriptableObject.CreateInstance<FishSO>();
            newSO.itemCode = fish.itemCode;
            newSO.itemName = fish.itemName;
            newSO.itemDescription = fish.itemDescription;
            newSO.itemSprite = fish.itemSprite;
            newSO.sellPrice = fish.sellPrice;
            newSO.isCanStack = fish.isCanStack;
            newSO.maxStack = fish.maxStack;
            newSO.dropItemPrefab = fish.dropItemPrefab;
            newSO.itemType = fish.ItemType;

            string assetPath = $"{savePath}/{fish.itemName}.asset";
            AssetDatabase.CreateAsset(newSO, assetPath);
        }

        AssetDatabase.SaveAssets();
    }

    void EnsureFolderExists(string path)
    {
        if (!path.StartsWith("Assets/"))
        {
            return;
        }

        string relativePath = path.Substring(7).Trim();

        string[] folders = relativePath.Split('/');

        string currentPath = "Assets";

        foreach (var folder in folders)
        {
            if (string.IsNullOrWhiteSpace(folder)) continue;

            if (!AssetDatabase.IsValidFolder($"{currentPath}/{folder}"))
            {
                AssetDatabase.CreateFolder(currentPath, folder);
            }
            currentPath = System.IO.Path.Combine(currentPath, folder);
        }
    }
}
#endif