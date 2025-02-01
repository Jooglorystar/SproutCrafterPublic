

using System;

public class MarketPopup : PopupUI
{
    private static ShopSystem _shopSystem;

    public static ShopSystem ShopSystem {  get { return _shopSystem; } }

    public override void Init()
    {
        base.Init();

        if(_shopSystem == null)
        {
            _shopSystem = GetComponentInChildren<ShopSystem>();
        }
    }


    private void OnEnable()
    {
        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI += CloseMarketPopup;
    }


    private void OnDisable()
    {
        GameManager.Instance.CharacterM.InputController.OnInteractPopupUI -= CloseMarketPopup;
    }


    private bool CloseMarketPopup()
    {
        Managers.UI.ClosePopup(gameObject);
        return true;
    }
}