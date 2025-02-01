using TMPro;
using UnityEngine;

public class CropPriceBoard : MonoBehaviour, IInteractable
{
    public void OpenPriceUI()
    {
        Managers.UI.OnEnableUI<CropPricePopup>();
    }

    public void Interact()
    {
        OpenPriceUI();
    }
}