using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/BuildingDatabase", fileName = "BuildingDatabase_")]
public class BuildingDatabase : BaseDatabase<BuildingSO>
{
    /// <summary>
    /// 특정 타입의 아이템만 필터링하여 가져옵니다.
    /// </summary>
    public List<T> GetEntriesByType<T>() where T : BuildingSO
    {
        List<T> filteredList = new List<T>();
        foreach (var entry in GetAllEntries())
        {
            if (entry is T castedEntry)
            {
                filteredList.Add(castedEntry);
            }
        }
        return filteredList;
    }

    /// <summary>
    /// ItemCode를 기준으로 정렬
    /// </summary>
    public void SortEntriesByItemCode()
    {
        GetAllEntries().Sort((a, b) => a.itemCode.CompareTo(b.itemCode));
    }
}