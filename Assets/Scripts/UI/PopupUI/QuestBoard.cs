using UnityEngine;

public class QuestBoard : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _questMark;

    public void Start()
    {
        GameManager.Instance.QuestM.OnCheckQuest += ControlQuestMark;
        ControlQuestMark();
    }

    public void OnDisable()
    {
        GameManager.Instance.QuestM.OnCheckQuest -= ControlQuestMark;
    }

    public void Interact()
    {
        Managers.UI.OnEnableUI<QuestBoardPopup>();
    }

    public void ControlQuestMark()
    {
        _questMark.SetActive(GameManager.Instance.QuestM.HasUnacceptedQuest());
    }
}