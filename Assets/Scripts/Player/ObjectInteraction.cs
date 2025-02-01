using UnityEngine;


public class ObjectInteraction : MonoBehaviour
{
    /// <summary>
    /// 상호작용 가능한 오브젝트 검출시 이벤트 시작
    /// </summary>
    public bool OnInteraction(Vector2 p_MousePos)
    {
        RaycastHit2D raycastHit2D = Physics2D.Raycast(p_MousePos, Vector2.zero, Setting.RaycastDistance, Setting.InteractableLayerMask);

        if (raycastHit2D.collider == null) return false;
        if (Vector2.Distance(gameObject.transform.position, raycastHit2D.collider.gameObject.transform.position) >= Setting.InteractRange + 1) return false;
        if (raycastHit2D.collider.gameObject.TryGetComponent(out IInteractable interactable) == false) return false;

        interactable.Interact();
        return true;
    }


    /// <summary>
    /// 상호작용할 오브젝트가 없으면 수확을 수행
    /// </summary>
    public void OnHarvest(Vector2 p_MousePos)
    {
        if (Vector3.Distance(transform.position, p_MousePos) > Setting.InteractRange) return;

        if (CheckCanCrop(p_MousePos))
        {
            HarvestCrop(p_MousePos);
        }
    }


    public bool CheckCanCrop(Vector2 p_MousePos)
    {
        if (GameManager.Instance.TilemapM.IsNullTilemap()) return false;

        Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);

        if (GameManager.Instance.CropM.cropStateDictionary.TryGetValue(cellPosition, out var cropState) &&
            cropState.growthStage >= cropState.seedSO.maxGrowthStage) return true;
        
        return false;
    }


    private void HarvestCrop(Vector2 p_MousePos)
    {
        Vector3Int cellPosition = GameManager.Instance.TilemapM.GroundTilemap.WorldToCell(p_MousePos);

        // 작물을 제거하고 제거된 작물 데이터를 받아옴
        ItemSO harvestedCrop = GameManager.Instance.CropM.HarvestCrop(cellPosition, out int cropAmount);

        // 인벤토리에 작물 데이터 추가
        if (harvestedCrop != null && cropAmount > 0)
        {
            GameManager.Instance.CharacterM.inventory.AcquireItem(harvestedCrop, cropAmount);
        }
    }
}