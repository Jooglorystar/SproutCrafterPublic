using UnityEngine;

public class AcceptedQuestPopup : PopupUI
{
    [SerializeField] private AcceptedQuestPaper[] _papers;
    public QuestInformationPanel questInformationPanel;
    public DenyConfirmaionPanel denyConfirmaionPanel;

    public override void Init()
    {
        base.Init();
    }

    public void Awake()
    {
        _papers = GetComponentsInChildren<AcceptedQuestPaper>();
    }

    private void OnEnable()
    {
        CheckPapers();
    }

    /// <summary>
    /// 수락하지 않은 퀘스트는 창에 뜨지 않도록 하는 메서드
    /// </summary>
    public void CheckPapers()
    {
        for (int i = 0; i < _papers.Length; i++)
        {
            if (GameManager.Instance.QuestM.quests.Count > i && GameManager.Instance.QuestM.quests[i].isAccepted)
            {
                _papers[i].SetQuest(GameManager.Instance.QuestM.quests[i], i);
            }
            else
            {
                _papers[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnDisplayDenyConfirmaition(bool p_isGiveup)
    {
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        if (p_isGiveup)
        {
            denyConfirmaionPanel.gameObject.SetActive(true);
        }
        questInformationPanel.gameObject.SetActive(false);
        CheckPapers();
    }

    public void OnGiveupButton(bool p_isGiveup)
    {
        if (p_isGiveup)
        {
            Managers.Sound.PlaySfx(SfxEnums.QuestGiveup);
            GameManager.Instance.QuestM.GiveupQuest();
        }
        questInformationPanel.gameObject.SetActive(false);
        denyConfirmaionPanel.gameObject.SetActive(false);
        CheckPapers();
    }
}
