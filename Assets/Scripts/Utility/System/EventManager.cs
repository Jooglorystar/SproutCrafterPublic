using System;
using System.Collections.Generic;


public delegate void EventListener(object p_args);


public static class EventManager
{ 
    private static readonly Dictionary<GameEventType, List<EventListener>> EventListenerDic = new Dictionary<GameEventType, List<EventListener>>();

    
    /// <summary>
    /// 구독하는 함수
    /// </summary>
    public static void Subscribe(GameEventType p_type, EventListener p_listener)
    {
        if (!EventListenerDic.TryGetValue(p_type, out var list))
        { 
            list = new List<EventListener>();
            EventListenerDic[p_type] = list;
        }

        list.Add(p_listener);
    }

        
    /// <summary>
    /// 구독취소하는 함수
    /// </summary>
    public static void Unsubscribe(GameEventType p_type, EventListener p_listener)
    {
        if (!EventListenerDic.TryGetValue(p_type, out var list))
        {
            return;
        }

        list.Remove(p_listener);
        
        if (list.Count == 0)
        {
            EventListenerDic.Remove(p_type);
        }
    }

        
    /// <summary>
    /// 이벤트 달성 시 구독한 리스트 실행해주는 함수
    /// </summary>
    public static void Dispatch(GameEventType p_type, object p_args)
    {
        if (!EventListenerDic.TryGetValue(p_type, out var list))
        {
            return;
        }

        foreach (EventListener listener in list)
        {
            try
            {
                listener.Invoke(p_args);
            }
            #pragma warning disable 0168
            catch (Exception e)
            {
                // Debug.LogException(e);
            }
        }
    }
}