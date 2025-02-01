using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;

[Serializable]
public class TreeState
{
    public int growthStage; // 성장 단계
    public int growingDate; // 성장 날짜
    public ResourceSO resourceSO; // 관련 자원 정보
    public SpriteResolver spriteResolver; // 스프라이트 관리
    public bool isPlanted; // 심어졌는지 여부
}

[Serializable]
public class TreeSaveData
{
    public Vector3Int treePosition; // 트리 위치
    public int growthStage; // 성장 단계
    public int growingDate; // 성장 날짜
    public int treeItemCode; // 관련 아이템 코드

    public TreeSaveData(Vector3Int p_position, TreeState p_treeState)
    {
        treePosition = p_position;
        growthStage = p_treeState.growthStage;
        growingDate = p_treeState.growingDate;
        treeItemCode = p_treeState.resourceSO.itemCode;
    }
}

public class TreeManager : MonoBehaviour
{
    [Header("나무 타일맵")]
    public Tilemap treeGroundTilemap; // 나무가 심어질 타일맵
    public TileBase plantableTile;    // 나무를 심을 수 있는 타일

    public Dictionary<Vector3Int, TreeState> treeStateDictionary = new Dictionary<Vector3Int, TreeState>();

    private List<TreeSaveData> treeSaveDatas = new List<TreeSaveData>(); // 트리 저장 데이터 리스트

    private Tilemap TreeGroundTilemap;
    private TilemapManager tilemapManager;
    private string currentSceneName;

    private void Awake()
    {
        GameManager.Instance.TreeM = this;
    }
    private void Start()
    {
        if (treeSaveDatas != null && treeSaveDatas.Count > 0)
        {
            // 저장된 데이터가 있으면 로드
            Debug.Log("저장된 나무 데이터를 로드합니다.");
            LoadTreeGameState();
        }
        else
        {
            // 저장된 데이터가 없으면 초기화
            // Debug.Log("저장된 데이터가 없으므로 나무를 초기화합니다.");
            InitializeTrees();
        }
    }
    #region 트리 저장 및 로드 관련 메서드

    public void SetTreeSaveDatas(List<TreeSaveData> p_treeSaveDatas)
    {
        treeSaveDatas = p_treeSaveDatas;
    }

    public List<TreeSaveData> GetTreeSaveDatas()
    {
        return treeSaveDatas;
    }

    /// <summary>
    /// 로드 시, Tree오브젝트를 다시 배치하는 메서드
    /// </summary>
    public void LoadTreeGameState()
    {
        GameManager.Instance.DataBaseM.ItemDatabase.Init();
        foreach (TreeSaveData treeSaveData in treeSaveDatas)
        {
            TreeState treeState = new TreeState();

            treeState.resourceSO = (ResourceSO)ItemCodeMapper.GetItemSo(treeSaveData.treeItemCode);
            treeState.growthStage = treeSaveData.growthStage;
            treeState.growingDate = treeSaveData.growingDate;

            GetTreeObject(treeSaveData.treePosition, treeState);
        }
    }
    /// <summary>
    /// 다른 씬에서 농장씬으로 돌아올 때 작물 재설치용 메서드
    /// </summary>
    public void LoadTreeFromTreeStateDictionary()
    {
        foreach (var (treePosition, treeState) in treeStateDictionary)
        {
            GetTreeObject(treePosition, treeState);
        }
    }

    /// <summary>
    /// 농장 씬에서 다른 씬 갈 때, 작물 다시 집어 넣는 메서드
    /// </summary>
    public void ReleaseTrees()
    {
        foreach (var (treePosition, treeState) in treeStateDictionary)
        {
            ReleaseTreeObject(treeState);
        }
    }
    public void PrepareForSceneTransition()
    {
        // 나무 상태를 저장하기 전에, treeStateDictionary를 기반으로 TreeSaveData 생성
        treeSaveDatas.Clear();
        foreach (var (treePosition, treeState) in treeStateDictionary)
        {
            TreeSaveData saveData = new TreeSaveData(treePosition, treeState);
            treeSaveDatas.Add(saveData);
        }

        Debug.Log($"TreeManager: {treeSaveDatas.Count}개의 나무 상태를 저장했습니다.");
    }

