using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;


public class CinemachineConfinerChanger : MonoBehaviour
{
    private CinemachineConfiner2D _cinemachineConfiner2D;

    
    private void Awake()
    {
        _cinemachineConfiner2D = GetComponent<CinemachineConfiner2D>();
    }


    private void Start()
    {
        ChangeConfiner(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
        
        EventManager.Subscribe(GameEventType.SceneChange, ChangeConfiner);
    }

    
    /// <summary>
    /// 씬 이동시 CinemachineConfiner를 교체하는 기능
    /// </summary>
    /// <param name="p_Args">null 값 넣기</param>
    private void ChangeConfiner(object p_Args)
    {
        PolygonCollider2D polygonCollider2D = GameObject.FindGameObjectWithTag(Setting.ConfinerTag).GetComponent<PolygonCollider2D>();
        
        _cinemachineConfiner2D.m_BoundingShape2D = polygonCollider2D;
        
        _cinemachineConfiner2D.InvalidateCache();
    }
}