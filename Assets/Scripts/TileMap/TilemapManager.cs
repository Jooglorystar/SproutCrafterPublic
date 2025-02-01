using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
/// <summary>
/// 타일의 상태를 관리하는 데이터 클래스.
/// </summary>
public class TileData
{
    public bool IsTilled; // 갈린 땅 여부
    public bool IsWatered; // 물이 뿌려졌는지 여부
    public bool IsPlanted; // 작물이 심어진 여부
    public ResourceSO TreeResource;
    public bool CanPlantTree; // 나무를 심을 수 있는지 여부 (새로 추가)

    public TileData()
    {
        IsTilled = false;
        IsWatered = false;
        IsPlanted = false;
        TreeResource = null;
        CanPlantTree = false;
    }

    /// <summary>
    /// 타일이 기본 상태인지 확인합니다.
    /// </summary>
    public bool IsDefaultState => !IsTilled && !IsWatered && !IsPlanted && TreeResource == null;
}

public class TilemapManager : MonoBehaviour
{
    #region 인스펙터 변수

    [Header("타일맵 설정")]
    public Tilemap GroundTilemap; // 땅 타일맵
    public Tilemap WaterTilemap; // 물 타일맵
    public Tilemap CropTilemap; // 작물 타일맵

    [Header("땅 타일 설정")]
    public TileBase SoilTile; // 일반 땅 타일
    public TileBase PlowedTile; // 갈린 땅 타일
    public TileBase WateredTile; // 물이 뿌려진 땅 타일

    [Header("나무 타일 설정")]
    public Tilemap TreeGroundTilemap; // 나무 타일맵

    [Header("낚시 타일 설정")]
    public Tilemap WaterCanFishing;
    public Tilemap GroundCanFishing;
    #endregion

    #region 공개, 비공개 변수

    /// <summary>
    /// 모든 타일 데이터를 저장하는 딕셔너리. 키는 타일의 위치입니다.
    /// </summary>
    public Dictionary<Vector3Int, TileData> tileDataMap = new Dictionary<Vector3Int, TileData>();

    private float TillResetRandom = 0.3f; // 땅 상태 초기화 확률

    private string SaveFilePath => Path.Combine(SaveManager.Instance.SaveSlotPath, $"Farm_TileData.json");

    private List<Vector3Int> modifiedTiles = new List<Vector3Int>();
    
    private bool _hasTilemap = true;
    private bool _hasWaterTilemap = false;
    
    public bool HasTilemap { get { return _hasTilemap; } set { _hasTilemap = value; } }
    public bool HasWaterTilemap { get { return _hasWaterTilemap; } set { _hasWaterTilemap = value; } }
    

    #endregion

    #region Unity 생명주기 메서드

    /// <summary>
    /// 초기화 메서드. 타일 데이터를 초기화하거나 저장된 데이터를 로드합니다.
    /// </summary>
    private void Awake()
    {
        GameManager.Instance.TilemapM = this;
    }

    #endregion

    #region 초기화 메서드

    /// <summary>
    /// 타일 데이터를 초기화합니다.
    /// </summary>
    private void InitializeTileData()
    {
        foreach (Vector3Int position in GroundTilemap.cellBounds.allPositionsWithin)
        {
            if (!GroundTilemap.HasTile(position)) continue;

            tileDataMap[position] = new TileData();
            UpdateTileVisual(position);
        }
    }

    public void UpdateTile()
    {
        foreach (var (tilePosition, tileData) in tileDataMap)
        {
            UpdateTileVisual(tilePosition);
        }
    }

    #endregion

    #region 타일 데이터 관리

    /// <summary>
    /// 타일 데이터를 JSON 파일로 저장합니다.
    /// </summary>
    public void SaveTileData()
    {
        List<TileDataSave> modifiedData = new List<TileDataSave>();
        foreach (var position in modifiedTiles)
        {
            if (tileDataMap.TryGetValue(position, out TileData data) && !data.IsDefaultState)
            {
                modifiedData.Add(new TileDataSave { Position = position, Data = data });
            }
        }

        File.WriteAllText(SaveFilePath, JsonUtility.ToJson(new TileDataContainer { Data = modifiedData }));
        modifiedTiles.Clear();
    }

