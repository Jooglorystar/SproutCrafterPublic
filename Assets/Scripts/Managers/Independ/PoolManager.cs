using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Pool
{
    public PoolTag tag;
    public int poolSize;
    public GameObject prefab;
}


public class PoolManager : MonoBehaviour
{
    private Dictionary<int, Queue<GameObject>> _poolDictionary = new Dictionary<int, Queue<GameObject>>();
    [SerializeField] private Pool[] _pool = null;
    [SerializeField] private Transform _objectPoolTransform = null;


    private void Awake()
    {
        GameManager.Instance.PoolM = this;

        for (int i = 0; i < _pool.Length; i++)
        {
            CreatePool(_pool[i].tag, _pool[i].prefab, _pool[i].poolSize);
        }
    }

    /// <summary>
    /// 정의된 _pool[]의 오브젝트 생성
    /// </summary>
    /// <param name="p_PoolTag"></param>
    /// <param name="p_Prefab"></param>
    /// <param name="p_PoolSize"></param>
    private void CreatePool(PoolTag p_PoolTag, GameObject p_Prefab, int p_PoolSize)
    {
        int poolTag = (int)p_PoolTag;
        string prefabName = p_Prefab.name;

        GameObject parentGameObject = new GameObject(prefabName);

        parentGameObject.transform.SetParent(_objectPoolTransform);

        if (!_poolDictionary.ContainsKey(poolTag))
        {
            _poolDictionary.Add(poolTag, new Queue<GameObject>());

            for (int i = 0; i < p_PoolSize; i++)
            {
                GameObject obj = Instantiate(p_Prefab, parentGameObject.transform);
                obj.SetActive(false);
                _poolDictionary[poolTag].Enqueue(obj);
            }
        }
    }


    /// <summary>
    /// 오브젝트를 얻어오는 함수
    /// </summary>
    /// <param name="p_PoolTag"></param>
    /// <returns></returns>
    public GameObject GetObject(PoolTag p_PoolTag)
    {
        int poolTag = (int)p_PoolTag;
        
        if (!_poolDictionary.ContainsKey(poolTag))
        {
            return null;
        }
        

        GameObject obj = _poolDictionary[poolTag].Dequeue();
        obj.gameObject.SetActive(true);
        
        return obj;
    }

    
    /// <summary>
    /// 반환하는 함수
    /// </summary>
    /// <param name="p_PoolTag"></param>
    /// <param name="obj"></param>
    public void ReleaseObject(PoolTag p_PoolTag ,GameObject p_Obj)
    {
        int poolTag = (int)p_PoolTag;
        
        p_Obj.SetActive(false);
        _poolDictionary[poolTag].Enqueue(p_Obj);
    }
}