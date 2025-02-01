using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

[System.Serializable]
public class CropState
{
    public int growthStage; // 성장 단계
    public int growingDate;
    public SeedSO seedSO;
    public SpriteResolver spriteResolver;

    public CropState(SeedSO p_crop)
    {
        seedSO = p_crop;
        growthStage = 0;
        growingDate = p_crop.growTime - 1;
    }

    public CropState(SeedSO p_crop, int p_growthStage, int p_growingData)
    {
        seedSO = p_crop;
        growthStage = p_growthStage;
        growingDate = p_growingData;
    }
}

[System.Serializable]
public struct CropSaveData
{
    public Vector3Int cropPosition;
    public byte growthStage;
    public byte growingDate;
    public int seedItemCode;

    public CropSaveData(Vector3Int p_cropPosition, CropState p_cropState)
    {
        cropPosition = p_cropPosition;
        growthStage = (byte)p_cropState.growthStage;
        growingDate = (byte)p_cropState.growingDate;
        seedItemCode = p_cropState.seedSO.itemCode;
    }
}

public class CropManager : MonoBehaviour
{
    public Dictionary<Vector3Int, CropState> cropStateDictionary = new Dictionary<Vector3Int, CropState>();

    private void Awake()
    {
        GameManager.Instance.CropM = this;
    }

    #region 작물 정보 저장, 로드 관련

    /// <summary>
    /// CropState를 CropSaveData로 변환하는 메서드
    /// </summary>
    public List<CropSaveData> SaveCropSaveData()
    {
        List<CropSaveData> cropSaveDatas = new List<CropSaveData>();

        foreach (var (cropPosition, cropState) in cropStateDictionary)
        {
            CropSaveData cropSaveData = new CropSaveData(cropPosition, cropState);

            cropSaveDatas.Add(cropSaveData);
        }

        return cropSaveDatas;
    }

    /// <summary>
    /// 로드 시, Crop오브젝트를 다시 배치하는 메서드
    /// </summary>
    public void LoadFromCropSaveData(List<CropSaveData> p_cropSaveDatas)
    {
        foreach (CropSaveData cropSaveData in p_cropSaveDatas)
        {
            CropState cropState = new CropState((SeedSO)ItemCodeMapper.GetItemSo(cropSaveData.seedItemCode), cropSaveData.growthStage, cropSaveData.growingDate);

            GetCropObject(cropSaveData.cropPosition, cropState);
        }
    }

    /// <summary>
    /// 다른 씬에서 농장씬으로 돌아올 때 작물 재설치용 메서드
    /// </summary>
    public void ActivateCrops()
    {
        if (SceneManager.GetActiveScene().name != "Farm") return;

        foreach (var (cropPosition, cropState) in cropStateDictionary)
        {
            ActivateCrop(cropState.spriteResolver);
        }
    }

    /// <summary>
    /// 농장 씬에서 다른 씬 갈 때, 작물 다시 집어 넣는 메서드
    /// </summary>
    public void DeactivateCrops()
    {
        foreach (var (cropPosition, cropState) in cropStateDictionary)
        {
            if (cropState.spriteResolver == null) return;

            DeactiveCrop(cropState.spriteResolver);
        }
    }

    private void ActivateCrop(SpriteResolver p_spriteResolver)
    {
        p_spriteResolver.gameObject.SetActive(true);
    }

    private void DeactiveCrop(SpriteResolver p_spriteResolver)
    {
        p_spriteResolver.gameObject.SetActive(false);
    }

    #endregion

    #region 작물 생성 및 제거 메서드

    /// <summary>
    /// 작물을 심는 메서드
    /// </summary>
    /// <param name="p_cropPosition">심을 위치</param>
    /// <param name="p_crop">심을 작물 씨앗 SO</param>
    public void PlantCrop(Vector3Int p_cropPosition, SeedSO p_crop)
    {
        CropState cropState = new CropState(p_crop);

        cropState.seedSO = p_crop;
        cropState.growthStage = 0;
        cropState.growingDate = p_crop.growTime - 1;

        Managers.Sound.PlaySfx(SfxEnums.CropPlant);

        GetCropObject(p_cropPosition, cropState);
    }

    /// <summary>
    /// 크롭 오브젝트를 생성하고, 타일맵에 배치 후, 딕셔너리에 추가하는 메서드
    /// </summary>
    /// <param name="p_cropPosition"></param>
    /// <param name="p_cropState"></param>
    private void GetCropObject(Vector3Int p_cropPosition, CropState p_cropState)
    {
        GameObject cropObject = GameManager.Instance.PoolM.GetObject(PoolTag.CropGrowthPrefab);
        cropObject.name = $"{p_cropPosition.x}_{p_cropPosition.y}";
        cropObject.transform.position = GameManager.Instance.TilemapM.GroundTilemap.GetCellCenterWorld(p_cropPosition);

        p_cropState.spriteResolver = cropObject.GetComponent<SpriteResolver>();

        if (!cropStateDictionary.ContainsKey(p_cropPosition))
            cropStateDictionary.Add(p_cropPosition, p_cropState);

        GameManager.Instance.TilemapM.tileDataMap[p_cropPosition].IsPlanted = true;

        UpdateCropSprite(p_cropPosition);
    }

