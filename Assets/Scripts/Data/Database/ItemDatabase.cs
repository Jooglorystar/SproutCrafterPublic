using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/ItemDatabase", fileName = "ItemDatabase_")]
public class ItemDatabase : BaseDatabase<ItemSO>
{
    /// <summary>
    /// 특정 타입의 아이템만 필터링하여 가져옵니다.
    /// </summary>
    public T GetEntryByCode<T>(int itemCode) where T : ItemSO
    {
        foreach (var entry in GetAllEntries())
        {
            if (entry.itemCode == itemCode && entry is T castedEntry)
            {
                return castedEntry;
            }
        }
        return null;
    }


    /// <summary>
    /// ItemCode를 기준으로 정렬
    /// </summary>
    public void SortEntriesByItemCode()
    {
        GetAllEntries().Sort((a, b) => a.itemCode.CompareTo(b.itemCode));
    }

    /// <summary>
    /// 기본 ResourceSO를 가져옵니다.
    /// </summary>
    public ResourceSO GetDefaultResourceSO()
    {
        foreach (var entry in GetAllEntries())
        {
            if (entry is ResourceSO resourceEntry)
            {
                // 기본 ResourceSO를 결정하는 기준
                if (resourceEntry.itemName == "OakTree")
                {
                    return resourceEntry;
                }
            }
        }

        // 기본값을 찾지 못한 경우 null 반환
        Debug.LogWarning("Default ResourceSO not found in database.");
        return null;
    }
}
