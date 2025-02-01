using UnityEngine;

public class LabPopup : PopupUI
{
    [Header("Managers")]
    [SerializeField] private LabSystem labSystem;

    [Header("UI")]
    [SerializeField] private LabUI labUI;

    public override void Init()
    {
        base.Init();

        if (labSystem == null)
        {
            labSystem = GetComponent<LabSystem>();
        }

        if (labUI != null)
        {
            labUI = GetComponentInChildren<LabUI>();
            labUI.Init();
        }
    }
    public void OpenLabPopup(GameObject gameObject)
    {
        gameObject.SetActive(true);
    }
}
