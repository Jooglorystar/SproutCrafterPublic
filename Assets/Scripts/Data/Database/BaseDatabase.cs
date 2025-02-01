using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDatabase<T> : ScriptableObject where T : class, IDatabaseEntry
{
    [Header("데이터 엔트리")]
    [SerializeField] private List<T> entries = new List<T>(); // 데이터 리스트

    private Dictionary<int, T> lookupDictionary = new Dictionary<int, T>(); // 키를 int로 변경

    /// <summary>
    /// 데이터베이스 초기화. 내부적으로 Dictionary를 설정합니다.
    /// </summary>
    public void Init()
    {
        lookupDictionary.Clear();
        foreach (var entry in entries)
        {
            if (entry == null || entry.Key == 0)
            {
                continue;
            }

            if (lookupDictionary.ContainsKey(entry.Key))
            {
                continue;
            }

            lookupDictionary[entry.Key] = entry;
        }
    }

    /// ID(Key)를 기반으로 데이터를 검색합니다.
    public T GetByID(int id)
    {
        if (lookupDictionary.TryGetValue(id, out var entry))
        {
            return entry;
        }
        // Debug.LogWarning($"ID({id})에 해당하는 데이터가 없습니다."); 25.01.03에 빌드를 위한 주석 처리
        return null;
    }


    /// <summary>
    /// 데이터 리스트를 반환합니다.
    /// </summary>
    public List<T> GetAllEntries()
    {
        return entries;
    }

    /// <summary>
    /// 데이터베이스에 새 데이터를 추가합니다. (런타임 전용)
    /// </summary>
    public void AddEntry(T newEntry)
    {
        if (lookupDictionary.ContainsKey(newEntry.Key))
        {
            Debug.LogWarning($"중복 키({newEntry.Key})로 인해 데이터 추가가 실패했습니다.");
            return;
        }

        entries.Add(newEntry);
        lookupDictionary[newEntry.Key] = newEntry;
    }

    /// <summary>
    /// 데이터베이스에서 데이터를 삭제합니다. (런타임 전용)
    /// </summary>
    public void RemoveEntry(int id)
    {
        if (!lookupDictionary.ContainsKey(id))
        {
            Debug.LogWarning($"ID({id})에 해당하는 데이터가 없어 삭제할 수 없습니다.");
            return;
        }

        var entry = lookupDictionary[id];
        entries.Remove(entry);
        lookupDictionary.Remove(id);
    }
}

public interface IDatabaseEntry
{
    int Key { get; } // 데이터의 고유 ID
}