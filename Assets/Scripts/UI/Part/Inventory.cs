using UnityEngine;


public class Inventory : MonoBehaviour
{
    [Header("UISlots")]
    private UISlot[] _uiSlots = new UISlot[40];

    [Header("UILog")]
    private UILog _logManager;

    [Header("선택된 슬롯 강조")]
    private int _previousFocusedSlot;

    [Tooltip("TileInteraction에서 사용하는 프로퍼티")]
    public int CurrentFocusedSlot
    {
        get { return _previousFocusedSlot; }
    }

    private Slot[] _slots => Managers.Data.inventorySlots;


    public void Init()
    {
        _uiSlots = GetComponentsInChildren<UISlot>();
        _logManager = GetComponentInChildren<UILog>();
        EventManager.Subscribe(GameEventType.GameStart, RefreshSlotsAfterGameStart);
    }


    private void Start()
    {
        GameManager.Instance.CharacterM.inventory = this;
        StartScrollSlot();
        OnSelectedSlotFocus(0);
    }


    #region 개별 슬롯 조작

    /// <summary>
    /// 인벤토리에 아이템 추가시 해당 매서드 이용
    /// </summary>
    public void AcquireItem(ItemSO p_Item, int p_Count = 1)
    {
        if (p_Item.itemType == ItemType.Equipment || p_Item.itemType == ItemType.Tool)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].itemSo == null)
                {
                    AddItem(i, p_Item, p_Count);
                    Managers.Sound.PlaySfx(SfxEnums.ItemPickup);
                    return;
                }
            }

            ThrowItem(p_Item, p_Count);
            return;
        }

        AddCount(p_Item, p_Count);

        ThrowItem(p_Item, p_Count);
    }


    /// <summary>
    /// i번째 인벤토리의 itemQuantity가 maxStack을 넘어 더이상 들어가지 못할때 다음 슬롯을 찾아서 넣어줌
    /// </summary>
    private void AddCount(ItemSO p_Item, int p_Count)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].itemSo == null) continue;

            if (_slots[i].itemSo.itemCode == p_Item.itemCode && _slots[i].itemSo.isCanStack)
            {
                int leftItemCount = p_Item.maxStack - _slots[i].itemCount;

                if (leftItemCount > 0)
                {
                    int addCount = Mathf.Min(leftItemCount, p_Count);
                    SetSlotCount(i, addCount);
                    p_Count -= addCount;

                    if (p_Count <= 0) return;
                }
            }
        }

        if (p_Count > 0)
        {
            AddEmptySlot(p_Item, p_Count);
        }
    }


    /// <summary>
    /// 스택이 불가능한 아이템이 남았을때 다시 빈자리를 찾아 추가를 시도함
    /// </summary>
    private void AddEmptySlot(ItemSO p_Item, int p_Count)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].itemSo == null)
            {
                if (p_Count > p_Item.maxStack)
                {
                    AddItem(i, p_Item, p_Item.maxStack);
                    p_Count -= p_Item.maxStack;
                }
                else
                {
                    AddItem(i, p_Item, p_Count);
                    return;
                }
            }
        }

        ThrowItem(p_Item, p_Count);
    }


    /// <summary>
    /// 실제로 아이템을 인벤토리에 추가하는 기능
    /// </summary>
    private void AddItem(int p_Index, ItemSO p_Item, int p_Quntity = 1)
    {
        Managers.Data.inventorySlots[p_Index].itemSo = p_Item;
        Managers.Data.inventorySlots[p_Index].itemCount = p_Quntity;

        GameManager.Instance.QuestM.CheckItem(Managers.Data.inventorySlots[p_Index].itemSo.itemCode);

        _uiSlots[p_Index].RefreshUIAddedItem(p_Item.itemSprite, p_Quntity);
        _uiSlots[p_Index].SetColor(1);

        _logManager.AddLog($"You have obtained {p_Item.itemName}.");
    }


    /// <summary>
    /// 슬롯의 아이템 개수를 조절하는 기능
    /// </summary>
    public void SetSlotCount(int p_Index, int p_Quntity = 1)
    {
        if (p_Index < 0) return;

        _slots[p_Index].itemCount += p_Quntity;

        if (Managers.Data.inventorySlots[p_Index].itemCount <= 0)
        {
            ClearSlot(p_Index);
            return;
        }

        GameManager.Instance.QuestM.CheckItem(Managers.Data.inventorySlots[p_Index].itemSo.itemCode);

        _uiSlots[p_Index].RefreshUIAddedItem(_slots[p_Index].itemSo.itemSprite, _slots[p_Index].itemCount);

        _logManager.AddLog($"You have obtained {_slots[p_Index].itemSo.itemName}.");
    }


    /// <summary>
    /// itemCount가 0이하일경우 데이터와 UI를 클리어 해줌
    /// </summary>
    private void ClearSlot(int p_Index)
    {
        Managers.Data.inventorySlots[p_Index].itemSo = null;
        Managers.Data.inventorySlots[p_Index].itemCount = 0;

        _uiSlots[p_Index].SetColor(1);
        _uiSlots[p_Index].ClearUI();
    }


    /// <summary>
    /// 인벤토리가 꽉 찼을 때 아이템을 버림
    /// </summary>
    public void ThrowItem(ItemSO p_Item, int p_Count = 1)
    {
        // Instantiate(p_Item, GameManager.Instance.CharacterM.transform.position, Quaternion.identity);
    }

    #endregion


    #region 외부에서 사용

    private void RefreshSlotsAfterGameStart(object p_Args)
    {
        foreach (UISlot item in _uiSlots)
        {
            if (item.gameObject.activeInHierarchy)
            {
                item.RefreshSlotUI();
            }
        }
    }

    public void StartScrollSlot()
    {
        GameManager.Instance.CharacterM.InputController.OnSelectSlotAction += OnSelectedSlotFocus;
    }

    public void StopScrollSlot()
    {
        GameManager.Instance.CharacterM.InputController.OnSelectSlotAction -= OnSelectedSlotFocus;
    }

    /// <summary>
    /// 인벤토리에 해당 아이템을 가지고 있는지 확인하는 메서드, 현재 p_index는 return이 false시 -1을 반환함
    /// </summary>
    /// <param name="p_itemCode">아이템 코드</param>
    /// <param name="p_index">가지고 있는 인벤토리 슬롯</param>
    /// <param name="p_itemAmount">가지고 있는 갯수</param>
    /// <returns>가지고 있는지 bool</returns>
    public bool IsHaveItem(int p_itemCode, out int p_index, out int p_itemAmount)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].itemSo != null && _slots[i].itemSo.itemCode == p_itemCode)
            {
                p_index = i;
                p_itemAmount = _slots[i].itemCount;
                return true;
            }
        }
        p_index = -1;
        p_itemAmount = 0;
        return false;
    }

    /// <summary>
    /// 현재 선택된 슬롯위에 포커스 이미지를 띄워주는 기능
    /// </summary>
    private void OnSelectedSlotFocus(int p_slotIndex)
    {
        _uiSlots[_previousFocusedSlot].TurnOffFocusImage();

        int selectedSlotIndex = p_slotIndex;

        _uiSlots[selectedSlotIndex].TurnOnFocusImage();

        _previousFocusedSlot = selectedSlotIndex;

        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }


    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEventType.GameStart, RefreshSlotsAfterGameStart);
    }

    #endregion
}