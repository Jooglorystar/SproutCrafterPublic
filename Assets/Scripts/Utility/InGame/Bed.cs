using UnityEngine;


public class Bed : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Managers.UI.OnEnableUI<BedPopup>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Managers.UI.CloseAllPopup();
    }
}