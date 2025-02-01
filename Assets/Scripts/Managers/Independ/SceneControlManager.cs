using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControlManager : MonoBehaviour
{    
    private void Awake()
    {
        GameManager.Instance.SceneControlM = this;
    }


#region 씬 전환

    public void MoveSceneWithFade(SceneName sceneName, Vector3 spawnPosition)
    {
        GameManager.Instance.StartFade(() => MoveScene(sceneName, spawnPosition));
    }
        
    /// <summary>
    /// 씬을 전환하고 플레이어의 위치를 설정해주는 역할
    /// </summary>
    public IEnumerator MoveScene(SceneName sceneName, Vector3 spawnPosition)
    {
        // 현재 씬의 데이터 저장
        SaveCurrentSceneData();

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        
        // 새로운 씬 로드
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName.EnumToString()));

        // 플레이어 위치 설정
        GameManager.Instance.CharacterM.CurrentScene = sceneName;
        GameManager.Instance.CharacterM.transform.position = spawnPosition;
        EventManager.Dispatch(GameEventType.SceneChange, sceneName);
    }

    /// <summary>
    /// 씬 로드 및 활성화
    /// </summary>
    /// <param name="sceneName">로드할 씬 이름</param>
    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);

        RestoreSceneData();
    }

#endregion
    

#region 씬 데이터 저장/복원

    /// <summary>
    /// 현재 씬 데이터를 저장
    /// </summary>
    private void SaveCurrentSceneData()
    {
        //GameManager.Instance.TilemapM?.SaveTileData();
    }

    /// <summary>
    /// 로드된 씬 데이터를 복원
    /// </summary>
    private void RestoreSceneData()
    {
        //GameManager.Instance.TilemapM?.LoadTileData();
    }

#endregion

}