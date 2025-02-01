using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ItemResourceSO", fileName = "ResourceSO_")]
public class ResourceSO : ItemSO
{
    [Header("자원 고유 속성")]
    public string requiredTool;
    public int maxGrowthStage;
    public int growTime;
    public int health;
    public int dropAmount;
    public ResourceSO linkedItem;
    
    
    public override bool CanChangeCursor(Vector2 p_MousePos)
    {
        return true;
    }
    
    
    public override bool CanUse(Vector2 p_MousePos)
    {
        return true;
    }

    public override void Use(Vector2 p_MousePos, int p_SlotNumber)
    {
        
    }
}