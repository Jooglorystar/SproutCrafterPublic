using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour, IInteractable
{
    public List<ItemSO> itemStocks;

    private ShopSystem _shopSystem;

    /// <summary>
    /// 상점창 여는 메서드
    /// </summary>
    /// <param name="shopKeeper"></param>
    public void OpenShopUI(ShopKeeper shopKeeper)
    {
        Managers.UI.OnEnableUI<MarketPopup>();

        _shopSystem = MarketPopup.ShopSystem;
        _shopSystem.SetItemStocks(shopKeeper);
    }

    public void Interact()
    {
        OpenShopUI(this);
    }
}
