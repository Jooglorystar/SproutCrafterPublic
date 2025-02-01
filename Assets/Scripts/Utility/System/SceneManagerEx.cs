using UnityEngine.Events;
using UnityEngine.SceneManagement;


public static class SceneManagerEx
{
    /// <summary>
    /// 씬 로드 완료 후 실행될 이벤트들을 등록함
    /// </summary>
    /// <param name="callback">로드된 씬 정보, 씬 로드 방식</param>
    public static void OnLoadCompleted(UnityAction<Scene, LoadSceneMode> p_Callback)
    {
        SceneManager.sceneLoaded += p_Callback;
    }
}