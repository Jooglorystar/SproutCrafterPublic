using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UISlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI 요소")]
    [SerializeField] private Image _itemSprite;
    [SerializeField] private TextMeshProUGUI _itemCountText;
    [SerializeField] private Image focusedSprite;

    [Header("고유 인덱스")]
    [HideInInspector] public bool isSuccessDrop = false;
    [SerializeField] private SlotType _slotType;
    [SerializeField] private int _slotIndex; // Managers.Data 의 slotType별 slots[]와 1:1 대응

    
    private void OnEnable()
    {
        RefreshSlotUI();
    }
    

#region UI Refresh

    /// <summary>
    /// 슬롯이 OnEnable될때 및 아이템 슬롯 상호 교체시, 해당 슬롯에 아이템이 있다면 보여줌 - 나눠야 할 필요 있음
    /// </summary>
    public void RefreshSlotUI()
    {
        switch (_slotType)
        {
            case SlotType.Inventory:
                if (Managers.Data.inventorySlots[_slotIndex].itemSo == null)
                {
                    ClearUI();
                    return;
                }

                _itemSprite.gameObject.SetActive(true);
                _itemSprite.sprite = Managers.Data.inventorySlots[_slotIndex].itemSo.itemSprite;
                _itemCountText.SetText(Managers.Data.inventorySlots[_slotIndex].itemCount > 1 ? Managers.Data.inventorySlots[_slotIndex].itemCount.ToString() : string.Empty);
                break;

            case SlotType.Equipment:
                if (Managers.Data.equipmentSlots[_slotIndex].itemSo == null)
                {
                    ClearUI();
                    return;
                }

                _itemSprite.gameObject.SetActive(true);
                _itemSprite.sprite = Managers.Data.equipmentSlots[_slotIndex].itemSo.itemSprite;
                _itemCountText.SetText(Managers.Data.equipmentSlots[_slotIndex].itemCount > 1 ? Managers.Data.equipmentSlots[_slotIndex].itemCount.ToString() : string.Empty);
                break;

            case SlotType.Combination:
                if (Managers.Data.combinationSlots[_slotIndex].itemSo == null)
                {
                    ClearUI();
                    return;
                }

                _itemSprite.gameObject.SetActive(true);
                _itemSprite.sprite = Managers.Data.combinationSlots[_slotIndex].itemSo.itemSprite;
                _itemCountText.SetText(Managers.Data.combinationSlots[_slotIndex].itemCount > 1 ? Managers.Data.combinationSlots[_slotIndex].itemCount.ToString() : string.Empty);
                break;
            
            case SlotType.Chest:
                if (Managers.Data.chestSlots[_slotIndex].itemSo == null)
                {
                    ClearUI();
                    return;
                }
                
                _itemSprite.gameObject.SetActive(true);
                _itemSprite.sprite = Managers.Data.chestSlots[_slotIndex].itemSo.itemSprite;
                _itemCountText.SetText(Managers.Data.chestSlots[_slotIndex].itemCount > 1 ? Managers.Data.chestSlots[_slotIndex].itemCount.ToString() : string.Empty);
                break;
        }
    }


    /// <summary>
    /// 슬롯에 아이템이 처음 추가될때 실행
    /// </summary>
    public void RefreshUIAddedItem(Sprite p_itemSprite, int p_itemCount = 1)
    {
        _itemSprite.gameObject.SetActive(true);
        _itemSprite.sprite = p_itemSprite;

        _itemCountText.SetText(p_itemCount > 1 ? p_itemCount.ToString() : string.Empty);
    }


    /// <summary>
    /// 슬롯의 아이템 개수의 변화가 있을때 실행
    /// </summary>
    public void RefreshUIItemCountChanged(int p_value = 1)
    {
        _itemCountText.text = p_value.ToString();
    }


    /// <summary>
    /// 슬롯의 아이템이 0이될때 UI를 초기화 시켜줌
    /// </summary>
    public void ClearUI()
    {
        _itemSprite.sprite = null;
        _itemCountText.text = string.Empty;
        _itemSprite.gameObject.SetActive(false);
    }
