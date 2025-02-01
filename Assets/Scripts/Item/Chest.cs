using UnityEngine;


[System.Serializable]
public class SaveChestData
{
    public Vector3Int chestID;
    public int[] itemCodes = new int[20];
    public int[] itemQuantities = new int[20];

    public SaveChestData(Vector3Int p_vector3Int, Slot[] p_chestSlots)
    {
        chestID = p_vector3Int;
        for (int i = 0; i < p_chestSlots.Length; i++)
        {
            if (p_chestSlots[i].itemSo == null) continue;

            itemCodes[i] = p_chestSlots[i].itemSo.itemCode;
            itemQuantities[i] = p_chestSlots[i].itemCount;
        }
    }
    

    /// <summary>
    /// itemCodes와 itemQuantities를 통해 Slot[]을 반환하는 메서드
    /// </summary>
    /// <param name="p_itemCodes"></param>
    /// <param name="p_itemQuantities"></param>
    /// <returns></returns>
    public Slot[] SetSlot(int[] p_itemCodes, int[] p_itemQuantities)
    {
        Slot[] slots = new Slot[20];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new Slot(SlotType.Chest);
            if (p_itemCodes[i] == 0) continue;

            slots[i].itemSo = ItemCodeMapper.GetItemSo(p_itemCodes[i]);
            slots[i].itemCount = p_itemQuantities[i];
        }

        return slots;
    }
}


public class Chest : MonoBehaviour, IInteractable
{
    private Slot[] _slots = new Slot[20];

    private Vector3Int chestPosition => Vector3Int.RoundToInt(transform.position);


    private void OnEnable()
    {
        if (!GameManager.Instance.BuildingM.IsHasChestData(chestPosition, out Slot[] p_slots))
        {
            GameManager.Instance.BuildingM.AddChestDictionary(chestPosition, _slots);
        }
        else
        {
            _slots = p_slots;
        }
    }

    
    /// <summary>
    /// 상자별 고유 데이터를 초기화
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = _slots[i] == null ? new Slot(SlotType.Chest) : _slots[i];
        }
    }
    

    public void Interact()
    {
        OpenChest();
    }


    /// <summary>
    /// 상자를 열때 사용, 외부에서 호출
    /// </summary>
    private void OpenChest()
    {
        Managers.Data.chestSlots = _slots;
        Managers.UI.OnEnableUI<ChestPopup>();
    }


    /// <summary>
    /// 상자 오브젝트를 파괴할때 사용, 상자 안의 모든 아이템을 떨굼
    /// </summary>
    public void DestroyChest()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] != null)
            {
                GameManager.Instance.CharacterM.inventory.ThrowItem(_slots[i].itemSo, _slots[i].itemCount);
            }
        }

        GameManager.Instance.BuildingM.RemoveChestDictionary(chestPosition);
    }
}