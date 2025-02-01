using UnityEngine;

public class TreeLoader : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.TreeM.LoadTreeFromTreeStateDictionary();
    }

    private void OnDisable()
    {
        GameManager.Instance.TreeM?.ReleaseTrees();
    }
}