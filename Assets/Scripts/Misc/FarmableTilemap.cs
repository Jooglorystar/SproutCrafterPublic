using UnityEngine;
using UnityEngine.Tilemaps;


public class FarmableTilemap :MonoBehaviour
{
    [SerializeField] private Tilemap _groundTilemap; // 땅 타일맵
    [SerializeField] private Tilemap _waterTilemap; // 물 타일맵
    [SerializeField] private Tilemap _cropTilemap; // 작물 타일맵
    

    private void Awake()
    {
        GameManager.Instance.TilemapM.GroundTilemap = _groundTilemap;
        GameManager.Instance.TilemapM.WaterTilemap = _waterTilemap;
        GameManager.Instance.TilemapM.CropTilemap = _cropTilemap;
    }

    private void OnEnable()
    {
        GameManager.Instance.TilemapM.UpdateTile();
        
        GameManager.Instance.TilemapM.HasTilemap = true;
    }


    private void OnDisable()
    {
        GameManager.Instance.TilemapM.HasTilemap = false;
    }
}