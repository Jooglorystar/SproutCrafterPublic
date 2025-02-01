using UnityEngine;


[CreateAssetMenu(menuName = "Skill/FarmSkillSO", fileName = "FarmSkillSO_index_")]
public class FarmSkillSO : ScriptableObject
{
    public string skillName;
    public Sprite skillSprite;
    public string skillDescription;

    public int needPoint;
    public FarmSkillEnums previousActiveSkill;
}