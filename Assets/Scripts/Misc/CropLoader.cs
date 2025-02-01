using UnityEngine;

public class CropLoader : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.CropM.ActivateCrops();
        GameManager.Instance.BuildingM.ActivateBuildings();
    }

    private void OnDisable()
    {
        GameManager.Instance.CropM?.DeactivateCrops();
        GameManager.Instance.BuildingM?.DeactivateBuildings();
    }
}