#endregion

    
#region 외부에서 조작
    /// <summary>
    /// 해당 슬롯이 선택되었을때 포커스 이미지를 띄워줌
    /// </summary>
    public void TurnOnFocusImage()
    {
        if (focusedSprite != null)
        {
            focusedSprite.gameObject.SetActive(true);
        }
    }
    

    /// <summary>
    /// 해당 슬롯이 선택해제되었을때 포커스 이미지를 꺼줌
    /// </summary>
    public void TurnOffFocusImage()
    {
        if (focusedSprite != null)
        {
            focusedSprite.gameObject.SetActive(false);
        }
    }

    
    /// <summary>
    /// 아이템이 존재 여부에 따라 투명도 조절
    /// </summary>
    /// <param name="alpha">아이템이 있는경우 1, 없을경우 0</param>
    public void SetColor(float alpha)
    {
        if (_itemSprite == null) return;

        Color color = _itemSprite.color;
        color.a = alpha;
        _itemSprite.color = color;
    }
# endregion
    
    
#region 아이템 드래그 드롭 기능
    /// <summary>
    /// 마우스 드래그가 시작될때 DragSlot에 이동시키고자하는 아이템의 정보를 담음
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(_slotType == SlotType.WasteBasket) return;
        
        (ItemSO, int) originSlotData = ReturnSlotData(_slotIndex, _slotType);

        if(originSlotData.Item1 == null) return;
        
        DragSlot.Instance.draggedSlot.itemSo = originSlotData.Item1;
        DragSlot.Instance.draggedSlot.itemCount = originSlotData.Item2;
        DragSlot.Instance.draggedSlotType = _slotType;
        DragSlot.Instance.draggedSlotIndex = _slotIndex;
        DragSlot.Instance.uiSlot = this;

        DragSlot.Instance.DragSlotSetImage(originSlotData.Item1.itemSprite);
        DragSlot.Instance.transform.position = eventData.position;
        
        Managers.Sound.PlaySfx(SfxEnums.ItemSelectInventory);
    }

    
    /// <summary>
    /// 피행위자,즉 드래그가 끝나는 지점의 아이템을 드래그 시작 슬롯으로 옮기기위해 아이템 정보를 임시로 저장해줌
    /// </summary>
    private (ItemSO, int) ReturnSlotData(int p_Index, SlotType p_slotType)
    {
        switch (p_slotType)
        {
            case SlotType.Inventory:
                return (Managers.Data.inventorySlots[p_Index].itemSo, Managers.Data.inventorySlots[p_Index].itemCount);
            
            case SlotType.Equipment:
                return (Managers.Data.equipmentSlots[p_Index].itemSo, Managers.Data.equipmentSlots[p_Index].itemCount);
            
            case SlotType.Combination:
                return (Managers.Data.combinationSlots[p_Index].itemSo, Managers.Data.combinationSlots[p_Index].itemCount);
            
            case SlotType.Chest:
                return (Managers.Data.chestSlots[p_Index].itemSo, Managers.Data.chestSlots[p_Index].itemCount);
        }
        
        return (null, 0);
    }


    /// <summary>
    /// 드래그중일때 DragSlot이 마우스를 따라가게하여 시각적인 정보를 제공
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if(_slotType == SlotType.WasteBasket) return;
        
        if (DragSlot.Instance.draggedSlot != null)
        {
            DragSlot.Instance.transform.position = eventData.position;
        }
    }
    
    
    /// <summary>
    /// 드래그가 UISlot에서 끝났을때 SlotType을 체크하여 아이템 교체를 진행함
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.Instance.draggedSlot.itemSo != null)
        {
            Managers.Sound.PlaySfx(SfxEnums.ItemDropInventory);
            DragSlot.Instance.uiSlot.isSuccessDrop = true;
            ChangeSlot();
        }
    }

    
    /// <summary>
    /// 드래그가 UISlot이 아닌 곳에서 끝났을때 아무 작업도 수행하지 않기위해 DragSlot을 초기화시켜줌
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if(_slotType == SlotType.WasteBasket) return;

        if (isSuccessDrop == false)
        {
            Managers.Sound.PlaySfx(SfxEnums.ButtonClickNegative);
        }
        
        isSuccessDrop = false;
        DragSlot.Instance.ClearDragSlot();
    }
    
    
    /// <summary>
    /// 아이템 교체 로직
    /// </summary>
    private void ChangeSlot()
    {
        (ItemSO, int) originSlot = ReturnSlotData(_slotIndex, _slotType);
        SlotType originSlotType = _slotType;
        int originSlotIndex = _slotIndex; // 피행위자의 아이템과 슬롯 정보
        

        switch (originSlotType)
        {
            case SlotType.WasteBasket: // 아이템 삭제
                ClearSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex);
                RefreshAfterChanged();
                return;

            case SlotType.Sell: // 아이템 판매
                
                break;

            case SlotType.Inventory: // 드래그가 끝난곳이 Inventory 배열이라면
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Equipment) // 타겟 슬롯의 아이템이 없고 드래그 된 아이템이 EquipmentSlots 배열의 아이템일때 
                {
                    SetSlotData(SlotType.Inventory, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Equipment, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Combination) // 타겟 슬롯의 아이템이 없고 드래그 된 아이템이 CombinationSlots 배열의 아이템일때 
                {
                    SetSlotData(SlotType.Inventory, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Combination, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Chest) // 드래그된 아이템이 어떤 것이든 타겟 슬롯의 아이템이 비어있을때
                {
                    SetSlotData(SlotType.Inventory, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Chest, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Inventory) // 드래그된 아이템이 인벤토리에서 왔고 타겟 슬롯이 비어있을때
                {
                    SetSlotData(SlotType.Inventory, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 != null && DragSlot.Instance.draggedSlotType == SlotType.Inventory) // 드래그된 아이템이 어떤 것이든 타겟 슬롯의 아이템과 교환 (SlotType.InventorySlot 끼리 교환하는 경우) 
                {
                    SetSlotData(SlotType.Inventory, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    SetSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex, originSlot.Item1, originSlot.Item2);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1.itemType == ItemType.Seed && DragSlot.Instance.draggedSlotType == SlotType.Combination) // 타겟 슬롯의 아이템이 Combination이고 드래그 된 아이템도 Combinations 배열의 아이템일때 
                {
                    SetSlotData(SlotType.Inventory, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    SetSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex, originSlot.Item1, originSlot.Item2);
                    
                    RefreshAfterChanged();
                }
                
                if (originSlot.Item1.itemType == ItemType.Equipment && DragSlot.Instance.draggedSlotType == SlotType.Equipment) // 타겟 슬롯의 아이템이 Equipment이고 드래그 된 아이템이 EquipmentSlots 배열의 아이템일때 
                {
                    SetSlotData(SlotType.Inventory, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Equipment, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                }
                
                break;

            case SlotType.Equipment: // 드래그가 끝난곳이 Equipment 배열이라면
                
                if(DragSlot.Instance.draggedSlot.itemSo.itemType != ItemType.Equipment) return; // Equipment 아이템만 들어갈 수 있도록
                if(DragSlot.Instance.draggedSlotType == _slotType) return; // Equipment의 슬롯끼리 교체할 수 없도록
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlot.itemSo.itemType == ItemType.Equipment) // 타겟 EquipmentSlots 가 비어있고 인벤토리에서 드래그된 아이템이 Equipment 일때 
                {
                    SetSlotData(SlotType.Equipment, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged(); // 장비 능력치 로직 추가 해야할 수도 있음
                }
                
                break;
            
            case SlotType.Combination: // 드래그가 끝난곳이 Combination 배열이라면
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Combination) // 타겟 Combinations 가 비어있고 드래그된 아이템이 Seed일때 (Seed 배열 끼리 교환, 한쪽은 비어있음)
                {
                    SetSlotData(SlotType.Combination, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Combination, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Inventory && DragSlot.Instance.draggedSlot.itemSo.itemType == ItemType.Seed) // 기존 CombinationSlots 가 비어있고 인벤토리에서 드래그된 아이템이 Seed일때 (checked)
                {
                    SetSlotData(SlotType.Combination, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 != null && DragSlot.Instance.draggedSlotType == SlotType.Combination) // 타겟 Combination 아이템이 Seed 인데, 드래그된 아이템도 Seed일때 (Seed 배열끼리 교환, 맞교환) ???
                {
                    SetSlotData(SlotType.Combination, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    SetSlotData(SlotType.Combination, DragSlot.Instance.draggedSlotIndex, originSlot.Item1, originSlot.Item2);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (DragSlot.Instance.draggedSlotType == SlotType.Inventory && DragSlot.Instance.draggedSlot.itemSo.itemType == ItemType.Seed)  // 기존 Combinations 의 Seed가 존재하지만 인벤토리의 Seed와 맞교환할때
                {
                    SetSlotData(SlotType.Combination, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    SetSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex, originSlot.Item1, originSlot.Item2);
                    
                    RefreshAfterChanged();
                }
                
                break;
            
            case SlotType.Chest: // 드래그가 끝난곳이 Chest 배열이라면
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Chest) // 체스트 끼리 교환했지만 타겟이 비어있을때
                {
                    SetSlotData(SlotType.Chest, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Chest, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 != null && DragSlot.Instance.draggedSlotType == SlotType.Chest) // 체스트 끼리 교환했지만 타겟이 비어있지 않을때 (맞교환)
                {
                    SetSlotData(SlotType.Chest, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    SetSlotData(SlotType.Chest, DragSlot.Instance.draggedSlotIndex, originSlot.Item1, originSlot.Item2);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 == null && DragSlot.Instance.draggedSlotType == SlotType.Inventory) // 체스트에 인벤토리의 아이템을 넣을 경우
                {
                    SetSlotData(SlotType.Chest, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    ClearSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                if (originSlot.Item1 != null && DragSlot.Instance.draggedSlotType == SlotType.Inventory) // 체스트와 인벤토리의 아이템을 맞교환하는 경우
                {
                    SetSlotData(SlotType.Chest, originSlotIndex, DragSlot.Instance.draggedSlot.itemSo, DragSlot.Instance.draggedSlot.itemCount);
                    SetSlotData(SlotType.Inventory, DragSlot.Instance.draggedSlotIndex, originSlot.Item1, originSlot.Item2);
                    
                    RefreshAfterChanged();
                    return;
                }
                
                break;
        }
    }
    
    
    /// <summary>
    /// 교환된 아이템을 추가해주는 기능
    /// </summary>
    private void SetSlotData(SlotType p_SlotType, int p_Index, ItemSO p_ItemSO, int p_ItemCount)
    {
        switch (p_SlotType)
        {
            case SlotType.Inventory:
                Managers.Data.inventorySlots[p_Index].itemSo = p_ItemSO;
                Managers.Data.inventorySlots[p_Index].itemCount = p_ItemCount;
                break;

            case SlotType.Equipment:
                Managers.Data.equipmentSlots[p_Index].itemSo = p_ItemSO;
                Managers.Data.equipmentSlots[p_Index].itemCount = p_ItemCount;
                break;

            case SlotType.Combination:
                Managers.Data.combinationSlots[p_Index].itemSo = p_ItemSO;
                Managers.Data.combinationSlots[p_Index].itemCount = p_ItemCount;
                break;
            case SlotType.Chest:
                Managers.Data.chestSlots[p_Index].itemSo = p_ItemSO;
                Managers.Data.chestSlots[p_Index].itemCount = p_ItemCount;
                break;
        }
    }
    
    
    /// <summary>
    /// 타겟 슬롯이 Null일때 아이템을 넣고 기존 슬롯을 초기화하는 기능
    /// </summary>
    private void ClearSlotData(SlotType p_SlotType, int p_Index)
    {
        switch (p_SlotType)
        {
            case SlotType.Inventory:
                Managers.Data.inventorySlots[p_Index].itemSo = null;
                Managers.Data.inventorySlots[p_Index].itemCount = 0;
                break;

            case SlotType.Equipment:
                Managers.Data.equipmentSlots[p_Index].itemSo = null;
                Managers.Data.equipmentSlots[p_Index].itemCount = 0;
                break;

            case SlotType.Combination:
                Managers.Data.combinationSlots[p_Index].itemSo = null;
                Managers.Data.combinationSlots[p_Index].itemCount = 0;
                break;
            case SlotType.Chest:
                Managers.Data.chestSlots[p_Index].itemSo = null;
                Managers.Data.chestSlots[p_Index].itemCount = 0;
                break;
        }
    }
    

    /// <summary>
    /// 아이템 교환이 완료된 후 교환이 일어난 두 슬롯의 UI 업데이트
    /// </summary>
    private void RefreshAfterChanged()
    {
        RefreshSlotUI();
        DragSlot.Instance.uiSlot.RefreshSlotUI();
        DragSlot.Instance.ClearDragSlot();
    }
#endregion
    
}