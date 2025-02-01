using UnityEngine;


[System.Serializable]
public class CursorTexture
{
    public Texture2D cursorTexture;
    public CursorTypes cursorType;
    public Vector2 hotspot;
}


[CreateAssetMenu(fileName = "CursorTextures", menuName = "UI/CursorTextures")]
public class CursorTexturesSO : ScriptableObject
{
    public CursorTexture[] Cursors;
}