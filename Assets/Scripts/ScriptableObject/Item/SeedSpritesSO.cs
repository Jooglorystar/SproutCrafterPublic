using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "SeedSpriteSO_", menuName = "Item/SeedSpriteSO")]
public class SeedSpritesSO : ScriptableObject
{
    public int ItemCode;
    public Sprite[] sprites;
    public TileBase[] tiles;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidateHelper.ValidateCheckEnumerableObject(this, nameof(sprites), sprites);
    }
#endif
}