using UnityEngine;

[CreateAssetMenu(menuName = "Item/CropsSO", fileName = "CropsSO_")]
public class CropsSO : ItemSO
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