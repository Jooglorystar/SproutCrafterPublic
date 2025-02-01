using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialoguePopup : PopupUI
{
    public static bool isDialoging;
    
    [Header("대화 시스템 제어")]
    private bool _isCanGoNext;
    private int _lineCount;
    private int _contextCount;
    
    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI _dialoguerName;
    [SerializeField] private Image _dialoguerSprite;
    [SerializeField] private TextMeshProUGUI _dialogue;

    private List<Dialogue> _dialogueList => Managers.Data.currentDialogue;

    
    private void OnEnable()
    {
        GameManager.Instance.CharacterM.playerMovement.UnSubscribeMoveEvent();
        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI += ReturnIsDialoging;

        StartDialogue();
    }


    private void OnDisable()
    {
        GameManager.Instance.CharacterM.playerMovement.SubscribeMoveEvent();
        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI -= ReturnIsDialoging;
    }


    public void OnClickNextDialogue()
    {
        if (isDialoging && !_isCanGoNext) return;

        _isCanGoNext = false;
        _dialogue.text = "";
        
        if (++_contextCount < _dialogueList[_lineCount].contexts.Length)
        {
            StartCoroutine(TypeWriter());
        }
        else
        {
            if (++_lineCount < _dialogueList.Count)
            {
                _contextCount = 0;
                StartCoroutine(TypeWriter());
            }
            else
            {
                EndDialogue();
            }
        }
    }


    private bool ReturnIsDialoging()
    {
        if (isDialoging)
        {
            OnClickNextDialogue();

            if (!isDialoging)
            {
                return true;
            }
        }

        return isDialoging;
    }
    
    
    private IEnumerator TypeWriter()
    {
        string replaceText = _dialogueList[_lineCount].contexts[_contextCount];
        replaceText = replaceText.Replace("`", ",");

        _dialoguerName.text = _dialogueList[_lineCount].characterName;
        _dialoguerSprite.sprite = Managers.Data.npcDataSO.faceSprite[(int)_dialogueList[_lineCount].faceType[_contextCount]];
        
        for (int i = 0; i < replaceText.Length; i++)
        {
            _dialogue.text += replaceText[i];
            Managers.Sound.PlaySfx(SfxEnums.Texting);
            yield return Setting.NextDelay;
        }
        
        _dialogue.text = replaceText;
        _isCanGoNext = true;
    }


    private void StartDialogue()
    {
        _dialoguerName.text = "";
        _dialogue.text = "";
        isDialoging = true;
        
        StartCoroutine(TypeWriter());
    }


    private void EndDialogue()
    {
        isDialoging = false;
        
        _lineCount = 0;
        _contextCount = 0;
        _isCanGoNext = false;
        
        Managers.UI.ClosePopup(gameObject);
        NPCManager.isDialoging = false;
    }
}