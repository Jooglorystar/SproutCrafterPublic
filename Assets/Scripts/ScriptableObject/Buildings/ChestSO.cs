using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Building/DefaultChest", fileName = "Chest_")]
public class ChestSO : BuildingSO
{
    public override void Use(Vector2 p_MousePos, int p_SlotNumber)
    {
        GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.UseBuilding(this, p_SlotNumber);
    }


    public override void Place(Building p_building)
    {
        if(!p_building.gameObject.TryGetComponent<Chest>(out Chest chest))
        {
            p_building.gameObject.AddComponent<Chest>();
        }
    }

    public override void Distroy(Building p_building)
    {
        base.Distroy(p_building);

        if(p_building.gameObject.TryGetComponent<Chest>(out Chest chest))
        {
            chest.DestroyChest();
        }
    }
}