using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// 게임 상태를 저장하고 불러오는 매니저
/// </summary>
public class SaveManager : ConvertToSingleton<SaveManager>
{
    #region 필드 및 프로퍼티
    // 저장 파일 경로
    [SerializeField] private int _saveSlot = 0;

    public static new SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("SaveManager").AddComponent<SaveManager>();
            }
            return _instance;
        }
    }

    public int SaveSlot
    {
        get { return _saveSlot; }
        set { _saveSlot = value; }
    }
    

    public string SaveSlotPath => Path.Combine(Application.persistentDataPath, _saveSlot.ToString());
    private string SaveFilePath => Path.Combine(SaveSlotPath, "GameSave.json");
    #endregion

    #region Unity 메서드

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        LoadSystemData();
    }

    #endregion

# region 슬롯별 데이터 확인
    

    public void SelectSaveSlotNumber(int p_saveSlotNumber)
    {
        _saveSlot = p_saveSlotNumber;
        
        SetSaveSlotFolder(SaveSlotPath);
    }
    
    
    private void SetSaveSlotFolder(string p_saveSlotPath)
    {
        if (!Directory.Exists(p_saveSlotPath))
        {
            Directory.CreateDirectory(p_saveSlotPath);
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2, LoadSceneMode.Additive);
    }
    
