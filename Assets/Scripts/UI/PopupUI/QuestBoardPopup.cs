using UnityEngine;

public class QuestBoardPopup : PopupUI
{
    [SerializeField] private QuestPaper[] _papers;
    public QuestInformationPanel questInformationPanel;

    public override void Init()
    {
        base.Init();
    }

    public void Awake()
    {
        _papers = GetComponentsInChildren<QuestPaper>();
    }

    private void OnEnable()
    {
        CheckPapers();
    }

    /// <summary>
    /// 수락한 퀘스트는 보드에 뜨지 안도록 하는 메서드
    /// </summary>
    public void CheckPapers()
    {
        for (int i = 0; i < _papers.Length; i++)
        {
            if (GameManager.Instance.QuestM.quests.Count > i && !GameManager.Instance.QuestM.quests[i].isAccepted)
            {
                _papers[i].SetQuest(GameManager.Instance.QuestM.quests[i], i);
            }
            else
            {
                _papers[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnAcceptButton(bool p_isAccepted)
    {
        if (p_isAccepted)
        {
            Managers.Sound.PlaySfx(SfxEnums.QuestAccept);
            GameManager.Instance.QuestM.AcceptQuest();
        }
        questInformationPanel.gameObject.SetActive(false);
        CheckPapers();
    }
}
