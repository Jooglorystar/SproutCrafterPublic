using UnityEngine;
using UnityEngine.EventSystems;


public class HUDController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Condition hpCondition;
    public Condition staminaCondition;

    
    public void Init()
    {
        GameManager.Instance.CharacterM.playerCondition.hudController = this;

        hpCondition.maxValue = Managers.Data.maxHp;
        hpCondition.currentValue = hpCondition.currentValue == 0 ? hpCondition.currentValue = Managers.Data.maxHp : hpCondition.currentValue = Managers.Data.currentHp;
        hpCondition.Init();

        staminaCondition.maxValue = Managers.Data.maxStamina;
        staminaCondition.currentValue = staminaCondition.currentValue == 0 ? staminaCondition.currentValue = Managers.Data.maxStamina : staminaCondition.currentValue = Managers.Data.currentStamina;
        staminaCondition.Init();
    }

    
    /// <summary>
    /// 정확한 수치를 보여주는 기능
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // _ShowStateText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // _ShowStateText.SetActive(true);
    }
}