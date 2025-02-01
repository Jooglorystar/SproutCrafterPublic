using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SOGeneratorEditorWindow : EditorWindow
{
    public GoogleSheetSO googleSheetSO; // GoogleSheetSO 참조
    private string baseSavePath = "Assets/ScriptableObjectAssets/Item"; // 저장 경로

    [MenuItem("Tools/ScriptableObject Generator")]
    public static void ShowWindow()
    {
        GetWindow<SOGeneratorEditorWindow>("SO Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("ScriptableObject 생성기", EditorStyles.boldLabel);

        googleSheetSO = (GoogleSheetSO)EditorGUILayout.ObjectField("Google Sheet SO", googleSheetSO, typeof(GoogleSheetSO), false);

        baseSavePath = EditorGUILayout.TextField("저장 경로", baseSavePath);

        if (googleSheetSO == null)
        {
            EditorGUILayout.HelpBox("GoogleSheetSO를 설정해주세요.", MessageType.Warning);
            return;
        }

        if (GUILayout.Button("Generate All ScriptableObjects"))
        {
            GenerateAllScriptableObjects();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Items"))
        {
            SaveItems(googleSheetSO.ItemList, "Items");
        }

        if (GUILayout.Button("Generate Resources"))
        {
            SaveResources(googleSheetSO.ResourceList, "Resources");
        }

        if (GUILayout.Button("Generate Seeds"))
        {
            SaveSeeds(googleSheetSO.SeedList, "Seeds");
        }

        if (GUILayout.Button("Generate Crops"))
        {
            SaveCrops(googleSheetSO.CropList, "Crops");
        }

        if (GUILayout.Button("Generate Fish"))
        {
            SaveFish(googleSheetSO.FishList, "Fish");
        }

        EditorGUILayout.Space();

        GUILayout.Label("개별 생성 버튼을 통해 특정 카테고리만 생성할 수 있습니다.", EditorStyles.helpBox);
    }

    private void GenerateAllScriptableObjects()
    {
        SaveItems(googleSheetSO.ItemList, "Items");
        SaveResources(googleSheetSO.ResourceList, "Resources");
        SaveSeeds(googleSheetSO.SeedList, "Seeds");
        SaveCrops(googleSheetSO.CropList, "Crops");
        SaveFish(googleSheetSO.FishList, "Fish");

        Debug.Log("모든 ScriptableObject 생성이 완료되었습니다.");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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

        Debug.Log("Item ScriptableObject 생성 완료.");
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

        Debug.Log("Resource ScriptableObject 생성 완료.");
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

        Debug.Log("Seed ScriptableObject 생성 완료.");
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

        Debug.Log("Crop ScriptableObject 생성 완료.");
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

        Debug.Log("Fish ScriptableObject 생성 완료.");
    }

    void EnsureFolderExists(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string[] folders = path.Replace("Assets/", "").Split('/');
            string currentPath = "Assets";

            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder($"{currentPath}/{folder}"))
                {
                    AssetDatabase.CreateFolder(currentPath, folder);
                }
                currentPath += $"/{folder}";
            }
        }
    }
}
