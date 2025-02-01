using UnityEngine;

public interface IHasTargetNPC
{
    public bool IsCorrectNPC(NpcName p_npcName);
}

[System.Serializable]
public class DeliveryQuest : Quest, IHasTargetNPC
{
    public int targetCropCode;
    public int targetAmount;

    public NpcName targetName;

    private int reward = 20;

    public DeliveryQuest()
    {
        targetName = GetRandomTarget();
        remainDay = SetRamainDay();
        targetCropCode = SetTargetCropCode();
        targetAmount = remainDay;
        isAccepted = false;
        isCompleted = false;
        questType = EQuestType.Delivery;

        isDeniableQuest = true;
        isNoDeadline = false;

        GameManager.Instance.QuestM.OnCheckItem += CheckItem;
    }

    public override string GetQuestTitleText()
    {
        return $"{ItemCodeMapper.GetItemSo(targetCropCode).itemName}";
    }

    public override string GetQuestContentText()
    {
        return $"Bring {targetAmount} {ItemCodeMapper.GetItemSo(targetCropCode).itemName} to {targetName}";
    }

    public override string GetQuestRewardText()
    {
        return $"Reward: {reward * targetAmount}";
    }

    public override void CheckQuestCondition()
    {
        CheckItem(targetCropCode);
    }
    private void CheckItem(int p_itemCode)
    {
        if (p_itemCode != targetCropCode) return;

        if (GameManager.Instance.CharacterM.inventory.IsHaveItem(p_itemCode, out _, out int amount))
        {
            isCompleted = amount >= targetAmount;
        }
    }

    private NpcName GetRandomTarget()
    {
        NpcName[] npcNames = (NpcName[])System.Enum.GetValues(typeof(NpcName));
        int npc = Random.Range(0, npcNames.Length);

        return npcNames[npc];
    }

    private int SetTargetCropCode()
    {
        int num;
        do
        {
            num = Random.Range(30000, 30010);
        } while (!ItemCodeMapper.GetItemSo(num));

        return num;
    }
    
    public bool IsCorrectNPC(NpcName p_npcName)
    {
        if (targetName == p_npcName)
        {
            return true;
        }
        return false;
    }

    public override void Complete()
    {
        if (!isCompleted) return;
        
        Managers.Data.gold += reward * targetAmount;
        Managers.Sound.PlaySfx(SfxEnums.Complete);
        GameManager.Instance.CharacterM.inventory.IsHaveItem(targetCropCode, out int indexnum, out _);
        GameManager.Instance.CharacterM.inventory.SetSlotCount(indexnum, -targetAmount);
        GameManager.Instance.QuestM.RemoveQuest(this);
    }
    public override void UnsubscribeEvent()
    {
        GameManager.Instance.QuestM.OnCheckItem -= CheckItem;
    }

    public override QuestSaveData SaveQuestData()
    {
        return new QuestSaveData(this);
    }

    public override string ConvertQuestData()
    {
        string json = JsonUtility.ToJson(this);
        return json;
    }
}