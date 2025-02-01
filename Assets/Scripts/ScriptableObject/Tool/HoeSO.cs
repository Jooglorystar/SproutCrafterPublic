using UnityEngine;


[CreateAssetMenu(fileName = "HoeSo", menuName = "Item/HoeSo")]
public class HoeSO : ItemSO
{
    public override bool CanChangeCursor(Vector2 p_MousePos)
    {
        return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanCursorHoe(p_MousePos);
    }
    
    
    public override bool CanUse(Vector2 p_MousePos)
    {
        if (GameManager.Instance.CharacterM.AnimationController.IsAnimating() == false)
        {
            return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanUseHoe(p_MousePos);
        }
        
        return false;
    }
    

    public override void Use(Vector2 p_MousePos, int p_SlotNumber)
    {
        GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.UseHoeTool(p_MousePos);
    }
}