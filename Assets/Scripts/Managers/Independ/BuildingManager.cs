using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[System.Serializable]
public class BuildingSaveData
{
    public int buildingId;
    public Vector3Int buildingPosition;

    public BuildingSaveData(int p_buildingId, Vector3Int p_buildingPosition)
    {
        buildingId = p_buildingId;
        buildingPosition = p_buildingPosition;
    }
}

public class BuildingManager : MonoBehaviour
{
    public GridLayout gridLayout;
    public Tilemap mainTilemap;
    public Tilemap tempTilemap;

    [SerializeField] private TileBase[] _tileBases;

    private Building _objectToBuild;
    private Vector3 _prevPos;
    private BoundsInt _prevArea;

    private Camera _mainCamera;

    private Dictionary<Vector3Int, BuildingSO> _buildingDataDictionary = new Dictionary<Vector3Int, BuildingSO>();
    private Dictionary<Vector3Int, Building> _buildingObjectDictionary = new Dictionary<Vector3Int, Building>();

    private Dictionary<Vector3Int, Slot[]> _chestDictionary = new Dictionary<Vector3Int, Slot[]>();

    private void Awake()
    {
        GameManager.Instance.BuildingM = this;

        _mainCamera = Camera.main;

        _tileBases[0] = null;
    }

    #region 저장 로드 관련

    public List<BuildingSaveData> GetBuildingSaveDatas()
    {
        return SaveBuildingData();
    }

    private List<BuildingSaveData> SaveBuildingData()
    {
        List<BuildingSaveData> _buildingSaveDatas = new List<BuildingSaveData>();

        _buildingSaveDatas.Clear();
        foreach (var (buildingPosition, bulidingSO) in _buildingDataDictionary)
        {
            BuildingSaveData buildingSaveData = new BuildingSaveData(bulidingSO.itemCode, buildingPosition);
            _buildingSaveDatas.Add(buildingSaveData);
        }

        return _buildingSaveDatas;
    }

    public void LoadBuildingData(List<BuildingSaveData> p_buildingSaveDatas)
    {
        foreach (BuildingSaveData saveData in p_buildingSaveDatas)
        {
            if (GameManager.Instance.DataBaseM.ItemDatabase.GetByID(saveData.buildingId) is BuildingSO buildingSO)
            {
                _buildingDataDictionary.Add(saveData.buildingPosition, buildingSO);
            }
        }
    }

    // 이하 Chest관련

    public List<SaveChestData> GetSaveChestDatas()
    {
        return SaveChestData();
    }

    public List<SaveChestData> SaveChestData()
    {
        List<SaveChestData> _chestDatas = new List<SaveChestData>();

        _chestDatas.Clear();
        foreach (var (chestPosition, ChestSlots) in _chestDictionary)
        {
            SaveChestData saveChestData = new SaveChestData(chestPosition, ChestSlots);
            _chestDatas.Add(saveChestData);
        }

        return _chestDatas;
    }

    public void LoadChestData(List<SaveChestData> p_saveChestDatas)
    {
        foreach (SaveChestData saveData in p_saveChestDatas)
        {
            _chestDictionary.Add(saveData.chestID, saveData.SetSlot(saveData.itemCodes, saveData.itemQuantities));
        }
        LoadBuildings();
    }

    private void LoadBuildings()
    {
        foreach (var (buildingPosition, buildingData) in _buildingDataDictionary)
        {
            Building building = GameManager.Instance.PoolM.GetObject(PoolTag.Building).GetComponent<Building>();
            building.SetBuilingData(buildingData);
            building.transform.position = buildingPosition;
            FollowBuilding(building);
            Build(building);
        }
    }
    #endregion

    #region Chest 관련 메서드

    public void AddChestDictionary(Vector3Int p_position, Slot[] p_slots)
    {
        _chestDictionary.Add(p_position, p_slots);
    }

    public void RemoveChestDictionary(Vector3Int p_position)
    {
        _chestDictionary.Remove(p_position);
    }

    public bool IsHasChestData(Vector3Int p_position, out Slot[] p_slots)
    {
        if (_chestDictionary.TryGetValue(p_position, out Slot[] slots))
        {
            p_slots = slots;
            return true;
        }
        p_slots = null;
        return false;
    }

    #endregion

    #region 설치, 제거 및 취소 메서드
    private void Build(int p_selected, Building p_building)
    {
        if (p_building.CanBePlaced())
        {
            p_building.Place();
            _buildingObjectDictionary.Add(p_building.area.position, p_building);

            if (!_buildingDataDictionary.ContainsKey(p_building.area.position))
            {
                _buildingDataDictionary.Add(p_building.area.position, p_building.buildingData);
            }
            GameManager.Instance.CharacterM.inventory.SetSlotCount(p_selected, -1);
            _prevPos = Vector3.zero;
            _objectToBuild = null;
        }
        else
        {
            CancelBuild();
        }
    }

    private void Build(Building p_building)
    {
        if (p_building.CanBePlaced())
        {
            p_building.Place();
            _buildingObjectDictionary.Add(p_building.area.position, p_building);

            if (!_buildingDataDictionary.ContainsKey(p_building.area.position))
            {
                _buildingDataDictionary.Add(p_building.area.position, p_building.buildingData);
            }
            _objectToBuild = null;
        }
        else
        {
            CancelBuild();
        }
    }

    public void Remove(Vector3Int p_position)
    {
        if (_chestDictionary.ContainsKey(p_position))
        {
            for (int i = 0; i < _chestDictionary[p_position].Length; i++)
            {
                if (_chestDictionary[p_position][i].itemSo != null)
                {
                    Managers.Sound.PlaySfx(SfxEnums.ButtonClickNegative);
                    return;
                }

            }
        }
        if (_buildingDataDictionary.ContainsKey(p_position))
        {
            RemoveBuilding(_buildingObjectDictionary[p_position]);
        }
    }

