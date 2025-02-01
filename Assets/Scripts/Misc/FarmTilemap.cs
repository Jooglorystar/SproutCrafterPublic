using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap _treeGoundTilemap; // 나무 타일맵

    private void Awake()
    {
        GameManager.Instance.TilemapM.TreeGroundTilemap = _treeGoundTilemap; //나무
    }
}
