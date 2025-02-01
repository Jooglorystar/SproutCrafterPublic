using UnityEngine;

public class AcceptedQuestPaper : DefaultQuestPaper
{
    private AcceptedQuestPopup _acceptedQuestPopup;

    [SerializeField] private GameObject _completedMark;

    private void Awake()
    {
        _acceptedQuestPopup = GetComponentInParent<AcceptedQuestPopup>();
    }

    private void OnEnable()
    {
        CheckCompleted();
    }

    private void CheckCompleted()
    {
        _completedMark.SetActive(_quest.isCompleted);
    }

    public override void OnInformation()
    {
        _acceptedQuestPopup.questInformationPanel.gameObject.SetActive(true);
        _acceptedQuestPopup.questInformationPanel.SetQuest(_quest);
        GameManager.Instance.QuestM.SelectQuest(_quest);
    }
}
