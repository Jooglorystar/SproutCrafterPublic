using UnityEngine;


[CreateAssetMenu(fileName = "AxeSo", menuName = "Item/AxsSo")]
public class AxeSO : ItemSO
{
    public override bool CanChangeCursor(Vector2 p_MousePos)
    {
        return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanCursorAxe(p_MousePos);
    }
    
    public override bool CanUse(Vector2 p_MousePos)
    {
        return !GameManager.Instance.CharacterM.AnimationController.IsAnimating() && GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanUseAxe(p_MousePos);
    }

    public override void Use(Vector2 p_MousePos, int p_SlotNumber)
    {
        GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.UseAxeTool(p_MousePos);
    }
}