using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SkillPopup : PopupUI
{
    [Header("플레이어 스킬 정보")]
    [SerializeField] private TextMeshProUGUI _currentLevelText;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private Image _expBarImage;
    
    [SerializeField] private Button _resetSkillButton;
    private int _previousIndex;
    
    [Header("스킬 세부 정보 패널")]
    [SerializeField] private GameObject _skillDetailPanel;
    [SerializeField] private Image _skillImage;
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDescriptionText;
    [SerializeField] private TextMeshProUGUI _skillPointText;
    
    
    private SkillInfoDataBaseSO _database => GameManager.Instance.DataBaseM.SkillInfoDataBase;
    private bool[] _farmSkillActive => Managers.Data.farmSkillActive;
    
    
    public override void Init()
    {
        base.Init();

        _previousIndex = 1;
        
        _currentLevelText.text = $"보유 스킬 포인트 {Managers.Data.haveSkillPoint}";

        _expText.text = $"{Managers.Data.currentExp} / {_database.needExpForNextLevel[Managers.Data.currentLevel - 1]}";
        _expBarImage.fillAmount = (float)Managers.Data.currentExp / _database.needExpForNextLevel[Managers.Data.currentLevel - 1];
    }

    
    /// <summary>
    /// 해당하는 스킬의 이미지를 반환해줌
    /// </summary>
    public Sprite ShowSkillState(SkillType p_SkillType, int p_Index)
    {
        switch (p_SkillType)
        {
            case SkillType.Farm:
                return _database.farmSkills[p_Index].skillSprite;
            case SkillType.Machine:
                // return 
            case SkillType.Fishing:
                // return 
            default:
                return null;
        }
    }
    
# region 스킬 세부정보 Panel 조절
    
    /// <summary>
    /// 해당하는 스킬의 세부 정보 패널을 설정해줌
    /// </summary>
    public void ShowSkillDetailsPanel(SkillType p_SkillType, int p_Index)
    {
        switch (p_SkillType)
        {
            case SkillType.Farm:
                ShowFarmDetail(p_Index); 
                break;
            case SkillType.Machine:
                break;
            case SkillType.Fishing:
                break;
        }
    }


    /// <summary>
    /// 실제로 스킬의 세부 정보 패널을 설정하고 키고 끄는 기능
    /// </summary>
    /// <param name="p_Index"></param>
    private void ShowFarmDetail(int p_Index)
    {
        _skillDetailPanel.SetActive(true);

        _skillImage.sprite = _database.farmSkills[p_Index].skillSprite;
        _skillNameText.text = _database.farmSkills[p_Index].skillName;
        _skillDescriptionText.text = _database.farmSkills[p_Index].skillDescription;
        
        if (Managers.Data.haveSkillPoint >= _database.farmSkills[p_Index].needPoint)
        {
            _skillPointText.color = Color.black;
        }
        else
        {
            _skillPointText.color = Color.red;
        }
        
        _skillPointText.text = $"요구 스킬 포인트 \n{_database.farmSkills[p_Index].needPoint.ToString()}";
    }
    
    
    /// <summary>
    /// 패널이 비활성화되기전 내용 초기화
    /// </summary>
    public void CloseSkillDetailsPanel()
    {
        _skillImage.sprite = null;
        _skillNameText.text = string.Empty;
        _skillDescriptionText.text = string.Empty;
        _skillPointText.text = string.Empty;
        
        _skillDetailPanel.gameObject.SetActive(false);
    }
# endregion


#region 스킬 획득
    public void LearnSkill(SkillType p_SkillType, int p_Index)
    {
        switch (p_SkillType)
        {
            case SkillType.Farm:
                AcquireFarmSkill(p_Index);
                break;
            case SkillType.Machine:
                break;
            case SkillType.Fishing:
                break;
        }
        
        _currentLevelText.text = $"보유 스킬 포인트 {Managers.Data.haveSkillPoint}";
    }

    
    private void AcquireFarmSkill(int pIndex)
    {
        if(Managers.Data.farmSkillActive[pIndex]) return;
        
        if (CheckEnoughSkillPoint(pIndex) && CheckpreviousLearned(pIndex))
        {
            _farmSkillActive[pIndex] = true;
            Managers.Data.haveSkillPoint -= _database.farmSkills[pIndex].needPoint;
        }
    }
    

    private bool CheckEnoughSkillPoint(int pIndex)
    {
        if (Managers.Data.haveSkillPoint >= _database.farmSkills[pIndex].needPoint)
        {
            return true;
        }
        
        return false;
    }
    
    private bool CheckpreviousLearned(int pIndex)
    {
        if(pIndex == 0 || pIndex == 4 || pIndex == 8) return true;
        
        if(Managers.Data.farmSkillActive[pIndex - _previousIndex]) return true;
        
        return false;
    }

#endregion
    

    private void OnDisable()
    {
        _skillDetailPanel.gameObject.SetActive(false);
    }
    
    
    /// <summary>
    /// 배운 모든 스킬 초기화 기능
    /// </summary>
    public void OnClickResetSkillPointButton()
    {
        for (int i = 0; i < Managers.Data.farmSkillActive.Length; i++)
        {
            if (Managers.Data.farmSkillActive[i])
            {
                Managers.Data.farmSkillActive[i] = false;
                Managers.Data.haveSkillPoint += _database.farmSkills[i].needPoint;
            }
        }
        
        _currentLevelText.text = $"보유 스킬 포인트 {Managers.Data.haveSkillPoint}";
    }
}