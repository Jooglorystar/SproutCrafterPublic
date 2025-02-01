using UnityEngine;


[CreateAssetMenu(menuName = "Item/SeedSO", fileName = "SeedSO_")]
public class SeedSO : ItemSO
{
    [Header("씨앗 고유 속성")]
    public int linkedCropCode;            // 심어서 자라나는 작물 (CropsSO)
    public int maxGrowthStage;            // 최대 성장 단계
    public int growTime;                  // 각 성장 단계에 필요한 시간 (일 단위)
    public ESeason plantingSeasons;        // 심을 수 있는 계절 (Enum)
    public TileType tileType;             // 심기 조건 (Enum)
    public int minYield;                  // 최소 수확량
    public int maxYield;                  // 최대 수확량


    public override bool CanChangeCursor(Vector2 p_MousePos)
    {
        return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanCursorSeed(p_MousePos);
    }

    public override bool CanUse(Vector2 p_MousePos)
    {
        return GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.CheckCanPlant(p_MousePos);
    }

    public override void Use(Vector2 p_MousePos, int p_SlotNumber)
    {
        GameManager.Instance.CharacterM.PlayerInteraction.ItemInteraction.PlantSeed(p_MousePos, p_SlotNumber);
    }
}