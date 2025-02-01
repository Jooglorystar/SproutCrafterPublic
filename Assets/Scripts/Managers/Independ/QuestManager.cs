using System;
using System.Collections.Generic;
using UnityEngine;


public enum EQuestType
{
    Default,
    Delivery,
    Tutorial
}

public class QuestManager : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();

    [SerializeField] private int _questCount;

    private Quest _selectedQuest;

    public event Action<int> OnCheckItem;

    public event Action OnCheckQuest;

    private void Awake()
    {
        GameManager.Instance.QuestM = this;
    }

    private void OnEnable()
    {
        GameManager.Instance.TimeM.OnDayCheck += AddQuest;
        GameManager.Instance.TimeM.OnDayCheck += DecreaseRemainDay;
    }

    private void OnDisable()
    {
        GameManager.Instance.TimeM.OnDayCheck -= AddQuest;
        GameManager.Instance.TimeM.OnDayCheck -= DecreaseRemainDay;
    }

    /// <summary>
    /// 퀘스트 대상 아이템인지 확인함
    /// </summary>
    /// <param name="p_itemcode"></param>
    public void CheckItem(int p_itemcode)
    {
        OnCheckItem?.Invoke(p_itemcode);
    }

    public bool IsTargetOfQuest(NpcName p_npcName, out Quest p_quest)
    {
        bool isTarget = false;

        p_quest = null;
        for (int i = 0; i < quests.Count; i++)
        {
            if (quests[i] is IHasTargetNPC targetQuest && quests[i].isCompleted && targetQuest.IsCorrectNPC(p_npcName))
            {
                p_quest = quests[i];
                isTarget = true;
            }
        }

        return isTarget;
    }

    /// <summary>
    /// 미수락 상태의 퀘스트가 있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool HasUnacceptedQuest()
    {
        for (int i = 0; i < quests.Count; i++)
        {
            if (!quests[i].isAccepted)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 퀘스트의 기한을 줄이고, 만약 기한이 지나면 사라지게 하는 메서드
    /// </summary>
    public void DecreaseRemainDay()
    {
        for (int i = 0; i < quests.Count; i++)
        {
            if (quests[i] != null)
            {
                quests[i].remainDay--;

                if (quests[i].remainDay <= 0 && !quests[i].isPermanent())
                {
                    quests[i].UnsubscribeEvent();
                    quests.Remove(quests[i]);
                }
            }
        }
    }

    /// <summary>
    /// 퀘스트를 추가 가능할 시 퀘스트 리스트에 추가하는 메서드
    /// </summary>
    public void AddQuest()
    {
        if (CanAddQuest())
        {
            quests.Add(CreateQuest<DeliveryQuest>());
            OnCheckQuest?.Invoke();
        }
    }

    private bool CanAddQuest()
    {
        return quests.Count < _questCount && GameManager.Instance.TimeM.CurrentWeekDay == EWeekday.Monday && GameManager.Instance.TimeM.InGameDay < 20;
    }

    private T CreateQuest<T>() where T : Quest, new()
    {
        T quest = new T();

        return quest;
    }

    #region 퀘스트 저장, 로드
    public List<QuestSaveData> SaveQuest()
    {
        List<QuestSaveData> questSaves = new List<QuestSaveData>();

        foreach (Quest quest in quests)
        {
            questSaves.Add(quest.SaveQuestData());
        }

        return questSaves;
    }

    public void LoadQuest(List<QuestSaveData> p_questSaveDatas)
    {
        for (int i = 0; i < p_questSaveDatas.Count; i++)
        {
            quests.Add(p_questSaveDatas[i].GetQuest());
        }
    }
    #endregion

    #region 퀘스트 수락, 포기, 제거, 선택
    /// <summary>
    /// 선택된 퀘스트를 수락상태로 변경
    /// </summary>
    public void AcceptQuest()
    {
        if (_selectedQuest == null) return;

        _selectedQuest.isAccepted = true;
        _selectedQuest.CheckQuestCondition();
        _selectedQuest = null;
        OnCheckQuest?.Invoke();
    }

    /// <summary>
    /// 선택된 퀘스트를 포기하고 제거함
    /// </summary>
    public void GiveupQuest()
    {
        if (_selectedQuest == null) return;

        _selectedQuest.isAccepted = false;
        quests.Remove(_selectedQuest);
    }

    /// <summary>
    /// 매개변수 퀘스트를 제거함
    /// </summary>
    /// <param name="quest"></param>
    public void RemoveQuest(Quest quest)
    {
        quests.Remove(quest);
    }

    /// <summary>
    /// _selectedQuest를 매개변수 퀘스트로 초기화함
    /// </summary>
    /// <param name="p_quest"></param>
    public void SelectQuest(Quest p_quest)
    {
        if (p_quest == null)
        {
            _selectedQuest = null;
            return;
        }
        _selectedQuest = p_quest;
    }
    #endregion
}