    /// <summary>
    /// JSON 파일에서 타일 데이터를 로드합니다.
    /// </summary>
    public void LoadTileData()
    {
        if (!File.Exists(SaveFilePath)) return;

        string json = File.ReadAllText(SaveFilePath);
        TileDataContainer container = JsonUtility.FromJson<TileDataContainer>(json);

        foreach (TileDataSave tile in container.Data)
        {
            tileDataMap[tile.Position] = tile.Data;
            UpdateTileVisual(tile.Position);
        }
    }

    public TileData GetTileData(Vector3Int position)
    {
        return tileDataMap.TryGetValue(position, out TileData data) ? data : new TileData();
    }

    #endregion

    #region 타일 상태 변경 메서드

    /// <summary>
    /// 아침이 되면 모든 타일 상태를 리셋합니다.
    /// </summary>
    public void ResetTileStateAtMorning()
    {
        //int seedValue = System.DateTime.Now.DayOfYear; // 시드 설정
        //Random.InitState(seedValue);

        foreach (var position in tileDataMap.Keys)
        {
            if (tileDataMap.TryGetValue(position, out TileData tileData))
            {
                tileData.IsWatered = false;

                // 갈린 땅 초기화 (나무가 없는 경우에만)
                if (tileData.IsTilled && !tileData.IsPlanted && tileData.TreeResource == null)
                {
                    if (Random.Range(0f, 1f) <= TillResetRandom)
                        tileData.IsTilled = false;
                }
                modifiedTiles.Add(position);
                UpdateTileVisual(position);
            }
        }
    }

    /// <summary>
    /// 타일을 갈린 상태로 변경합니다.
    /// </summary>
    public void TillTile(Vector3Int position)
    {
        if (!GroundTilemap.HasTile(position))
            return;

        if (!tileDataMap.ContainsKey(position))
            tileDataMap[position] = new TileData();

        TileData tileData = tileDataMap[position];
        tileData.IsTilled = true;

        modifiedTiles.Add(position);
        UpdateTileVisual(position);
        
        Managers.Sound.PlaySfx(SfxEnums.HitGrass);
    }

    
    /// <summary>
    /// 타일이 갈려있는지 반환하는 메서드
    /// </summary>
    public bool CheckNotTilledTile(Vector3Int position)
    {
        if (!GroundTilemap.HasTile(position)) return false;

        if (tileDataMap.TryGetValue(position, out TileData tileData))
        {
            if (tileData.IsTilled) return false;
        }

        return true;
    }

    /// <summary>
    /// 타일에 물을 뿌립니다.
    /// </summary>
    public void WaterTile(Vector3Int position)
    {
        if (tileDataMap.TryGetValue(position, out TileData tileData) && tileData.IsTilled)
        {
            tileData.IsWatered = true;

            modifiedTiles.Add(position);
            UpdateTileVisual(position);
        }
    }


    public bool CheckNotWateredTile(Vector3Int position)
    {
        if (tileDataMap.TryGetValue(position, out TileData tileData) && tileData.IsTilled)
        {
            return true;
        }
        
        return false;
    }
    
    
    /// <summary>
    /// 타일이 갈려있는지 반환하는 메서드
    /// </summary>
    public bool CheckCanFishingTile(Vector3Int position)
    {
        return WaterCanFishing.HasTile(position);
    }
    

    /// <summary>
    /// 타일의 시각적 상태를 업데이트합니다.
    /// </summary>
    public void UpdateTileVisual(Vector3Int position)
    {
        if (!tileDataMap.TryGetValue(position, out TileData tileData)) return;

        // 땅 상태 업데이트
        GroundTilemap.SetTile(position, tileData.IsTilled ? PlowedTile : SoilTile);
        WaterTilemap.SetTile(position, tileData.IsWatered ? WateredTile : null);
        GroundTilemap.RefreshTile(position);
        WaterTilemap.RefreshTile(position);

    }

