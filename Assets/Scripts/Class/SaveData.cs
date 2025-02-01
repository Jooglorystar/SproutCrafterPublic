using static TilemapManager;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [Header("플레이어")]
    public Vector3 PlayerPosition;
    public string PlayerName;
    public int PlayerGold;

    public int maxHp;
    public int maxStamina;

    public int currentLevel;
    public int currentExp;
    public int haveSkillPoint;
    public bool[] farmSkillActive;

    public SlotSaveData[] inventorySlots;
    public SlotSaveData[] equipmentSlots;
    public SlotSaveData[] combinationSlots;
    public Slot wasteBasket;
    public Slot sellSlot;

    
    [Header("세계 상태")]
    public List<TileDataSave> TileStates; // 타일 상태
    public List<CropSaveData> cropSaveDatas; // 작물 상태
    public List<TreeSaveData> treeSaveDatas; //나무상태 저장
    public List<BuildingSaveData> buildingSaveDatas;
    public List<SaveChestData> chestDatas;
    public int CurrentDay; // 현재 날짜
    public string CurrentSeason; // 현재 계절
    public string CurrentWeather; // 현재 날씨
    public List<CropPriceData> cropPriceDatas;
    // 진행 데이터
    public List<string> QuestProgress; // 퀘스트 진행도
    public List<QuestSaveData> Quests; // 진행중인 퀘스트

    // 플레이어 하루 행동 기록
    public List<Vector3Int> TilledTiles; // 경작한 타일
    public List<Vector3Int> WateredTiles; // 물을 준 타일
    public List<Vector3Int> PlantedSeeds; // 심은 씨앗 위치
}