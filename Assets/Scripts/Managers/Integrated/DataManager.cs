using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[System.Serializable]
public class SystemData
{
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float ambientVolume;
    public float footStepVolume;
    public bool isMute;

    public int screenResolutionIndex;
    public bool isFullScreen;
}


[System.Serializable]
public class Slot
{
    public Slot(SlotType p_Type)
    {
        _slotType = p_Type;
    }

    public ItemSO itemSo;
    public int itemCount;
    private SlotType _slotType;
}


[System.Serializable]
public struct SlotSaveData
{
    public int itemCode;
    public int itemCount;
}


public sealed class DataManager : IInit
{
    [Header("시스템")]
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public float ambientVolume;
    public float footstepVolume;
    public bool isMute;
    
    public int screenResolutionIndex;
    public bool isFullScreen;

    [Header("스텟")]
    public int maxHp;
    public int currentHp;
    public int maxStamina;
    public int currentStamina;

    [Header("스킬")]
    public int currentLevel;
    public int currentExp;
    public int haveSkillPoint;
    public bool[] farmSkillActive;

    [Header("인벤토리")]
    public int gold;
    public Slot[] inventorySlots;
    public Slot[] equipmentSlots;
    public Slot[] combinationSlots;
    public Slot[] chestSlots;
    public Slot wasteBasket;
    public Slot sellSlot;

    [Header("UI 정보")]
    public string playerName = "Default";

    [Header("대화")]
    public List<Dialogue> currentDialogue;
    public NpcDataSO npcDataSO;

    public List<SaveChestData> chestDatas;

    
    public void Init()
    {
        SceneManagerEx.OnLoadCompleted(ResetData);
    }
    

    private void ResetData(Scene p_Scene, LoadSceneMode p_SceneMode)
    {
        if (p_SceneMode == LoadSceneMode.Additive) return;

        InitEtc();
        InitSlots();
        InitPlayerStat();
        InitPlayerSkill();
    }
    

    private void InitSlots()
    {
        inventorySlots = new Slot[30];
        equipmentSlots = new Slot[4];
        combinationSlots = new Slot[2];
        chestSlots = new Slot[20];
        wasteBasket = new Slot(SlotType.WasteBasket);
        sellSlot = new Slot(SlotType.Sell);


        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i] = new Slot(SlotType.Inventory);
        }


        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i] = new Slot(SlotType.Equipment);
        }


        for (int i = 0; i < combinationSlots.Length; i++)
        {
            combinationSlots[i] = new Slot(SlotType.Combination);
        }

        for (int i = 0; i < chestSlots.Length; i++)
        {
            chestSlots[i] = new Slot(SlotType.Chest);
        }
    }


    private void InitPlayerStat()
    {
        maxHp = 100;
        maxStamina = 150;

        currentLevel = 1;
        currentExp = 50;
        haveSkillPoint = 10;

        gold = 5000;
    }


    private void InitPlayerSkill()
    {
        farmSkillActive = new bool[12];
    }


    private void InitEtc()
    {
        currentDialogue = new List<Dialogue>();
    }
    

    #region 슬롯 저장용 메서드
    /// <summary>
    /// Slot의 데이터를 SlotSaveData 형태로 전환하는 메서드
    /// </summary>
    private SlotSaveData SaveSlot(Slot p_slots)
    {
        SlotSaveData saveData;
        if (p_slots.itemSo != null)
        {
            saveData.itemCode = (p_slots.itemSo.GetItemCode());
        }
        else
        {
            saveData.itemCode = 0;
        }
        saveData.itemCount = p_slots.itemCount;

        return saveData;
    }

    /// <summary>
    /// Slot 배열을 SlotSaveData 형태로 저장하는 메서드
    /// </summary>
    public SlotSaveData[] SaveSlots(Slot[] p_slots)
    {
        SlotSaveData[] slotSaveDatas = new SlotSaveData[p_slots.Length];

        for (int i = 0; i < p_slots.Length; i++)
        {
            slotSaveDatas[i] = SaveSlot(p_slots[i]);
        }

        return slotSaveDatas;
    }

    
    /// <summary>
    /// Slot에 SlotSaveData를 불러오는 메서드
    /// </summary>
    /// <param name="p_slot">불러올 슬롯</param>
    /// <param name="p_slotSaveData">불러올 데이터</param>
    private void LoadSlot(Slot p_slot, SlotSaveData p_slotSaveData)
    {
        if (p_slotSaveData.itemCode == 0) return;
        p_slot.itemSo = p_slotSaveData.itemCode.GetItemSo();
        p_slot.itemCount = p_slotSaveData.itemCount;
    }
    

    /// <summary>
    /// Slot배열에 LoadSlot을 사용하는 메서드
    /// </summary>
    public void LoadSlots(Slot[] p_slots, SlotSaveData[] p_slotSaveDatas)
    {
        for (int i = 0; i < p_slotSaveDatas.Length; i++)
        {
            LoadSlot(p_slots[i], p_slotSaveDatas[i]);
        }
    }
    
    #endregion
    
}