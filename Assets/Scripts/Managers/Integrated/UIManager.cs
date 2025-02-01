using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour, IInit
{
    private readonly List<PopupUI> _popupsList = new List<PopupUI>();
    public bool isUIPopupOpen = false;

    public event Action OnPopupOpen;
    public event Action OnPopupClose;

    public void Init()
    {
        SceneManagerEx.OnLoadCompleted(OnSceneLoaded);
    }


    private void OnSceneLoaded(Scene p_Scene, LoadSceneMode p_SceneMode)
    {
        if (p_SceneMode == LoadSceneMode.Additive) return;

        ClearPopupAll();
        CreateSceneUI(p_Scene.name);
    }


    /// <summary>
    /// 메인 UI를 만들어줌(Title, InGame)
    /// </summary>
    /// <param name="p_Name">현재 열린 씬 이름</param>
    private void CreateSceneUI(string p_Name)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/Main/{p_Name}");

        if (prefab == null)
        {
            Debug.LogWarning($"{p_Name} 이라는 UI Prefab이 없음");
            return;
        }

        GameObject clone = Instantiate(prefab);

        if (!clone.TryGetComponent(out SceneUI sceneUI))
        {
            Debug.LogError($"게임 오브젝트가 SceneUI를 상속 받아야 합니다 : {prefab}");
            return;
        }

        sceneUI.Init();
    }


    /// <summary>
    /// UI를 키고 끄는 기능
    /// </summary>
    /// <typeparam name="T">키고자하는 Popup의 이름을 넘겨주면 됨</typeparam>
    public void OnEnableUI<T>() where T : PopupUI
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CharacterM.playerMovement.UnSubscribeMoveEvent();
            GameManager.Instance.TimeM.IsPlayingTime = false;
            OnPopupOpen?.Invoke();
        }
        
        PopupUI popupUI = _popupsList.Find(popup => popup.GetType() == typeof(T));

        if (popupUI != null)
        {
            popupUI.gameObject.SetActive(true);
            isUIPopupOpen = true;
        }
        else
        {
            CreatePopup<T>();
        }
    }


    /// <summary>
    /// 팝업이 없다면 PopupUI 생성
    /// </summary>
    private void CreatePopup<T>() where T : PopupUI
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/UI/Popup/{typeof(T).Name}");

        if (prefab == null)
        {
            Debug.LogWarning($"{name} 이라는 팝업이 없음.");
            return;
        }

        GameObject clone = Instantiate(prefab);

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Scene targetScene = SceneManager.GetSceneByBuildIndex(1);
            SceneManager.MoveGameObjectToScene(clone, targetScene);
            // 생성된 UI를 InGame씬에 머무를 수 있도록 함
        }

        if (!clone.TryGetComponent(out T popupUI))
        {
            Debug.LogError($"게임 오브젝트가 popupUI를 상속 받아야 합니다 : {prefab}");
            return;
        }

        _popupsList.Add(popupUI);
        isUIPopupOpen = true;
        popupUI.Init();
    }


    /// <summary>
    /// 팝업 닫음
    /// </summary>
    public void ClosePopup(GameObject p_obj)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CharacterM.playerMovement.SubscribeMoveEvent();
            GameManager.Instance.TimeM.IsPlayingTime = true;
            OnPopupClose?.Invoke();
        }

        p_obj.gameObject.SetActive(false);
        isUIPopupOpen = false;
    }


    /// <summary>
    /// 모든 팝업 닫음
    /// </summary>
    public void CloseAllPopup()
    {
        isUIPopupOpen = false;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CharacterM.playerMovement.SubscribeMoveEvent();

            if (!GameManager.Instance.TimeM.IsSleep)
            {
                GameManager.Instance.TimeM.IsPlayingTime = true;
            }
            OnPopupClose?.Invoke();
        }

        foreach (PopupUI item in _popupsList)
        {
            if (item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 씬 전환시 필요없는 팝업 초기화 Title -> InGame
    /// </summary>
    private void ClearPopupAll()
    {
        isUIPopupOpen = false;
        _popupsList.Clear();
    }
}