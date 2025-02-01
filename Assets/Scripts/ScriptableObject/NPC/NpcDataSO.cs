using UnityEngine;


[CreateAssetMenu(fileName = "NPCData_", menuName = "NPC/NPCData", order = 1)]
public class NpcDataSO : ScriptableObject
{
    public NpcName npcName;
    public Sprite[] faceSprite;
}