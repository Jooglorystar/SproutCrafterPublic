using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourcesPlaceTilemap : MonoBehaviour
{
    [SerializeField] private Tilemap _treeGoundTilemap; // 땅 타일맵


    private void Awake()
    {
        GameManager.Instance.TilemapM.TreeGroundTilemap = _treeGoundTilemap;
    }

    private void OnEnable()
    {
        GameManager.Instance.TilemapM.UpdateTile();
    }
}
