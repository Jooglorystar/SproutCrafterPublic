using UnityEngine;


[CreateAssetMenu(fileName = "FishingRodSo", menuName = "Item/FishingRodSo")]
public class FishingRodSO : ItemSO
{
    public override bool CanChangeCursor(Vector2 p_MousePos)
    {
        return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanCursorFishingLoad(p_MousePos);
    }
    
    public override bool CanUse(Vector2 p_MousePos)
    {
        if (!GameManager.Instance.CharacterM.AnimationController.IsAnimating() && 
            GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanUseFishingLoad(p_MousePos))
        {
            return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanFishing(p_MousePos);
        }
        
        return false;
    }

    public override void Use(Vector2 p_MousePos, int p_SlotNumber)
    {
        GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.UseFishingRod(p_MousePos);
    }
}