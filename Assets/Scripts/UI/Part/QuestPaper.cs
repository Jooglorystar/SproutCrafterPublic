using UnityEngine;

public abstract class DefaultQuestPaper : MonoBehaviour
{
    protected Quest _quest;

    public abstract void OnInformation();

    public void SetQuest(Quest p_quest, int p_indexNum)
    {
        _quest = p_quest;
        gameObject.SetActive(true);
    }

    public void CheckPaper()
    {
        if (_quest == null) return;

        gameObject.SetActive(!_quest.isAccepted);
    }
}

public class QuestPaper : DefaultQuestPaper
{
    private QuestBoardPopup _questBoardPopup;


    private void Awake()
    {
        _questBoardPopup = GetComponentInParent<QuestBoardPopup>();
    }

    public override void OnInformation()
    {
        _questBoardPopup.questInformationPanel.gameObject.SetActive(true);
        _questBoardPopup.questInformationPanel.SetQuest(_quest);
        GameManager.Instance.QuestM.SelectQuest(_quest);
    }

}
