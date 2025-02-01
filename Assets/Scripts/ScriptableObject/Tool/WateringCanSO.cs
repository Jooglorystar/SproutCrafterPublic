using UnityEngine;


[CreateAssetMenu(fileName = "WateringCanSo", menuName = "Item/WateringCanSo")]
public class WateringCanSO : ItemSO
{
    public override bool CanChangeCursor(Vector2 p_MousePos)
    {
        return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanCursorWatering(p_MousePos);
    }
    
    
    public override bool CanUse(Vector2 p_MousePos)
    {
        return !GameManager.Instance.CharacterM.AnimationController.IsAnimating() && GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanUseWatering(p_MousePos);
    }

    
    public override void Use(Vector2 p_MousePos, int p_SlotNumber)
    {
        GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.UseWateringTool(p_MousePos);
    }
}