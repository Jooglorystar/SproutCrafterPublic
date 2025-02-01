using System.IO;
using UnityEngine;
using UnityEditor;


public class InGameControlWindow : EditorWindow
{
    [Header("아이템 추가")]
    private string[] _ToolOptions;
    private string[] _EquipmentOptions;
    private string[] _FishOptions;
    private string[] _ConsumableOptions;
    private string[] _CropOptions;
    private string[] _SeedOptions;

    private int _toolIndex = 0;
    private int _equipmetnIndex = 0;
    private int _fishIndex = 0;
    private int _comsumableIndex = 0;
    private int _cropIndex = 0;
    private int _seedIndex = 0;

    private int _itemQuantity = 0;
    private int _cropStage = 0;
    
    private GUIStyle _labelStyle;

    
    [MenuItem("Window/Customizing/InGameControl")]
    public static void ShowWindow()
    {
        GetWindow<InGameControlWindow>("InGameControl");
    }


    private void OnEnable()
    {
        _labelStyle = new GUIStyle
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fontSize = 15
        };

        _ToolOptions = new string[] { "호미", "물뿌리개", "검" };
        _EquipmentOptions = new string[] { "미정", "미정", "미정" };
        _FishOptions = new string[] { "미정", "미정", "미정" };
        _ConsumableOptions = new string[] { "미정", "미정", "미정" };
        _CropOptions = new string[] { "미정", "미정", "미정" };
        _SeedOptions = new string[] { "미정", "미정", "미정" };
    }


    private void OnGUI()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DataBaseM.ItemDatabase.Init();
        }

        GUILayout.Space(10);
        GUILayout.Label("씬 이동", _labelStyle);
        GUILayout.Space(15);
        
        MoveScene();
        
        GUILayout.Space(10);
        GUILayout.Label("아이템 추가", _labelStyle);
        GUILayout.Space(15);

        AddItem();
        
        GUILayout.Space(10);
        GUILayout.Label("날짜, 계절 제어", _labelStyle);
        GUILayout.Space(15);

        ControlTime();

        GUILayout.Space(10);
        GUILayout.Label("작물 제어", _labelStyle);
        GUILayout.Space(15);
        ControlCrop();

        GUILayout.Space(15);
        GUILayout.Label("데이터 삭제", _labelStyle);

        DeleteData();
    }
    
    
    /// <summary>
    /// 씬 이동
    /// </summary>
    private void MoveScene()
    {
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Farm", GUILayout.Height(50)))
        {
            GameManager.Instance.SceneControlM.MoveSceneWithFade(SceneName.Farm, new Vector2(6f, -5.6f));
        }

        if (GUILayout.Button("Home", GUILayout.Height(50)))
        {
            GameManager.Instance.SceneControlM.MoveSceneWithFade(SceneName.Farm, new Vector2(65f, 11f));
        }

        if (GUILayout.Button("Bridge", GUILayout.Height(50)))
        {
            GameManager.Instance.SceneControlM.MoveSceneWithFade(SceneName.Bridge, new Vector2(10f, -1f));
        }

        if (GUILayout.Button("Village", GUILayout.Height(50)))
        {
            GameManager.Instance.SceneControlM.MoveSceneWithFade(SceneName.Village, new Vector2(0f, 0f));
        }

        if (GUILayout.Button("Market", GUILayout.Height(50)))
        {
            GameManager.Instance.SceneControlM.MoveSceneWithFade(SceneName.ResourcesPlace, new Vector2(3f, 7f));
        }
        
        GUILayout.EndHorizontal();
    }
    

