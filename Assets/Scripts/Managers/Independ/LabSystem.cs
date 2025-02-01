using System.Collections.Generic;
using UnityEngine;

public class LabSystem : MonoBehaviour
{
    [Header("돌연변이 코드 범위")]
    private const int MutantStartCode = 700;
    private const int MutantEndCode = 710;

    #region Combine Logic (조합 로직)
    /// CombinationSlots에 있는 두 작물을 조합하여 결과 아이템을 반환합니다.
    public ItemSO CombineCrops()
    {
        GameManager.Instance.DataBaseM.ItemDatabase.Init();
        Slot[] slots = Managers.Data.combinationSlots;

        // 슬롯 유효성 검증
        if (!ValidateCombinationSlots(slots))
        {
            return null;
        }

        ItemSO cropA = slots[0].itemSo;
        ItemSO cropB = slots[1].itemSo;

        // 돌연변이 확률 계산
        float mutationChance = 0.1f; // 10% 확률로 돌연변이 발생
        if (Random.value < mutationChance)
        {
            ItemSO mutantItem = GenerateMutation();
            if (mutantItem != null)
            {
                return mutantItem;
            }
        }

        // 두 아이템의 코드 합산
        int resultCode = cropA.itemCode + cropB.itemCode;

        // 데이터베이스에서 결과 아이템 검색
        ItemSO resultItem = GameManager.Instance.DataBaseM.ItemDatabase.GetByID(resultCode);

        if (resultItem == null)
        {
            return null;
        }
        return resultItem;
    }

    /// 슬롯 유효성 검증
    private bool ValidateCombinationSlots(Slot[] slots)
    {
        if (slots == null || slots.Length < 2)
        {
            return false; // 슬롯이 부족한 경우
        }

        // 각 슬롯에 아이템이 있는지 확인
        if (slots[0].itemSo == null || slots[1].itemSo == null)
        {
            return false;
        }

        // 각 아이템이 올바른 타입인지 확인
        if (slots[0].itemSo.itemType != ItemType.Seed || slots[1].itemSo.itemType != ItemType.Seed)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region Mutation Logic (돌연변이 로직)
    /// 돌연변이 작물을 생성하여 반환합니다.
    public ItemSO GenerateMutation()
    {
        var mutantItems = GameManager.Instance.DataBaseM.ItemDatabase.GetAllEntries()
            .FindAll(item => item.itemCode >= MutantStartCode && item.itemCode <= MutantEndCode);

        if (mutantItems.Count == 0)
        {
            return null;
        }

        ItemSO mutation = mutantItems[Random.Range(0, mutantItems.Count)];
        return mutation;
    }
    #endregion
}
