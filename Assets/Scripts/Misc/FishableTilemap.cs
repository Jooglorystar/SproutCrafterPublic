using UnityEngine;
using UnityEngine.Tilemaps;


public class FishableTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap _waterCanFishing;
    [SerializeField] private Tilemap _groundCanFishing;

    private void Awake()
    {
        GameManager.Instance.TilemapM.WaterCanFishing = _waterCanFishing;
        GameManager.Instance.TilemapM.GroundCanFishing = _groundCanFishing;
        
        GameManager.Instance.TilemapM.HasWaterTilemap = true;
    }


    private void OnDisable()
    {
        GameManager.Instance.TilemapM.HasWaterTilemap = false;
    }
}