# endregion
    

    #region 저장 및 불러오기 메서드
    /// <summary>
    /// 게임 상태를 저장합니다.
    /// </summary>
    public void SaveGame()
    {
        #region SaveData 생성
        SaveData saveData = new SaveData
        {
            // 1. 플레이어 상태 저장
            PlayerPosition = GameManager.Instance.CharacterM.transform.position,
            PlayerName = Managers.Data.playerName,

            // 스탯
            maxHp = Managers.Data.maxHp,
            maxStamina = Managers.Data.maxStamina,

            // 스킬
            currentLevel = Managers.Data.currentLevel,
            currentExp = Managers.Data.currentExp,
            haveSkillPoint = Managers.Data.haveSkillPoint,
            farmSkillActive = Managers.Data.farmSkillActive,

            // 인벤토리
            PlayerGold = Managers.Data.gold,
            inventorySlots = Managers.Data.SaveSlots(Managers.Data.inventorySlots),
            equipmentSlots = Managers.Data.SaveSlots(Managers.Data.equipmentSlots),
            combinationSlots = Managers.Data.SaveSlots(Managers.Data.combinationSlots),
            wasteBasket = Managers.Data.wasteBasket,
            sellSlot = Managers.Data.sellSlot,

            Quests = GameManager.Instance.QuestM.SaveQuest(),

            // 2. 세계 상태 저장
            TileStates = GameManager.Instance.TilemapM.GetTileStates(),
            cropSaveDatas = GameManager.Instance.CropM.SaveCropSaveData(),
            treeSaveDatas = GameManager.Instance.TreeM.GetTreeSaveDatas(),
            buildingSaveDatas = GameManager.Instance.BuildingM.GetBuildingSaveDatas(),
            chestDatas = GameManager.Instance.BuildingM.GetSaveChestDatas(),
            CurrentDay = GameManager.Instance.TimeM.InGameTotalDay - 1,
            CurrentWeather = "Sunny", // 날씨 정보는 나중에 추가
            cropPriceDatas = GameManager.Instance.DynamicPricingSystem.SavePriceDatas()
        };
        #endregion

        #region 데이터 저장
        // JSON 형식으로 직렬화 후 파일로 저장
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SaveFilePath, json);
        
        // SaveData 객체 해제
        saveData = null;
        //Debug.Log($"게임 상태가 저장되었습니다: {SaveFilePath}");
        #endregion
    }


    public void LoadTitleInfo(int p_SaveSlotNumber)
    {
        _saveSlot = p_SaveSlotNumber;
        
        if (File.Exists(SaveFilePath))
        {
            string json = File.ReadAllText(SaveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            
            Managers.Data.playerName = saveData.PlayerName;
            Managers.Data.gold = saveData.PlayerGold;
            Managers.Data.currentLevel = saveData.currentLevel;
        }
    }
    

    /// <summary>
    /// 게임 상태를 불러옵니다.
    /// </summary>
    public void LoadGame()
    {
        if (!File.Exists(SaveFilePath))
        {
            //Debug.LogWarning("저장된 게임 데이터가 없습니다.");
            SaveGame();
            return;
        }

        #region 데이터 불러오기
        // JSON 형식의 데이터를 읽어와 역직렬화
        string json = File.ReadAllText(SaveFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        #endregion
        GameManager.Instance.DataBaseM.ItemDatabase.Init();
        #region 데이터 복원
        
        // 1. 플레이어 상태 복원
        GameManager.Instance.CharacterM.transform.position = saveData.PlayerPosition;

        Managers.Data.maxHp = saveData.maxHp;
        Managers.Data.maxStamina = saveData.maxStamina;

        Managers.Data.playerName = saveData.PlayerName;
        
        Managers.Data.currentLevel = saveData.currentLevel;
        Managers.Data.currentExp = saveData.currentExp;
        Managers.Data.haveSkillPoint = saveData.haveSkillPoint;
        Managers.Data.farmSkillActive = saveData.farmSkillActive;

        Managers.Data.gold = saveData.PlayerGold;
        Managers.Data.LoadSlots(Managers.Data.inventorySlots, saveData.inventorySlots);
        Managers.Data.LoadSlots(Managers.Data.equipmentSlots, saveData.equipmentSlots);
        Managers.Data.LoadSlots(Managers.Data.combinationSlots, saveData.combinationSlots);
        Managers.Data.wasteBasket = saveData.wasteBasket;
        Managers.Data.sellSlot = saveData.sellSlot;

        // 2. 세계 상태 복원
        GameManager.Instance.TilemapM.SetTileStates(saveData.TileStates);
        GameManager.Instance.TimeM.SetInGameDay(saveData.CurrentDay);
        GameManager.Instance.CropM.LoadFromCropSaveData(saveData.cropSaveDatas);
        GameManager.Instance.TreeM.SetTreeSaveDatas(saveData.treeSaveDatas);
        GameManager.Instance.TreeM.LoadTreeGameState();
        GameManager.Instance.BuildingM.LoadBuildingData(saveData.buildingSaveDatas);
        GameManager.Instance.BuildingM.LoadChestData(saveData.chestDatas);
        GameManager.Instance.DynamicPricingSystem.LoadPriceDatas(saveData.cropPriceDatas);
        // 3. 진행 데이터 복원
        //GameManager.Instance.QuestM.SetQuestProgress(saveData.QuestProgress);
        GameManager.Instance.QuestM.LoadQuest(saveData.Quests);
        // SaveData 객체 해제
        saveData = null;
        // Debug.Log($"게임 상태가 복원되었습니다: {SaveFilePath}");
        #endregion
    }
    #endregion


    private void SaveSystemData()
    {
        SystemData systemData = new SystemData
        {
            masterVolume = Managers.Data.masterVolume,
            musicVolume = Managers.Data.musicVolume,
            sfxVolume = Managers.Data.sfxVolume,
            ambientVolume = Managers.Data.ambientVolume,
            footStepVolume = Managers.Data.footstepVolume,
            isMute  = Managers.Data.isMute,

            screenResolutionIndex = Managers.Data.screenResolutionIndex,
            isFullScreen = Managers.Data.isFullScreen
        };
        
        string json = JsonUtility.ToJson(systemData, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "SystemData.json") , json);
    }


    private void LoadSystemData()
    {
        string path = Path.Combine(Application.persistentDataPath, "SystemData.json");
        
        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);
        SystemData systemData = JsonUtility.FromJson<SystemData>(json);

        Managers.Data.masterVolume = systemData.masterVolume;
        Managers.Data.musicVolume = systemData.musicVolume;
        Managers.Data.sfxVolume = systemData.sfxVolume;
        Managers.Data.ambientVolume = systemData.ambientVolume;
        Managers.Data.footstepVolume = systemData.footStepVolume;
        Managers.Data.isMute = systemData.isMute;
        
        Managers.Data.screenResolutionIndex = systemData.screenResolutionIndex;
        Managers.Data.isFullScreen = systemData.isFullScreen;

        Managers.Sound.MuteAllVolume(systemData.isMute);
    }


    private void OnDisable()
    {
        SaveSystemData();
    }
}