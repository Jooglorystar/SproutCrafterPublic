using UnityEngine;

[CreateAssetMenu(menuName = "Building/DefaultBuilding", fileName = "Building_")]
public class BuildingSO : ItemSO
{
    [Header("건물 정보")]
    public string spriteCategory;
    public Vector3Int buildingAreaSize;


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
    
    public virtual void Place(Building p_building)
    {
        if(p_building.gameObject.TryGetComponent<Chest>(out Chest chest))
        {
            Destroy(chest);
        }
    }

    public virtual void Distroy(Building p_building)
    {

    }
}