using UnityEngine;


public class NPCManager : MonoBehaviour
{
    public static bool isDialoging;

    public NPCLocationHandler[] npcLocationHandlers;

    
    private void Awake()
    {
        GameManager.Instance.NpcM = this;
        npcLocationHandlers = GetComponentsInChildren<NPCLocationHandler>();
    }

    
    private void OnEnable()
    {
        EventManager.Subscribe(GameEventType.GameStart, CheckNPCLocation);
        EventManager.Subscribe(GameEventType.SceneChange, CheckNPCLocation);
    }

    
    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEventType.GameStart, CheckNPCLocation);
        EventManager.Unsubscribe(GameEventType.SceneChange, CheckNPCLocation);
    }

    
    /// <summary>
    /// 등록된 NPC들의 씬 정보와 플레이어의 현재 씬 정보를 비교하여 NPC를 활성화/비활성화 해줌
    /// </summary>
    private void CheckNPCLocation(object p_Args)
    {
        foreach (NPCLocationHandler handler in npcLocationHandlers)
        {
            handler.CheckLocation();
        }
    }
}