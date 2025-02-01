using System.IO;
using UnityEngine;


public class Managers : MonoBehaviour
{
    private static Managers _instance;
    
    public static UIManager UI { get { return _instance._uiManager; } }
    public static AudioManager Sound { get { return _instance._soundManager; } }
    public static DataManager Data { get { return _instance._dataManager; } }
    public static CursorManager Cursor {get { return _instance._cursorManager; } }
    
    private UIManager _uiManager;
    private CursorManager _cursorManager;
    private AudioManager _soundManager;
    private DataManager _dataManager = new DataManager();
    
    
    /// <summary>
    /// 게임 시작 전 실행되어 Managers 세팅
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    internal static void Init()
    {
        GameObject obj = new GameObject("Managers");
        _instance = obj.AddComponent<Managers>();

        DontDestroyOnLoad(obj);

        _instance._dataManager.Init();
        _instance._uiManager = CreateManager<UIManager>(obj.transform);
        _instance._cursorManager = CreateManager<CursorManager>(obj.transform);
        _instance._soundManager = CreateAudioManager(obj.transform);

        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            Debug.Log(Application.persistentDataPath);
        }
    }
    
    
    /// <summary>
    /// 해당하는 Manager 만들어줌
    /// </summary>
    /// <param name="parent">Managers 트랜스폼 넘겨줌</param>
    /// <typeparam name="T">스크립트 명 넘겨줌</typeparam>
    /// <returns>만들어진 Manager를 돌려주어 Managers의 변수에 할당</returns>
    private static T CreateManager<T>(Transform parent) where T : Component, IInit
    {
        GameObject obj = new GameObject(typeof(T).Name);
        T t = obj.AddComponent<T>();
        obj.transform.parent = parent;

        t.Init();

        return t;
    }
    
    
    private static AudioManager CreateAudioManager(Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Managers/AudioManager");
        if (prefab == null)
        {
            Debug.LogError("AudioManager 프리팹을 찾을 수 없습니다.");
            return null;
        }

        GameObject obj = Instantiate(prefab, parent);
        obj.name = "AudioManager";
        
        AudioManager audioManager = obj.GetComponent<AudioManager>();

        audioManager.Init();

        return audioManager;
    }
}