    /// <summary>
    /// 특정 위치에 나무를 심는 메서드
    /// </summary>
    /// <param name="position">나무를 심을 타일의 위치</param>
    /// <param name="treeResource">심을 나무의 ResourceSO</param>
    public void PlantTreeOnTile(Vector3Int position, ResourceSO treeResource)
    {
        // 타일이 존재하고 나무를 심을 수 있는 상태인지 확인
        if (!tileDataMap.ContainsKey(position) || !tileDataMap[position].CanPlantTree)
        {
            Debug.LogWarning($"타일 {position}에 나무를 심을 수 없습니다.");
            return;
        }

        TileData tileData = tileDataMap[position];

        // 이미 나무가 심어진 상태라면 무시
        if (tileData.TreeResource != null)
        {
            Debug.LogWarning($"타일 {position}에 이미 나무가 심어져 있습니다.");
            return;
        }

        // 타일 데이터를 업데이트
        tileData.TreeResource = treeResource;
        modifiedTiles.Add(position);

        // TreeManager를 호출하여 나무를 생성
        GameManager.Instance.TreeM.PlantTree(position, treeResource);

        Debug.Log($"타일 {position}에 나무 {treeResource.itemName}를 심었습니다.");
    }
    #endregion

    #region 타일 상태 조회 메서드

    /// <summary>
    /// 갈린 타일의 위치를 반환합니다.
    /// </summary>
    public List<Vector3Int> GetTilledTiles() => GetTilesByCondition(tile => tile.IsTilled);

    /// <summary>
    /// 물을 뿌린 타일의 위치를 반환합니다.
    /// </summary>
    public List<Vector3Int> GetWateredTiles() => GetTilesByCondition(tile => tile.IsWatered);

    /// <summary>
    /// 씨앗이 심어진 타일의 위치를 반환합니다.
    /// </summary>
    public List<Vector3Int> GetPlantedSeeds() => GetTilesByCondition(tile => tile.IsPlanted);

    /// <summary>
    /// 특정 조건을 만족하는 타일의 위치를 반환합니다.
    /// </summary>
    private List<Vector3Int> GetTilesByCondition(System.Func<TileData, bool> condition)
    {
        List<Vector3Int> result = new List<Vector3Int>();
        foreach (KeyValuePair<Vector3Int, TileData> pair in tileDataMap)
        {
            if (condition(pair.Value)) result.Add(pair.Key);
        }
        return result;
    }

    /// <summary>
    /// 현재 모든 타일 상태를 반환합니다.
    /// </summary>
    public List<TileDataSave> GetTileStates()
    {
        List<TileDataSave> data = new List<TileDataSave>();
        foreach (KeyValuePair<Vector3Int, TileData> pair in tileDataMap)
        {
            data.Add(new TileDataSave { Position = pair.Key, Data = pair.Value });
        }
        return data;
    }

    /// <summary>
    /// 저장된 타일 상태를 불러와 설정합니다.
    /// </summary>
    public void SetTileStates(List<TileDataSave> tileStates)
    {
        foreach (TileDataSave tile in tileStates)
        {
            tileDataMap[tile.Position] = tile.Data;
            UpdateTileVisual(tile.Position);
        }
    }

    #endregion

    public bool IsNullTilemap()
    {
        if (GroundTilemap == null && WaterTilemap == null)
        {
            return true;
        }
        return false;
    }

    #region 직렬화 클래스

    [System.Serializable]
    /// <summary>
    /// 타일 데이터를 저장하기 위한 클래스.
    /// </summary>
    public class TileDataSave
    {
        public Vector3Int Position;
        public TileData Data;
    }

    [System.Serializable]
    /// <summary>
    /// 타일 데이터 컨테이너 클래스.
    /// </summary>
    public class TileDataContainer
    {
        public List<TileDataSave> Data;
    }

    #endregion
}