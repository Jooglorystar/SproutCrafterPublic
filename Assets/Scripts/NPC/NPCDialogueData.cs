using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueData : MonoBehaviour
{
    private DialogueParser _parser;

    [SerializeField] private TextAsset _textAsset;
    [SerializeField] private NpcDataSO _npcDataSo;

    private Dictionary<int, Dialogue> _dialogueDictionary = new Dictionary<int, Dialogue>();


    private void Awake()
    {
        _parser = GetComponentInParent<DialogueParser>();

        Dialogue[] dialogues = _parser.Parse(_textAsset);

        for (int i = 0; i < dialogues.Length; i++)
        {
            _dialogueDictionary.Add(i + 1, dialogues[i]);
        }
    }


    /// <summary>
    /// 필요한 대사를 딕셔너리에 넣고 vector2 의 x와 y를 이용하여 시작 줄과 끝 줄의 내용을 반환함
    /// </summary>
    /// <param name="p_StartNum"></param>
    /// <param name="p_EndNum"></param>
    /// <returns></returns>
    public (Dialogue[], NpcDataSO) GetDialogue(int p_StartNum, int p_EndNum)
    {
        List<Dialogue> dialoguesList = new List<Dialogue>();

        for (int i = 0; i <= p_EndNum - p_StartNum; i++)
        {
            dialoguesList.Add(_dialogueDictionary[p_StartNum + i]);
        }

        return (dialoguesList.ToArray(), _npcDataSo);
    }
}