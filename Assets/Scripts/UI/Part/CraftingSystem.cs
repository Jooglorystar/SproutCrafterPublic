using System.Collections.Generic;
using UnityEngine;


public class CraftingSystem : MonoBehaviour
{
    private Slot[] slots => Managers.Data.inventorySlots;


    public void CreateItem(CraftRecipeSO p_CraftRecipeSo)
    {
        ProcessSortingSlots(p_CraftRecipeSo);
    }

    
    /// <summary>
    /// 슬롯에 퍼져있는 아이템들을 정렬
    /// </summary>
    private void ProcessSortingSlots(CraftRecipeSO p_CraftRecipeSo)
    {
        Dictionary<int, Slot> sortedSlots = new Dictionary<int, Slot>();

        foreach (Slot slot in slots)
        {
            if (slot.itemCount == 0) break;

            if (sortedSlots.ContainsKey(slot.itemSo.itemCode))
            {
                sortedSlots[slot.itemSo.itemCode].itemCount += slot.itemCount;
                // 만약 wood라는 아이템이 20개, 30개씩 떨어져 있다면 합쳐서 50개의 숫자로 만들어줌
            }
            else
            {
                sortedSlots.Add(slot.itemSo.itemCode, slot);
                // 만약 wood라는 아이템이 없다면 itemCode를 이용하여 새로 추가
            }
        }

        if (sortedSlots.Count >= p_CraftRecipeSo.needItems.Length)
        {
            ProcessCheckEnoughItems(sortedSlots, p_CraftRecipeSo);
        }
    }


    /// <summary>
    /// 충분한 수량의 아이템이 있는지 확인
    /// </summary>
    private void ProcessCheckEnoughItems(Dictionary<int, Slot> p_SlotsDic, CraftRecipeSO p_CraftRecipeSO)
    {
        int count = 0;

        foreach (NeedItems item in p_CraftRecipeSO.needItems)
        {
            if (p_SlotsDic.TryGetValue(item.materialItem.itemCode, out Slot slot))
            {
                if (slot.itemCount >= item.materialAmount)
                {
                    count++;
                }
            }

            if (count >= p_CraftRecipeSO.needItems.Length)
            {
                ProcessMakeItem(p_CraftRecipeSO);
            }
        }
    }


    /// <summary>
    /// 실제로 아이템을 만들어주는 기능, 인벤토리에 추가
    /// </summary>
    /// <param name="p_CraftRecipeSO"></param>
    private void ProcessMakeItem(CraftRecipeSO p_CraftRecipeSO)
    {
        foreach (NeedItems item in p_CraftRecipeSO.needItems)
        {
            int needItemCount = item.materialAmount;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].itemCount == 0) continue;

                if (slots[i].itemSo.itemCode != item.materialItem.itemCode) continue;

                if (slots[i].itemCount - item.materialAmount < 0) // slot[i]에서 충분한 수량이 없다면 다음 slot[i]에서 이어서 빼줌
                {
                    needItemCount -= slots[i].itemCount;

                    GameManager.Instance.CharacterM.inventory.SetSlotCount(i, -slots[i].itemCount);
                }
                else
                {
                    GameManager.Instance.CharacterM.inventory.SetSlotCount(i, -needItemCount);
                }
            }
        }
        
        GameManager.Instance.CharacterM.inventory.AcquireItem(p_CraftRecipeSO.resultItem);
        EventManager.Dispatch(GameEventType.OnInventory, null);
    }
}