    /// <summary>
    /// 작물을 수확하는 메서드
    /// </summary>
    /// <param name="p_cropPosition">지울 crop의 위치</param>
    public ItemSO HarvestCrop(Vector3Int p_cropPosition, out int p_cropAmount)
    {
        RemoveCrop(p_cropPosition, out CropState cropState);
        if (cropState != null)
        {
            ItemSO harvestedCrop = ItemCodeMapper.GetItemSo(cropState.seedSO.linkedCropCode);
            Managers.Sound.PlaySfx(SfxEnums.CropHarvest);
            p_cropAmount = Random.Range(cropState.seedSO.minYield, cropState.seedSO.maxYield + 1);
            return harvestedCrop;
        }
        p_cropAmount = 0;
        return null;
    }

    private void RemoveCrop(Vector3Int p_cropPosition, out CropState p_cropState)
    {
        if (cropStateDictionary.TryGetValue(p_cropPosition, out CropState cropState))
        {
            p_cropState = cropState;
            ReleaseCropObject(cropState);
            cropStateDictionary.Remove(p_cropPosition);
            GameManager.Instance.TilemapM.tileDataMap[p_cropPosition].IsPlanted = false;
        }
        else
        {
            p_cropState = null;
        }
    }

    private void RemoveCrop(Vector3Int p_cropPosition)
    {
        if (cropStateDictionary.TryGetValue(p_cropPosition, out CropState cropState))
        {
            ReleaseCropObject(cropState);
            cropStateDictionary.Remove(p_cropPosition);
            GameManager.Instance.TilemapM.tileDataMap[p_cropPosition].IsPlanted = false;
        }
    }

    private void ReleaseCropObject(CropState p_cropState)
    {
        GameManager.Instance.PoolM.ReleaseObject(PoolTag.CropGrowthPrefab, p_cropState.spriteResolver.gameObject);
    }

    #endregion

    #region 크롭 상태 갱신 관련 메서드
    /// <summary>
    /// 작물의 상태를 업데이트함
    /// </summary>
    public void UpdateGrowth()
    {
        foreach (var (tilePosition, tileData) in GameManager.Instance.TilemapM.tileDataMap)
        {
            if (cropStateDictionary.TryGetValue(tilePosition, out CropState cropState))
            {
                if (tileData.IsWatered)
                {
                    cropState.growingDate++;

                    if (cropState.growingDate >= cropState.seedSO.growTime && cropState.growthStage < cropState.seedSO.maxGrowthStage)
                    {
                        cropState.growthStage++;
                        cropState.growingDate = 0;
                    }
                }
                UpdateCropSprite(tilePosition);
            }
        }
    }

    /// <summary>
    /// 계절이 맞지 않을 시 작물이 썩음
    /// </summary>
    private void CheckDecayCrop(Vector3Int p_cropPosition, CropState p_cropState)
    {
        if (IsCropOutofSeason(p_cropState))
        {
            RemoveCrop(p_cropPosition);
        }
    }

    private bool IsCropOutofSeason(CropState p_cropState)
    {
        return p_cropState.growthStage > 0 && p_cropState.seedSO.plantingSeasons != GameManager.Instance.TimeM.CurrentSeason;
    }

    /// <summary>
    /// 작물의 sprite를 업데이트함
    /// </summary>
    /// <param name="p_cropPosition"></param>
    private void UpdateCropSprite(Vector3Int p_cropPosition)
    {
        if (cropStateDictionary.TryGetValue(p_cropPosition, out CropState cropState))
        {
            cropState.spriteResolver.SetCategoryAndLabel(cropState.seedSO.itemCode.ToString(), cropState.growthStage.ToString());
        }
        CheckDecayCrop(p_cropPosition, cropState);
    }
    #endregion

    /// <summary>
    /// 모든 작물을 해당하는 단계로 성장하게 하는 디버그용 메서드입니다. 호출 주의
    /// </summary>
    public void DebugGrowthAllCrop(int p_stage)
    {
        foreach (var (tilePosition, tileData) in GameManager.Instance.TilemapM.tileDataMap)
        {
            if (cropStateDictionary.TryGetValue(tilePosition, out CropState cropState))
            {
                cropState.growthStage = Mathf.Clamp(p_stage, 0, cropState.seedSO.maxGrowthStage);
                cropState.growingDate = 0;
                UpdateCropSprite(tilePosition);
            }
        }
    }
}