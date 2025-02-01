using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCLocationHandler : MonoBehaviour
{
    [SerializeField] private SceneName _locatedScene;
    private string _locatedSceneName = string.Empty;

    public void CheckLocation()
    {
        if (_locatedSceneName == string.Empty)
        {
            _locatedSceneName = _locatedScene.EnumToString();
        }

        if (SceneManager.GetActiveScene().name != _locatedSceneName)
        {
            gameObject.SetActive(false);

            if (SceneManager.GetActiveScene().name == "InGame" && _locatedSceneName == "Farm") // 수정 필요
            {
                gameObject.SetActive(true);
            }
            return;
        }
        gameObject.SetActive(true);
    }
}