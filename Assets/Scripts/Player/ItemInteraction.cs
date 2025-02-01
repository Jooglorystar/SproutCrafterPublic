using UnityEngine;
using UnityEngine.Tilemaps;


public class ItemInteraction : MonoBehaviour
{
    [Header("타일 상호작용")]
    private Tilemap fishingTilemap;
    
    private Slot[] _slots => Managers.Data.inventorySlots;

    
#region 도구

    #region 괭이

    /// <summary>
    /// 괭이를 사용 가능하다면 커서 모양 변경
    /// </summary>
    public bool CheckCanCursorHoe(Vector2 p_MousePos)
    {
        if(GameManager.Instance.TilemapM.HasTilemap == false) return false;
        
        Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);
        
        return GameManager.Instance.TilemapM.CheckNotTilledTile(cellPosition);
    }


    /// <summary>
    /// 괭이를 사용 가능한지 여부를 반환
    /// </summary>
    public bool CheckCanUseHoe(Vector2 p_MousePos)
    {
        if (GameManager.Instance.TilemapM.IsNullTilemap()) return false;
        return CheckToolRange(p_MousePos);
    }
    
    
    /// <summary>
    /// 괭이를 사용하여 타일을 경작 가능한 상태로 변경
    /// </summary>
    public void UseHoeTool(Vector2 p_MousePos)
    {
        GameManager.Instance.CharacterM.PlayerMovement.UnSubscribeMoveEvent();
        GameManager.Instance.CharacterM.AnimationController.SetHoe();
        GameManager.Instance.CharacterM.playerCondition.UseStamina(Setting.StaminaCost);

        Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);
        GameManager.Instance.TilemapM.TillTile(cellPosition);
    }

    #endregion


    #region 물뿌리개

    public bool CheckCanCursorWatering(Vector2 p_MousePos)
    {
        if(GameManager.Instance.TilemapM.HasTilemap == false) return false;
        
        Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);

        return GameManager.Instance.TilemapM.CheckNotWateredTile(cellPosition);
    }


    public bool CheckCanUseWatering(Vector2 p_MousePos)
    {
        if (GameManager.Instance.TilemapM.IsNullTilemap()) return false;
        
        return CheckToolRange(p_MousePos);
    }
    

    /// <summary>
    /// 물뿌리개 사용
    /// </summary>
    public void UseWateringTool(Vector2 p_MousePos)
    {
        GameManager.Instance.CharacterM.PlayerMovement.UnSubscribeMoveEvent();
        GameManager.Instance.CharacterM.AnimationController.SetWatering();
        GameManager.Instance.CharacterM.playerCondition.UseStamina(Setting.StaminaCost);

        Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);

        Managers.Sound.PlaySfx(SfxEnums.Watering);
        GameManager.Instance.TilemapM.WaterTile(cellPosition);
    }

    #endregion
    

    # region 도끼
    
    public bool CheckCanCursorAxe(Vector2 p_MousePos)
    {
        RaycastHit2D hit = Physics2D.Raycast(p_MousePos, Vector2.zero, Setting.RaycastDistance, Setting.TreeLayerMask);

        return hit && hit.collider.TryGetComponent(out ResourceInteraction resourceInteraction);
    }


    public bool CheckCanUseAxe(Vector2 p_MousePos)
    {
        return CheckToolRange(p_MousePos);
    }
    
    
    /// <summary>
    /// 도끼 사용 - 나무 자르기
    /// </summary>
    public void UseAxeTool(Vector2 p_MousePos)
    {
        RaycastHit2D hit = Physics2D.Raycast(p_MousePos, Vector2.zero, Setting.RaycastDistance, Setting.TreeLayerMask);

        GameManager.Instance.CharacterM.PlayerMovement.UnSubscribeMoveEvent();
        GameManager.Instance.CharacterM.AnimationController.SetAxe();
        GameManager.Instance.CharacterM.playerCondition.UseStamina(Setting.StaminaCost);

        if (hit&& hit.collider.TryGetComponent(out ResourceInteraction resourceInteraction))
        {
            resourceInteraction.Hit(1);
            Managers.Sound.PlaySfx(SfxEnums.HitWood);
        }

        if (GameManager.Instance.TilemapM.GroundTilemap == null) return;

        Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);
        
        if (GameManager.Instance.BuildingM.HasBuildingData(cellPosition))
        {
            GameManager.Instance.BuildingM.Remove(cellPosition);
        }
    }
    
    #endregion


    #region 낚시
    
    public bool CheckCanCursorFishingLoad(Vector2 p_MousePos)
    {
        if(GameManager.Instance.TilemapM.HasWaterTilemap == false) return false;
        
        Vector3Int cellPosition = GameManager.Instance.TilemapM.WaterCanFishing.WorldToCell(p_MousePos);
        
        return GameManager.Instance.TilemapM.CheckCanFishingTile(cellPosition);
    }


    public bool CheckCanUseFishingLoad(Vector2 p_MousePos)
    {
        return CheckToolRange(p_MousePos);
    }

    
    /// <summary>
    /// 낚시대 사용
    /// </summary>
    public void UseFishingRod(Vector2 p_MousePos)
    {
        GameManager.Instance.CharacterM.PlayerMovement.UnSubscribeMoveEvent();
        GameManager.Instance.CharacterM.playerCondition.UseStamina(Setting.StaminaCost);
        Managers.UI.OnEnableUI<FishingPopup>();
    }

    #endregion
    
