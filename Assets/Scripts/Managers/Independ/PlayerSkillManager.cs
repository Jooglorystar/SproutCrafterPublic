using UnityEngine;


public enum SkillType
{
    Farm,
    Machine,
    Fishing
}


public class PlayerSkillManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.PlayerSkillM = this;
    }
    
    
# region 경험치 조작

    /// <summary>
    /// 플레이어 레벨이 최대치를 넘어가면 Return
    /// </summary>
    public void ProcessIncreaseExp(int p_Value)
    {
        if(Managers.Data.currentLevel >= GameManager.Instance.DataBaseM.SkillInfoDataBase.maxLevel) return;
        
        ProcessExpCheckAndAdd(p_Value);
    }
    

    /// <summary>
    /// 경험치를 더하고 그 값이 해당 레벨의 목표 경험치를 초과하는지 검사
    /// </summary>
    private void ProcessExpCheckAndAdd(int p_Value)
    {
        Managers.Data.currentExp += p_Value;
        
        if (Managers.Data.currentExp >= GameManager.Instance.DataBaseM.SkillInfoDataBase.needExpForNextLevel[Managers.Data.currentLevel - 1])
        {
            ProcessLevelUp();
        }
    }


    /// <summary>
    /// 해당 레벨의 목표 경험치를 초과한다면 레벨을 올려주고 남은 경험치를 다시 더해주는 기능
    /// </summary>
    private void ProcessLevelUp()
    {
        int leftExp = Managers.Data.currentExp - GameManager.Instance.DataBaseM.SkillInfoDataBase.needExpForNextLevel[Managers.Data.currentLevel - 1];
            
        Managers.Data.currentLevel++;
           
        Managers.Data.currentExp = 0;
        Managers.Data.currentExp += leftExp;
    }

# endregion
    
    
/// <summary>
/// 스킬 활성화 여부 돌려주는 기능
/// </summary>
/// <param name="p_FarmSkill"></param>
/// <returns></returns>
    public bool isActiveTargetSkill(FarmSkillEnums p_FarmSkill)
    {
        if (Managers.Data.farmSkillActive[(int)p_FarmSkill])
        {
            return true;
        }
        
        return false;
    }
}