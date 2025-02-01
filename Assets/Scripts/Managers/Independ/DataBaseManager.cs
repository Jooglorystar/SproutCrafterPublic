using System.Collections.Generic;
using UnityEngine;


public class DataBaseManager : MonoBehaviour
{
    [SerializeField] private ItemDatabase _itemDatabase;
    [SerializeField] private BuildingDatabase _buildingDatabase;
    [SerializeField] private CraftRecipeSO[] _craftRecipes = null;
    [SerializeField] private SkillInfoDataBaseSO _skillInfoDataBase;
    
    public Dictionary<int, bool> idCollectedDic = new Dictionary<int, bool>(); // DB로 옮겨야함
    public Dictionary<int, ItemSO> allItemDic = new Dictionary<int, ItemSO>(); // DB로 옮겨야함
    
    
    public ItemDatabase ItemDatabase => _itemDatabase;
    public BuildingDatabase BuildingDatabase => _buildingDatabase;
    public CraftRecipeSO[] craftRecipesDatabase => _craftRecipes;
    public SkillInfoDataBaseSO SkillInfoDataBase => _skillInfoDataBase;

    private void Awake()
    {
        GameManager.Instance.DataBaseM = this;
    }
}