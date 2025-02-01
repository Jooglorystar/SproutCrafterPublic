using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildableTilemap :MonoBehaviour
{
    [SerializeField] private Tilemap _mainTilemap; // 작물 타일맵
    [SerializeField] private Tilemap _tempTilemap; // 작물 타일맵

    private void Awake()
    {
        GameManager.Instance.BuildingM.gridLayout = this.GetComponent<GridLayout>();
        GameManager.Instance.BuildingM.mainTilemap = _mainTilemap;
        GameManager.Instance.BuildingM.tempTilemap = _tempTilemap;
    }
}