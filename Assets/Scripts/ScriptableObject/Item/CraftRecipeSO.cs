using UnityEngine;


[System.Serializable]
public class NeedItems
{
    public ItemSO materialItem;
    public int materialAmount;
}


[CreateAssetMenu(menuName = "Item/CraftRecipeSO", fileName = "CraftRecipeSO_")]
public class CraftRecipeSO : ScriptableObject
{
    public ItemSO resultItem;
    public NeedItems[] needItems;
}