    public void RestoreAfterSceneTransition()
    {
        // 저장된 TreeSaveData를 기반으로 treeStateDictionary 및 나무 오브젝트 복원
        treeStateDictionary.Clear();
        foreach (var saveData in treeSaveDatas)
        {
            TreeState treeState = new TreeState
            {
                growthStage = saveData.growthStage,
                growingDate = saveData.growingDate,
                resourceSO = (ResourceSO)ItemCodeMapper.GetItemSo(saveData.treeItemCode),
                isPlanted = true // 이미 심어진 상태
            };

            treeStateDictionary[saveData.treePosition] = treeState;
        }

        // 나무 오브젝트를 실제로 재배치
        LoadTreeFromTreeStateDictionary();
        Debug.Log($"TreeManager: {treeSaveDatas.Count}개의 나무 상태를 복원했습니다.");
    }

    /// <summary>
    /// TreeState를 TreeSaveData로 변환하는 메서드
    /// </summary>
    /// <param name="p_treePosition"></param>
    /// <param name="p_treeState"></param>
    private void SetTreeSaveData(Vector3Int p_treePosition, TreeState p_treeState)
    {
        if (p_treeState == null)
        {
            foreach (TreeSaveData treeSaveData in treeSaveDatas)
            {
                if (treeSaveData.treePosition == p_treePosition)
                {
                    treeSaveDatas.Remove(treeSaveData);
                    break;
                }
            }
            return;
        }
        TreeSaveData existingTreeData = null;

        foreach (TreeSaveData treeSaveData in treeSaveDatas)
        {
            if (treeSaveData.treePosition == p_treePosition)
            {
                existingTreeData = treeSaveData;
                break;
            }
        }

        if (existingTreeData != null)
        {
            existingTreeData.growthStage = p_treeState.growthStage;
            existingTreeData.growingDate = p_treeState.growingDate;
            existingTreeData.treeItemCode = p_treeState.resourceSO.itemCode;
        }
        else
        {
            TreeSaveData newCropData = new TreeSaveData(p_treePosition, p_treeState);

            treeSaveDatas.Add(newCropData);
        }
    }
    #endregion

    #region 트리 생성 및 제거 메서드
    /// <summary>
    /// 작물을 심는 메서드
    /// </summary>
    /// <param name="p_treePosition">심을 위치</param>
    /// <param name="p_tree">심을 작물 씨앗 SO</param>
    public void PlantTree(Vector3Int p_treePosition, ResourceSO p_tree)
    {
        TreeState treeState = new TreeState();

        treeState.resourceSO = p_tree;
        treeState.growthStage = 0;
        treeState.growingDate = p_tree.growTime - 1;

        GetTreeObject(p_treePosition, treeState);

        UpdateTileState(p_treePosition, true);
    }

    /// <summary>
    /// 크롭 오브젝트를 생성하고, 타일맵에 배치 후, 딕셔너리에 추가하는 메서드
    /// </summary>
    /// <param name="p_treePosition"></param>
    /// <param name="p_treeState"></param>
    private void GetTreeObject(Vector3Int p_treePosition, TreeState p_treeState)
    {
        if (p_treeState.isPlanted) return;

        // 오브젝트 풀에서 나무 오브젝트 가져오기
        GameObject treeObject = GameManager.Instance.PoolM.GetObject(PoolTag.TreeGrowthPrefab);
        p_treeState.isPlanted = true;

        // 나무 위치 설정
        treeObject.name = $"{p_treePosition.x}_{p_treePosition.y}";
        treeObject.transform.position = GameManager.Instance.TilemapM.GroundTilemap.GetCellCenterWorld(p_treePosition);

        // SpriteResolver 설정
        SpriteResolver spriteResolver = treeObject.GetComponent<SpriteResolver>();
        p_treeState.spriteResolver = spriteResolver;

        // ResourceInteraction 설정 및 ResourceSO 전달
        ResourceInteraction resourceInteraction = treeObject.GetComponent<ResourceInteraction>();
        if (resourceInteraction != null)
        {
            resourceInteraction.Initialize(p_treeState.resourceSO);
        }

        // 딕셔너리에 추가
        if (!treeStateDictionary.ContainsKey(p_treePosition))
            treeStateDictionary.Add(p_treePosition, p_treeState);

        // TileDataMap 갱신
        GameManager.Instance.TilemapM.tileDataMap[p_treePosition].TreeResource = p_treeState.resourceSO;

        // 나무 스프라이트 업데이트
        UpdateTreeSprite(p_treePosition);
    }


