using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UISkillLevelUp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SkillPopup _skillLevelController;
    
    [SerializeField] private Button _button;
    [SerializeField] private SkillType _skillType;
    [SerializeField] private int _uiIndex;


    private void Start()
    {
        _button.image.sprite = _skillLevelController.ShowSkillState(_skillType, _uiIndex);
    }


    /// <summary>
    /// 버튼 ui위에 손을 올리면 스킬 세부 정보 패널을 띄워줌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        _skillLevelController.ShowSkillDetailsPanel(_skillType, _uiIndex);
    }

    
    /// <summary>
    /// 버튼 ui위에서 손을 때면 스킬 세부 정보 패널을 꺼줌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        _skillLevelController.CloseSkillDetailsPanel();
    }
    

    public void OnClickLearnSkill()
    {
        _skillLevelController.LearnSkill(_skillType, _uiIndex);
    }
}