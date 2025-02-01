using UnityEngine;

[System.Serializable]
public abstract class Quest
{
    public int remainDay = 1;
    public EQuestType questType = EQuestType.Default;
    public bool isAccepted;

    private int minRemainDay = 15;
    private int maxRemainDay = 18;

    protected bool isDeniableQuest;
    protected bool isNoDeadline;

    public bool isCompleted;

    protected int SetRamainDay()
    {
        return Random.Range(minRemainDay, maxRemainDay);
    }

    public abstract string GetQuestTitleText();
    public abstract string GetQuestContentText();
    public string GetQuestRemainDayText()
    {
        if (isNoDeadline) return $"No Deadline";
        return $"{remainDay} Days Left";
    }
    public abstract string GetQuestRewardText();

    public bool isDeniable()
    {
        return isDeniableQuest;
    }

    public bool isPermanent()
    {
        return isNoDeadline;
    }

    public abstract void CheckQuestCondition();
    public abstract void Complete();

    public abstract void UnsubscribeEvent();

    public abstract QuestSaveData SaveQuestData();
    public abstract string ConvertQuestData();
}

[System.Serializable]
public class QuestSaveData
{
    public EQuestType questType;
    public string questData;

    public QuestSaveData(Quest p_quest)
    {
        questType = p_quest.questType;
        questData = p_quest.ConvertQuestData();
    }

    /// <summary>
    /// questData를 알맞는 퀘스트로 전환하는 메서드
    /// </summary>
    /// <returns></returns>
    public Quest GetQuest()
    {
        switch(questType)
        {
            case EQuestType.Delivery:
                return JsonUtility.FromJson<DeliveryQuest>(questData);
        }
        return null;
    }
}