    /// <summary>
    /// 오브젝트풀에서 생성된 Crop을 오브젝트풀로 반환하는 메서드 
    /// </summary>
    /// <param name="p_cropPosition">지울 crop의 위치</param>
    public ItemSO RemoveCrop(Vector3Int p_cropPosition)
    {
        if (treeStateDictionary.TryGetValue(p_cropPosition, out TreeState treeState))
        {
            ItemSO harvestedCrop = treeState.resourceSO.linkedItem;

            // 오브젝트를 오브젝트풀로 반환
            ReleaseTreeObject(treeState);

            // 상태 사전에서 제거
            treeStateDictionary.Remove(p_cropPosition);

            GameManager.Instance.TilemapM.tileDataMap[p_cropPosition].IsPlanted = false;

            UpdateTileState(p_cropPosition, false);

            // 수확한 작물 데이터를 반환
            return harvestedCrop;
        }

        return null; // 만약 해당 위치에 작물이 없다면 null 반환

    }

    private void ReleaseTreeObject(TreeState p_treeState)
    {
        if (p_treeState.spriteResolver == null) return;

        GameManager.Instance.PoolM.ReleaseObject(PoolTag.TreeGrowthPrefab, p_treeState.spriteResolver.gameObject);
        p_treeState.isPlanted = false;
    }
    #endregion

    #region 트리 상태 갱신 메서드
    /// <summary>
    /// 작물의 상태를 업데이트함
    /// </summary>
    public void UpdateGrowth()
    {
        foreach (var (tilePosition, tileData) in GameManager.Instance.TilemapM.tileDataMap)
        {
            if (treeStateDictionary.TryGetValue(tilePosition, out TreeState treeState))
            {
                if (tileData.IsWatered)
                {
                    treeState.growingDate++;

                    if (treeState.growingDate >= treeState.resourceSO.growTime && treeState.growthStage < treeState.resourceSO.maxGrowthStage)
                    {
                        treeState.growthStage++;
                        treeState.growingDate = 0;
                    }
                }
                UpdateTreeSprite(tilePosition);
            }
            SetTreeSaveData(tilePosition, treeState);
        }
    }
    /* 보류
    /// <summary>
    /// 계절이 맞지 않을 시 작물이 썩음
    /// </summary>
    public void CheckDecayTree(TreeState p_treeState)
    {
        if (p_treeState.growthStage > 0 && p_treeState.resourceSO.plantingSeasons != GameManager.Instance.TimeM.CurrentSeason)
        {
            p_treeState.spriteResolver.SetCategoryAndLabel("DecayCrop", "0");
        }
    }
    */

    /// <summary>
    /// 작물의 sprite를 업데이트함
    /// </summary>
    /// <param name="p_treePosition"></param>
    private void UpdateTreeSprite(Vector3Int p_treePosition)
    {
        if (treeStateDictionary.TryGetValue(p_treePosition, out TreeState treeState))
        {
            // ResourceSO가 null이면 기본값으로 설정
            if (treeState.resourceSO == null)
            {
                Debug.LogWarning($"ResourceSO가 null입니다. 기본값을 사용합니다. Position: {p_treePosition}");
                treeState.resourceSO = GameManager.Instance.DataBaseM.ItemDatabase.GetDefaultResourceSO();
            }

            // SpriteResolver를 업데이트
            if (treeState.spriteResolver != null)
            {
                treeState.spriteResolver.SetCategoryAndLabel(treeState.resourceSO.itemName, treeState.growthStage.ToString());
            }
            else
            {
                Debug.LogError($"SpriteResolver가 null입니다. Position: {p_treePosition}");
            }
        }
        else
        {
            Debug.LogWarning($"TreeState를 찾을 수 없습니다. Position: {p_treePosition}");
        }
    }
    #endregion

