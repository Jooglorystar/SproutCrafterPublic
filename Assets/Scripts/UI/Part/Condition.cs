using UnityEngine;
using UnityEngine.UI;


public class Condition : MonoBehaviour
{
    public float currentValue;
    public float maxValue;
    public Image uiBar;

    private float _dangerLine;

    public void Init()
    {
        RefreshConditon();

        SetDangerLine();
    }


    /// <summary>
    /// 외부에서 호출, Value 증가
    /// </summary>
    /// <param name="value"></param>
    public void IncereaseStatValue(float p_Value)
    {
        currentValue = Mathf.Min(currentValue + p_Value, maxValue);
        RefreshConditon();
    }


    /// <summary>
    /// 외부에서 호출, Value 감소
    /// </summary>
    /// <param name="value"></param>
    public void DecreaseStatValue(float p_Value)
    {
        currentValue = Mathf.Max(currentValue - p_Value, 0);
        RefreshConditon();
    }


    public void RefreshConditon()
    {
        uiBar.fillAmount = SetStatValue();
        SetConditionBarColor();
    }

    private void SetConditionBarColor()
    {
        if (currentValue < _dangerLine)
        {
            uiBar.color = Color.yellow;
        }
        else
        {
            uiBar.color = Color.white;
        }
    }

    private void SetDangerLine()
    {
        _dangerLine = maxValue / 5;
    }

    private float SetStatValue()
    {
        return currentValue / maxValue;
    }
}