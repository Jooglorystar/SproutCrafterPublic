using UnityEngine;


public enum DialogueType
{
    LikeabilityLevel1,
    LikeabilityLevel2,
    LikeabilityLevel3,
    ReceiveGift1,
    ReceiveGift2,
    ReceiveGift3,
    QuestClear
}


public enum FaceType
{
    Normal,
    Talk,
    Happy,
    BigHappy,
    Sad,
    Smile,
    BigSmile,
    XX
}


public class DialogueInteraction : MonoBehaviour
{
    private NPCDialogueData _npcDialogueData;
    
    [SerializeField] private DialogueEvent[] _dialogueEvent;


    private void Awake()
    {
        _npcDialogueData = GetComponent<NPCDialogueData>();
    }


    /// <summary>
    /// 필요한 대사를 반환해줌
    /// </summary>
    /// <param name="p_DialogueType"></param>
    /// <returns></returns>
    public Dialogue[] GetDialogue(DialogueType p_DialogueType)
    {
        SetDialoguesEventData(p_DialogueType);
        
        return _dialogueEvent[(int)p_DialogueType].dialogues;
    }


    /// <summary>
    /// 필요한 대사의 정확한 위치를 파악하여 돌려줌
    /// </summary>
    /// <param name="p_DialogueType"></param>
    private void SetDialoguesEventData(DialogueType p_DialogueType)
    {
        _dialogueEvent[(int)p_DialogueType].dialogues = _npcDialogueData.GetDialogue((int)_dialogueEvent[(int)p_DialogueType].line.x, (int)_dialogueEvent[(int)p_DialogueType].line.y).Item1;
        Managers.Data.npcDataSO = _npcDialogueData.GetDialogue((int)_dialogueEvent[(int)p_DialogueType].line.x, (int)_dialogueEvent[(int)p_DialogueType].line.y).Item2;
    }
}