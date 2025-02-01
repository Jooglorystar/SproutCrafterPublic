using UnityEngine;


public class InGame : SceneUI
{
    public static bool isUIOpened = true;
    
    private Inventory _inventory;
    private HUDController _hudController;
    
    [Header("UI 요소")]
    [SerializeField] private GameObject _extendUIBakcGorund; // 확장 UI의 최상단 백그라운드
    [SerializeField] private GameObject _constantSlotUIPanel; // 상시 슬롯 UI
    [SerializeField] private GameObject _ExtendSlotsPanel; // 확장 슬롯 UI
    [SerializeField] private GameObject _logPanel;

    
    public override void Init()
    {
        base.Init();
        EventManager.Subscribe(GameEventType.OnInventory, OnExtendUI);
        
        _inventory = GetComponent<Inventory>();
        _hudController = GetComponentInChildren<HUDController>();
        
        _inventory.Init();
        _hudController.Init();
        
        _extendUIBakcGorund.SetActive(false);
        _ExtendSlotsPanel.SetActive(false);
        isUIOpened = false;
    }

    private void OnEnable()
    {
        Managers.UI.OnPopupOpen += CloseConstantSlotUIPanel;
        Managers.UI.OnPopupClose += OpenConstantSlotUIPanel;
    }

    /// <summary>
    /// 플레이어 인풋 컨트롤에서 이용하여 인벤토리를 열고 닫음
    /// </summary>
    private void OnExtendUI(object args)
    {
        if(DialoguePopup.isDialoging || FishingPopup.IsFishing) return;
        
        if (!isUIOpened)
        {
            Managers.Sound.PlaySfx(SfxEnums.InventoryOpen);

            Managers.UI.CloseAllPopup();
            Managers.UI.OnEnableUI<PlayerInfomationPopup>();
            
            _extendUIBakcGorund.SetActive(true);
            _constantSlotUIPanel.SetActive(false);
            _ExtendSlotsPanel.SetActive(true);
            _logPanel.SetActive(false);
            
            isUIOpened = true;
        }
        else
        {
            Managers.Sound.PlaySfx(SfxEnums.InventoryClose);
            
            Managers.UI.CloseAllPopup();
            
            _extendUIBakcGorund.SetActive(false);
            _constantSlotUIPanel.SetActive(true);
            _ExtendSlotsPanel.SetActive(false);
            _logPanel.SetActive(true);
            
            isUIOpened = false;
        }
    }


    private void OnDisable()
    {
        EventManager.Unsubscribe(GameEventType.OnInventory, OnExtendUI);
        Managers.UI.OnPopupOpen -= CloseConstantSlotUIPanel;
        Managers.UI.OnPopupClose -= OpenConstantSlotUIPanel;
    }

    private void OpenConstantSlotUIPanel()
    {
        _constantSlotUIPanel.SetActive(true);
    }

    private void CloseConstantSlotUIPanel()
    {
        _constantSlotUIPanel.SetActive(false);
    }


#region 버튼을 이용한 ui 키고 끄는 기능

    public void OnClickInventoryUIButton()
    {
        Managers.UI.CloseAllPopup();
        Managers.UI.OnEnableUI<PlayerInfomationPopup>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        
        _ExtendSlotsPanel.gameObject.SetActive(true);
    }


    public void OnClickCombinationButton()
    {
        Managers.UI.CloseAllPopup();
        Managers.UI.OnEnableUI<CombinationPopup>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        
        _ExtendSlotsPanel.gameObject.SetActive(true);
    }


    public void OnClickSkillButton()
    {
        Managers.UI.CloseAllPopup();
        Managers.UI.OnEnableUI<SkillPopup>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        
        _ExtendSlotsPanel.gameObject.SetActive(false);
    }
    

    public void OnClickQuestButton()
    {
        Managers.UI.CloseAllPopup();
        Managers.UI.OnEnableUI<AcceptedQuestPopup>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);

        _ExtendSlotsPanel.gameObject.SetActive(false);
    }

    public void OnClickInGameManual()
    {
        Managers.UI.CloseAllPopup();
        Managers.UI.OnEnableUI<InGameManual>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);

        _ExtendSlotsPanel.gameObject.SetActive(false);
    }


    public void OnClickSettingsButton()
    {
        Managers.UI.CloseAllPopup();
        Managers.UI.OnEnableUI<SettingsPopup>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        
        _ExtendSlotsPanel.gameObject.SetActive(false);
    }
    
    
    public void OnClickQuitGameButton()
    {
        OnExtendUI(null);
        Managers.UI.CloseAllPopup();
        Managers.UI.OnEnableUI<QuitGamePopup>();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        
        _ExtendSlotsPanel.gameObject.SetActive(false);
    }
#endregion
}