#endregion
    

#region 아이템
    
    public bool CheckCanCursorSeed(Vector2 p_MousePos)
    {
        if (GameManager.Instance.TilemapM.HasTilemap == false) return false;

        if (_slots[GameManager.Instance.CharacterM.InputController.FocusedSlotNumber].itemSo is SeedSO seedSo)
        {
            Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);
            TileData tileData = GameManager.Instance.TilemapM.GetTileData(cellPosition);

            if (!tileData.IsPlanted && tileData.IsTilled)
            {
                return true;
            }
        }
        
        return false;
    }


    public bool CheckCanPlant(Vector2 p_MousePos)
    {
        return (!GameManager.Instance.TilemapM.IsNullTilemap() && CheckToolRange(p_MousePos));
    }
    
    
    /// <summary>
    /// 씨앗 사용
    /// </summary>
    public void PlantSeed(Vector2 p_MousePos, int p_SlotNumber)
    {
        if (_slots[p_SlotNumber].itemSo is SeedSO seedSo)
        {
            Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);
            TileData tileData = GameManager.Instance.TilemapM.GetTileData(cellPosition);

            if (!tileData.IsPlanted && tileData.IsTilled)
            {
                GameManager.Instance.CharacterM.inventory.SetSlotCount(p_SlotNumber, -1);
                GameManager.Instance.CropM.PlantCrop(cellPosition, seedSo);
                GameManager.Instance.CharacterM.playerCondition.UseStamina(Setting.StaminaCost);
            }
        }
    }
    

    /// <summary>
    /// 건물 설치
    /// </summary>
    public void UseBuilding(ItemSO p_itemSO, int p_SlotNumber)
    {
        GameManager.Instance.BuildingM.InitializeWithBuilding(p_SlotNumber, ((BuildingSO)p_itemSO));
    }

#endregion
    
    public bool CheckCanFishing(Vector2 p_MousePos)
    {
        if (GameManager.Instance.TilemapM.WaterCanFishing != null)
        {
            Vector3Int gridCellPosition = GameManager.Instance.TilemapM.WaterCanFishing.WorldToCell(p_MousePos);
        
            if (CanFishAtTile(gridCellPosition))
            {
                return true;
            }
        }
        
        return false;
    }
    
    
    private bool CanFishAtTile(Vector3Int tilePosition)
    {
        Vector3Int gridCellPosition = GameManager.Instance.TilemapM.WaterCanFishing.WorldToCell(transform.position);
        
        return GameManager.Instance.TilemapM.WaterCanFishing.HasTile(tilePosition) && GameManager.Instance.TilemapM.GroundCanFishing.HasTile(gridCellPosition);
    }
    
    
    /// <summary>
    /// 도구 사용 가능 거리 여부를 반환
    /// </summary>
    private bool CheckToolRange(Vector2 p_MousePos, float p_Range = Setting.InteractRange)
    {
        if (Vector2.Distance(transform.position, p_MousePos) > p_Range)
        {
            return false;
        }
        
        return true;
    }
}