# region 아이템 추가
    
    private void AddItem()
    {
        GUILayout.BeginHorizontal();
        _toolIndex = EditorGUILayout.Popup(_toolIndex, _ToolOptions);
        _equipmetnIndex = EditorGUILayout.Popup(_equipmetnIndex, _EquipmentOptions);
        _fishIndex = EditorGUILayout.Popup(_fishIndex, _FishOptions);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        _comsumableIndex = EditorGUILayout.Popup(_comsumableIndex, _ConsumableOptions);
        _cropIndex = EditorGUILayout.Popup(_cropIndex, _CropOptions);
        _seedIndex = EditorGUILayout.Popup(_seedIndex, _SeedOptions);
        GUILayout.EndHorizontal();
        
        _itemQuantity = EditorGUILayout.IntField("수량", _itemQuantity);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("툴 추가"))
        {
            InstantiateToolItem();
        }

        if (GUILayout.Button("장비 추가"))
        {
            InstantiateToolItem();
        }

        if (GUILayout.Button("물고기 추가"))
        {
            InstantiateToolItem();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("조합품 추가"))
        {
            InstantiateToolItem();
        }

        if (GUILayout.Button("작물 추가"))
        {
            InstantiateToolItem();
        }

        if (GUILayout.Button("씨앗 추가"))
        {
            InstantiateToolItem();
        }
        GUILayout.EndHorizontal();
    }
    
    
    private void AddItemByID(int p_ItemID)
    {
        ItemSO itemSo = GameManager.Instance.DataBaseM.ItemDatabase.GetByID(p_ItemID);

        if (itemSo != null)
        {
            GameManager.Instance.CharacterM.inventory.AcquireItem(itemSo, _itemQuantity);
        }
        else
        {
            Debug.Log("해당하는 아이템 코드가 없음");
        }
    }
    
    
    private void InstantiateToolItem()
    {
        switch (_toolIndex)
        {
            case 0:
                AddItemByID(1);
                break;
            case 1:
                AddItemByID(2);
                break;
            case 2:
                AddItemByID(4);
                break;
        }
    }
    
# endregion


# region 시간 제어
    
    /// <summary>
    /// 다음날로 이동
    /// </summary>
    private void EditorGoNextDay()
    {
        GameManager.Instance.TimeM.Sleep(6, GameManager.Instance.CharacterM.gameObject.transform.position);
    }
    

    private void ControlTime()
    {
        if (GUILayout.Button("다음날 진행"))
        {
            EditorGoNextDay();
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("1년차 봄", GUILayout.Height(50)))
        {
            GameManager.Instance.TimeM.SetInGameDay(-1);

            EditorGoNextDay();
        }
        if (GUILayout.Button("1년차 여름", GUILayout.Height(50)))
        {
            GameManager.Instance.TimeM.SetInGameDay(27);

            EditorGoNextDay();
        }
        if (GUILayout.Button("1년차 가을", GUILayout.Height(50)))
        {
            GameManager.Instance.TimeM.SetInGameDay(55);

            EditorGoNextDay();
        }
        if (GUILayout.Button("1년차 겨울", GUILayout.Height(50)))
        {
            GameManager.Instance.TimeM.SetInGameDay(83);

            EditorGoNextDay();
        }
        GUILayout.EndHorizontal();
    }
    
    #endregion

    
    private void ControlCrop()
    {
        if (GUILayout.Button("모든 타일에 물 주기"))
        {
            WaterAllTile();
        }

        GUILayout.BeginHorizontal();

        _cropStage = EditorGUILayout.IntField("단계", _cropStage);

        if (GUILayout.Button("모든 작물 설정"))
        {
            GameManager.Instance.CropM.DebugGrowthAllCrop(_cropStage);
        }

        GUILayout.EndHorizontal();
    }

    
    private void WaterAllTile()
    {
        foreach (var (tilePos, tileData) in GameManager.Instance.TilemapM.tileDataMap)
        {
            if (tileData.IsTilled) tileData.IsWatered = true;
        }
        GameManager.Instance.TilemapM.UpdateTile();
    }

    
    private void DeleteData()
    {
        if (GUILayout.Button("데이터 삭제"))
        {
            Directory.Delete(SaveManager.Instance.SaveSlotPath, true);
        }
    }
}