    private void RemoveBuilding(Building p_building)
    {
        if (p_building.placed)
        {
            _buildingDataDictionary.Remove(p_building.area.position);
            GameManager.Instance.CharacterM.inventory.AcquireItem(p_building.buildingData);
            p_building.buildingData.Distroy(p_building);
            _buildingObjectDictionary.Remove(p_building.area.position);
            SetTilesBlock(p_building.area, 1, mainTilemap);
            ReleaseBuilding(p_building);
            Managers.Sound.PlaySfx(SfxEnums.BuildSelect);
            p_building.Remove();
        }
    }

    private void CancelBuild()
    {
        ClearArea();
        ReleaseBuilding(_objectToBuild);
        _objectToBuild = null;
    }

    private void ReleaseBuilding(Building p_building)
    {
        if (p_building == null) return;

        p_building.ResetPostion();
        GameManager.Instance.PoolM.ReleaseObject(PoolTag.Building, p_building.gameObject);
    }
    #endregion

    /// <summary>
    /// 건물 생성 메서드
    /// </summary>
    /// <param name="p_buildingData"></param>
    public void InitializeWithBuilding(int p_selected, BuildingSO p_buildingData)
    {
        if (_objectToBuild != null) return;
        if(mainTilemap == null || tempTilemap == null) return;

        Managers.Sound.PlaySfx(SfxEnums.Build);
        _objectToBuild = GameManager.Instance.PoolM.GetObject(PoolTag.Building).GetComponent<Building>();
        _objectToBuild.SetBuilingData(p_buildingData);
        FollowBuilding(_objectToBuild);
        MouseTracking(Mouse.current.position.ReadValue());
        Build(p_selected, _objectToBuild);
    }

    private void MouseTracking(Vector3 p_mousePosition)
    {
        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(p_mousePosition);
        Vector3Int cellPosition = gridLayout.LocalToCell(mousePosition);

        if (_prevPos != cellPosition)
        {
            _objectToBuild.transform.localPosition = gridLayout.CellToLocalInterpolated(cellPosition);
            _prevPos = cellPosition;
            FollowBuilding(_objectToBuild);
        }
    }

    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[GetBuildingSize(_prevArea)];

        FillTiles(toClear, 0);
        tempTilemap.SetTilesBlock(_prevArea, toClear);
    }

    private void FollowBuilding(Building p_building)
    {
        ClearArea();

        p_building.area.position = gridLayout.WorldToCell(p_building.gameObject.transform.position);
        BoundsInt buildingArea = p_building.area;

        TileBase[] baseArray = GetTileBlock(buildingArea, mainTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            Vector3Int tilePosition = GetPositionFromIndex(buildingArea, i);

            if (baseArray[i] == _tileBases[1] && !GameManager.Instance.CropM.cropStateDictionary.ContainsKey(tilePosition))
            {
                tileArray[i] = _tileBases[2];
            }
            else
            {
                FillTiles(tileArray, 3);
                break;
            }
        }
        tempTilemap.SetTilesBlock(buildingArea, tileArray);
        _prevArea = buildingArea;
    }

    private Vector3Int GetPositionFromIndex(BoundsInt p_buildingArea, int p_numberOfTile)
    {
        int counter = 0;
        foreach (Vector3Int position in p_buildingArea.allPositionsWithin)
        {
            if (counter == p_numberOfTile)
            {
                return position;
            }
            counter++;
        }
        throw new IndexOutOfRangeException();
    }

    public bool CanTakeArea(BoundsInt p_area)
    {
        TileBase[] baseArray = GetTileBlock(p_area, tempTilemap);
        foreach (TileBase a in baseArray)
        {
            if (a == _tileBases[3])
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 타일 설치 관련 메서드
    /// </summary>
    /// <param name="p_area"></param>
    public void TakeArea(BoundsInt p_area)
    {
        SetTilesBlock(p_area, 0, tempTilemap);
        SetTilesBlock(p_area, 2, mainTilemap);
    }

    public bool HasBuildingData(Vector3Int p_buildingPosition)
    {
        return _buildingDataDictionary.ContainsKey(p_buildingPosition);
    }

    #region 씬 이동간 타일 제어 관련
    public void ActivateBuildings()
    {
        foreach (var building in _buildingObjectDictionary)
        {
            building.Value.gameObject.SetActive(true);
        }
    }

    public void DeactivateBuildings()
    {
        foreach (var building in _buildingObjectDictionary)
        {
            if (building.Value == null) return;

            building.Value.gameObject.SetActive(false);
        }
    }
    #endregion

    #region 타일 제어 관련

    private TileBase[] GetTileBlock(BoundsInt p_area, Tilemap p_tilemap)
    {
        TileBase[] array = new TileBase[GetBuildingSize(p_area)];
        int counter = 0;

        foreach (Vector3Int v in p_area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = p_tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    private void SetTilesBlock(BoundsInt p_area, int p_tileNumber, Tilemap p_tilemap)
    {
        TileBase[] tileArray = new TileBase[GetBuildingSize(p_area)];
        FillTiles(tileArray, p_tileNumber);
        p_tilemap.SetTilesBlock(p_area, tileArray);
    }

    private void FillTiles(TileBase[] p_arr, int p_n)
    {
        for (int i = 0; i < p_arr.Length; i++)
        {
            p_arr[i] = _tileBases[p_n];
        }
    }

    #endregion

    private int GetBuildingSize(BoundsInt p_area)
    {
        return p_area.size.x * p_area.size.y * p_area.size.z;
    }
}