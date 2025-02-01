using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoardInteraction : MonoBehaviour
{
    private void OnMouseDown()
    {
        Managers.UI.OnEnableUI<QuestBoardPopup>();
    }
}