    #region TreeGround 타일맵 탐지 추가
    private bool IsPlantableTile(Vector3Int position)
    {
        TileBase tile = treeGroundTilemap.GetTile(position);

        // 현재 타일이 나무를 심을 수 있는 타일인지 확인
        return tile == plantableTile;
    }
    public void InitializeTrees()
    {
        if (treeSaveDatas != null && treeSaveDatas.Count > 0)
        {
            Debug.LogWarning("저장된 데이터가 있으므로 초기화를 건너뜁니다.");
            return;
        }
        int maxTrees = 10; // 최대 나무 개수
        int placedTrees = 0; // 현재 배치된 나무 개수
        List<Vector3Int> availablePositions = new List<Vector3Int>(); // 배치 가능한 위치 목록

        // 심을 수 있는 모든 타일 위치를 수집
        foreach (Vector3Int position in treeGroundTilemap.cellBounds.allPositionsWithin)
        {
            if (IsPlantableTile(position))
            {
                availablePositions.Add(position);
            }
        }

        // 랜덤으로 위치를 섞음
        System.Random random = new System.Random();
        availablePositions = availablePositions.OrderBy(pos => random.Next()).ToList();

        foreach (Vector3Int position in availablePositions)
        {
            // 이미 최대 개수를 배치했다면 종료
            if (placedTrees >= maxTrees) break;

            // 거리 조건을 만족하는지 확인
            if (IsValidTreePosition(position))
            {
                GameObject treeObject = GameManager.Instance.PoolM.GetObject(PoolTag.TreeGrowthPrefab);

                treeObject.name = $"Tree_{position.x}_{position.y}";
                treeObject.transform.position = treeGroundTilemap.GetCellCenterWorld(position);

                // 기본 성장 단계와 날짜를 설정
                int defaultGrowthStage = UnityEngine.Random.Range(1, 3); // 1단계에서 2단계 사이 랜덤
                int defaultGrowingDate = UnityEngine.Random.Range(0, 2); // 랜덤 성장 날짜
                ResourceSO defaultResourceSO = GameManager.Instance.DataBaseM.ItemDatabase.GetDefaultResourceSO(); // 기본 ResourceSO 가져오기

                TreeState treeState = new TreeState
                {
                    growthStage = defaultGrowthStage, // 성장 단계
                    growingDate = defaultGrowingDate, // 성장 날짜
                    resourceSO = defaultResourceSO,   // 기본 ResourceSO
                    spriteResolver = treeObject.GetComponent<SpriteResolver>(),
                    isPlanted = true
                };

                if (!treeStateDictionary.ContainsKey(position))
                {
                    treeStateDictionary.Add(position, treeState);
                }

                // 나무 스프라이트 업데이트
                UpdateTreeSprite(position);

                // 타일 상태 업데이트
                UpdateTileState(position, true);

                placedTrees++; // 배치된 나무 개수 증가
            }
        }

        // Debug.Log($"총 {placedTrees}개의 나무가 배치되었습니다.");
    }

    // 특정 위치가 기존 나무들로부터 충분히 떨어져 있는지 확인하는 메서드
    private bool IsValidTreePosition(Vector3Int newPosition)
    {
        foreach (var existingPosition in treeStateDictionary.Keys)
        {
            float distance = Vector3Int.Distance(newPosition, existingPosition);

            // 3칸 이내에 다른 나무가 있으면 배치 불가
            if (distance < 3)
            {
                return false;
            }
        }
        return true;
    }


    private void UpdateTileState(Vector3Int position, bool isPlanted)
    {
        if (isPlanted)
        {
            // 나무가 심어졌다면 타일을 '사용 중'으로 설정
            treeGroundTilemap.SetTileFlags(position, TileFlags.None);
            treeGroundTilemap.SetColor(position, Color.green);
        }
        else
        {
            // 나무가 제거되었다면 타일을 초기화
            treeGroundTilemap.SetTileFlags(position, TileFlags.None);
            treeGroundTilemap.SetColor(position, Color.white);
        }
    }

    #endregion
}