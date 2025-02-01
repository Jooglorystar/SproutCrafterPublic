using UnityEngine;


[CreateAssetMenu(menuName = "Item/FishSO", fileName = "FishSO_")]
public class FishSO : ItemSO
{
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