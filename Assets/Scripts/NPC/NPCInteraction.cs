using UnityEngine;
using UnityEngine.EventSystems;


[System.Serializable]
public class NPCData
{
    public int likeability;
    public NpcName npcName;
}


public class NPCInteraction : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler
{
    private DialogueInteraction _dialogueInteraction;
    public NPCData npcData = new NPCData();

    [SerializeField] private GameObject _canDialoguePopup;

    private bool _isCanDialogueToday = true;
    
    #pragma warning disable 0414
    private bool _isCanGiftToday = true;

    
    private void Awake()
    {
        _dialogueInteraction = GetComponent<DialogueInteraction>();
    }


    private void Start()
    {
        GameManager.Instance.TimeM.OnDayCheck += ResetBool;
        GameManager.Instance.TimeM.OnDayCheck += SetPopup;
    }


    private void OnEnable()
    {
        SetPopup();
    }


    public void Interact()
    {
        if (GameManager.Instance.QuestM.IsTargetOfQuest(npcData.npcName, out Quest quest))
        {
            ProcessClearQuest();
            quest?.Complete();
            return;
        }
        ProcessCheckCanDialogueToday();
    }


    /// <summary>
    /// NPC를 클릭했을때 호출, 대화 시작 가능한지 판단
    /// </summary>
    public void ProcessCheckCanDialogueToday()
    {
        if (_isCanDialogueToday)
        {
            ProcessStartNormalDialogue();
            _canDialoguePopup.gameObject.SetActive(false);
            _isCanDialogueToday = false;
        }
    }

    
    /// <summary>
    /// 개별 NPC의 호감도에 따른 대사 분기
    /// </summary>
    private void ProcessStartNormalDialogue()
    {
        _isCanDialogueToday = false;
        DialogueType dialogueType = ReturnNpcLikeability();

        SetDialogueData(dialogueType);

        Managers.UI.OnEnableUI<DialoguePopup>();
    }


    /// <summary>
    /// 개별 NPC에게 선물을 주었을때 호감도에 따른 대사 분기
    /// </summary>
    private void ProcessStartGiftDialogue()
    {
        _isCanGiftToday = false;
        DialogueType dialogueType = ReturnNPCLikeabillityGift();

        SetDialogueData(dialogueType);

        Managers.UI.OnEnableUI<DialoguePopup>();
    }

    
    private void ProcessClearQuest()
    {
        SetDialogueData(DialogueType.QuestClear);

        Managers.UI.OnEnableUI<DialoguePopup>();
    }

    
    private void SetDialogueData(DialogueType p_dialogueType)
    {
        Managers.Data.currentDialogue.Clear();
        Managers.Data.npcDataSO = null;

        foreach (Dialogue item in _dialogueInteraction.GetDialogue(p_dialogueType))
        {
            Managers.Data.currentDialogue.Add(item);
        }
    }
    

    /// <summary>
    /// 현재 호감도에 따른 대사 분기점 반환
    /// </summary>
    private DialogueType ReturnNpcLikeability()
    {
        switch (npcData.likeability)
        {
            case < 3: return DialogueType.LikeabilityLevel1;
            case < 6: return DialogueType.LikeabilityLevel2;
            case <= 10: return DialogueType.LikeabilityLevel3;
        }

        return 0;
    }


    /// <summary>
    /// 현재 호감도에 따른 대사 분기점 반환
    /// </summary>
    private DialogueType ReturnNPCLikeabillityGift()
    {
        switch (npcData.likeability)
        {
            case < 3: return DialogueType.ReceiveGift1;
            case < 6: return DialogueType.ReceiveGift1;
            case <= 10: return DialogueType.ReceiveGift1;
        }

        return 0;
    }


    /// <summary>
    /// 하루가 지나갈때 대화 가능 여부를 초기화
    /// </summary>
    private void ResetBool()
    {
        _isCanDialogueToday = true;
        _isCanGiftToday = true;
    }


    private void SetPopup()
    {
        _canDialoguePopup.gameObject.SetActive(_isCanDialogueToday);
    }


    /// <summary>
    /// 마우스가 NPC에게 들어왔을 때 커서를 바꿔주는 기능
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        Managers.Cursor.SetCursorTexture(CursorTypes.Interaction);
    }


    /// <summary>
    /// 마우스가 NPC에게서 나갔을 때 커서를 바꿔주는 기능
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        Managers.Cursor.SetCursorTexture(CursorTypes.Default);
    }
}