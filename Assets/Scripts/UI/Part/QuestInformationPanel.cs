using TMPro;
using UnityEngine;

public class QuestInformationPanel : MonoBehaviour
{
    private TextMeshProUGUI[] _questTexts;
    private TextMeshProUGUI _questTitleText;
    private TextMeshProUGUI _questContentText;
    private TextMeshProUGUI _questRemainDayText;
    private TextMeshProUGUI _questRewardText;

    private void Awake()
    {
        SetTexts();

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
    }

    public void SetQuest(Quest p_quest)
    {
        _questTitleText.text = p_quest.GetQuestTitleText();
        _questContentText.text = p_quest.GetQuestContentText();
        _questRemainDayText.text = p_quest.GetQuestRemainDayText();
        _questRewardText.text = p_quest.GetQuestRewardText();
    }

    private void SetTexts()
    {
        if (_questTexts != null) return;

        _questTexts = GetComponentsInChildren<TextMeshProUGUI>();
        _questTitleText = _questTexts[0];
        _questContentText = _questTexts[1];
        _questRemainDayText = _questTexts[2];
        _questRewardText = _questTexts[3];
    }
}
