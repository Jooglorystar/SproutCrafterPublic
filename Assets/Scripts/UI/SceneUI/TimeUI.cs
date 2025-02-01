using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _weekDayText;
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _TimeText;
    [SerializeField] private Image _seasonIconImage;

    [SerializeField] private Sprite[] _seasonIconSprites;
    
    private void Start()
    {
        GameManager.Instance.TimeM.OnTimeCheck += UpdateTimeText;
        GameManager.Instance.TimeM.OnDayCheck += UpdateDayText;
        GameManager.Instance.TimeM.OnDayCheck += UpdateSeasonIcon;

        UpdateTimeText();
        UpdateDayText();
        UpdateSeasonIcon();
    }

    
    private void UpdateTimeText()
    {
        _TimeText.text = GameManager.Instance.TimeM.GetTimeToString(); // ToString 쓰는거 바꿔야함
    }

    private void UpdateDayText()
    {
        _dayText.SetText($"{GameManager.Instance.TimeM.InGameDay}");
        _weekDayText.text = WeekDayToString();
    }

    private void UpdateSeasonIcon()
    {
        _seasonIconImage.sprite = _seasonIconSprites[(int)GameManager.Instance.TimeM.CurrentSeason];
    }


    private string WeekDayToString()
    {
        return GameManager.Instance.TimeM.CurrentWeekDay.EnumToString().Substring(0,3);
    }
}