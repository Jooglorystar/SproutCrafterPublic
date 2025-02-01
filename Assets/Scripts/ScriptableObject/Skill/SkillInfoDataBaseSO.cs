using UnityEngine;


[CreateAssetMenu(menuName = "Skill/SkillInfoDataBaseSO", fileName = "SkillInfoDataBaseSO")]
public class SkillInfoDataBaseSO : ScriptableObject
{
    [Header("레벨업 수치")]
    public int maxLevel;
    public int[] needExpForNextLevel;
    
    [Header("스킬별 SO")]
    public FarmSkillSO[] farmSkills;
    // 낚시
    // 기계
    // 전투
    // 등등
}