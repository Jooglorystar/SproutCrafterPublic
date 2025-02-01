using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    private enum EShopState
    {
        None,
        Buy,
        Sell
    }

    [Header("구매 판매 탭")]
    [SerializeField] private GameObject _buyTab;
    [SerializeField] private GameObject _sellTab;
    [SerializeField] private Button _buySellButton;

    [Header("TextMeshPro")]
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private TextMeshProUGUI _totalGoldText;

    [Header("구매 관련 필드")]
    [SerializeField] private Transform _itemStockContainer;
    private List<ShopStockPanel> _stockPanels = new List<ShopStockPanel>();
    public GameObject _itemStockinformation; // 구매 아이템 UI

    [Header("선택한 아이템 정보창")]
    [SerializeField] private ItemInformationPanel _itemInformationPanel; // 선택한 아이템 정보 표시

    private SellingItemPanel[] _sellPanels;

    private EShopState _shopState;
    private int _selectedItemIndex; // 선택된 아이템

    private ShopItemPanel _curItemPanel;
    private ShopItemPanel _prevItemPanel;

    [Header("복수구매 관련 필드")]
    [SerializeField] private GameObject _amountContainer;
    private int _amount = 1;
    private int _minAmount = 1;
    private int _maxAmount;

    private int MinAmount
    {
        get
        {
            return _minAmount;
        }
    }
    private int MaxAmount
    {
        get
        {
            _maxAmount = 0;

            if (_selectedItemIndex < 0)
                return 0;

            if (_shopState == EShopState.Sell)
            {
                _maxAmount = Managers.Data.inventorySlots[_selectedItemIndex].itemCount;
            }
            else if (_shopState == EShopState.Buy)
            {
                _maxAmount = Managers.Data.gold / _stockPanels[_selectedItemIndex].ItemSO.buyPrice;
                _maxAmount = Math.Min(_maxAmount, _stockPanels[_selectedItemIndex].ItemSO.maxStack);
            }

            return _maxAmount;
        }
    }

    private void Awake()
    {
        _sellPanels = GetComponentsInChildren<SellingItemPanel>();
        SetSellingItemIndex();
    }

    private void OnEnable()
    {
        _shopState = EShopState.Buy;
        ChangeShopState();
        SetPlayerGoldText();
        ResetAmount();
        _itemStockContainer.localPosition = Vector3.zero;
        _selectedItemIndex = -1;

        DeactivateTrade();

        _itemInformationPanel.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ClearPanel();
        _selectedItemIndex = -1;
    }

    // 상태에 따라 탭 제어 메서드
    private void ChangeShopState()
    {
        switch (_shopState)
        {
            case EShopState.Buy:
                _buyTab.SetActive(true);
                _sellTab.SetActive(false);
                break;
            case EShopState.Sell:
                _buyTab.SetActive(false);
                _sellTab.SetActive(true);
                break;
            default:
                _buyTab.SetActive(false);
                _sellTab.SetActive(false);
                break;
        }
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        DeactivateTrade();
    }

    /// <summary>
    /// 판매할 아이템 슬롯의 IndexNumber 지정 메서드
    /// </summary>
    private void SetSellingItemIndex()
    {
        for (int i = 0; i < _sellPanels.Length; i++)
        {
            _sellPanels[i].SetIndex(i);
        }
    }

    /// <summary>
    /// 판매할 아이템 정보 초기화용 메서드
    /// </summary>
    private void SetSellingItems()
    {
        for (int i = 0; i < _sellPanels.Length; i++)
        {
            _sellPanels[i].SetSellingItem(i, this);
        }
    }

    /// <summary>
    /// 구매할 아이템 정보 초기화용 메서드
    /// </summary>
    /// <param name="p_shopKeeper"></param>
    public void SetItemStocks(ShopKeeper p_shopKeeper)
    {
        for (int i = 0; i < p_shopKeeper.itemStocks.Count; i++)
        {
            if (_stockPanels.Count < p_shopKeeper.itemStocks.Count)
            {
                ShopStockPanel itemPanel = Instantiate(_itemStockinformation, _itemStockContainer).GetComponent<ShopStockPanel>();
                _stockPanels.Add(itemPanel);
            }
            _stockPanels[i].gameObject.SetActive(true);
            _stockPanels[i].SetShopStock(p_shopKeeper.itemStocks[i], this);
            _stockPanels[i].SetIndex(i);
        }
    }

    /// <summary>
    /// 구매할 아이템 판넬 초기화용
    /// </summary>
    private void ClearPanel()
    {
        for (int i = 0; i < _stockPanels.Count; i++)
        {
            _stockPanels[i].ReleaseStock();
        }
    }

    /// <summary>
    /// 복수구매량을 수정하는 메서드
    /// </summary>
    /// <param name="p_value"></param>
    private void ModifyAmount(int p_value)
    {
        _amount += p_value;
        if (_amount <= MinAmount || _amount >= MaxAmount)
        {
            _amount = Mathf.Clamp(_amount, MinAmount, MaxAmount);
            Managers.Sound.PlaySfx(SfxEnums.ButtonClickNegative);
        }
        else
        {
            Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        }
        SetAmountText();
        SetTotalGoldText();
    }

    private void ResetAmount()
    {
        _amount = 1;
        SetAmountText();
    }

    /// <summary>
    /// 복수 구매량 표시 텍스트 초기화
    /// </summary>
    private void SetAmountText()
    {
        _amountText.text = _amount.ToString();
    }

    /// <summary>
    /// 플레이어 소지 골드 텍스트 초기화
    /// </summary>
    private void SetPlayerGoldText()
    {
        _goldText.text = $"{Managers.Data.gold} G";
    }

    private void SetTotalGoldText()
    {
        if (_selectedItemIndex < 0) return;
        int price;

        if (_shopState == EShopState.Sell)
        {
            if (_sellPanels[_selectedItemIndex].ItemSO == null) return;

            price = (int)(_sellPanels[_selectedItemIndex].ItemSO.sellPrice * GameManager.Instance.DynamicPricingSystem.GetSellPriceModifier(Managers.Data.inventorySlots[_selectedItemIndex].itemSo)) * _amount;
            _totalGoldText.text = $"+ {price} G";
        }
        else if (_shopState == EShopState.Buy)
        {
            price = _stockPanels[_selectedItemIndex].ItemSO.buyPrice * _amount;
            _totalGoldText.text = $"- {price} G";
        }
    }

    public int GetSelectedItemIndex()
    {
        return _selectedItemIndex;
    }

    private void SetCurrentItemPanel(int p_selectedindex)
    {
        if (_selectedItemIndex < 0) return;

        if (_shopState == EShopState.Sell)
        {
            if (_curItemPanel != _sellPanels[p_selectedindex])
            {
                SetPreviousItemPanel();
                _curItemPanel = _sellPanels[p_selectedindex];
            }
        }
        else if (_shopState == EShopState.Buy)
        {
            if (_curItemPanel != _stockPanels[p_selectedindex])
            {
                SetPreviousItemPanel();
                _curItemPanel = _stockPanels[p_selectedindex];
            }
        }
    }

    private void SetPreviousItemPanel()
    {
        if (_curItemPanel != null)
            _prevItemPanel = _curItemPanel;
    }

    private void ActivateTrade()
    {
        _buySellButton.gameObject.SetActive(true);
        _amountContainer.SetActive(true);
    }

    private void DeactivateTrade()
    {
        _buySellButton.gameObject.SetActive(false);
        _amountContainer.SetActive(false);
    }

    private void TogglePanel()
    {
        _curItemPanel?.ToggleSelect();
        _prevItemPanel?.ToggleSelect();
    }

    #region 구매 판매 관련 메서드
    /// <summary>
    /// 아이템 선택
    /// </summary>
    /// <param name="p_index"></param>
    public void SelectItem(int p_index)
    {
        if (p_index >= 0)
        {
            if(_shopState == EShopState.Sell)
            {
                if (_sellPanels[p_index].ItemSO == null)
                {
                    _itemInformationPanel.ResetText();
                    DeactivateTrade();
                }
                else
                {
                    _itemInformationPanel.SetText(_sellPanels[p_index].ItemSO, true);
                    ActivateTrade();
                }
            }
            else if(_shopState == EShopState.Buy)
            {
                _itemInformationPanel.SetText(_stockPanels[p_index].ItemSO, false);
                ActivateTrade();
            }
        }
        _selectedItemIndex = p_index;
        ResetAmount();
        SetCurrentItemPanel(_selectedItemIndex);
        TogglePanel();
        SetTotalGoldText();
    }

    /// <summary>
    /// 판매하는 메서드
    /// </summary>
    /// <param name="p_slotIndex">판매할 아이템 슬롯 인덱스</param>
    /// <param name="p_amount">판매할 양</param>
    public void Sell(int p_slotIndex, int p_amount)
    {
        if (!CanSell(p_slotIndex, p_amount)) return;

        Managers.Data.gold += (int)(Managers.Data.inventorySlots[p_slotIndex].itemSo.sellPrice
            * GameManager.Instance.DynamicPricingSystem.GetSellPriceModifier(Managers.Data.inventorySlots[p_slotIndex].itemSo))
            * p_amount;

        Managers.Sound.PlaySfx(p_amount > 1 ? SfxEnums.CoinMulti : SfxEnums.CoinSingle);

        if (Managers.Data.inventorySlots[p_slotIndex].itemSo is CropsSO crop)
        {
            GameManager.Instance.DynamicPricingSystem.SellCrop(crop, p_amount);
        }

        GameManager.Instance.CharacterM.inventory.SetSlotCount(p_slotIndex, -p_amount);

        if (Managers.Data.inventorySlots[p_slotIndex].itemSo == null)
        {
            _itemInformationPanel.ResetText();
        }
        ResetAmount();
    }

    /// <summary>
    /// 구매하는 메서드
    /// </summary>
    /// <param name="p_selectedItem">구매할 아이템의 ItemSO</param>
    /// <param name="p_amount">구매할 양</param>
    public void Buy(int p_selectedItem, int p_amount)
    {
        if (!CanBuy(p_selectedItem, p_amount)) return;

        GameManager.Instance.CharacterM.inventory.AcquireItem(_stockPanels[p_selectedItem].ItemSO, p_amount);
        Managers.Sound.PlaySfx(p_amount > 1 ? SfxEnums.CoinMulti : SfxEnums.CoinSingle);
        Managers.Data.gold -= _stockPanels[p_selectedItem].ItemSO.buyPrice * p_amount;
        ResetAmount();
    }

    // 판매 가능 여부 체크
    private bool CanSell(int p_slotIndex, int p_amount)
    {
        if (p_slotIndex < 0) return false;

        return (Managers.Data.inventorySlots[p_slotIndex].itemSo != null
            && Managers.Data.inventorySlots[p_slotIndex].itemCount >= p_amount
            && p_amount > 0);
    }

    // 구매 가능 여부 체크
    private bool CanBuy(int p_slotIndex, int p_amount)
    {
        if (p_slotIndex < 0) return false;

        return (_stockPanels[p_slotIndex].ItemSO != null
            && Managers.Data.gold >= _stockPanels[p_slotIndex].ItemSO.buyPrice * p_amount
            && p_amount > 0);
    }
    #endregion

    #region 버튼 동작 메서드

    public void OnBuyTabButton()
    {
        if (_shopState == EShopState.Buy)
            return;

        _shopState = EShopState.Buy;
        ResetAmount();
        _itemStockContainer.localPosition = Vector3.zero;
        SelectItem(-1);
        _itemInformationPanel.ResetText();
        ChangeShopState();
    }

    public void OnSellTabButton()
    {
        if (_shopState == EShopState.Sell)
            return;

        _shopState = EShopState.Sell;
        ResetAmount();
        SelectItem(-1);
        _itemInformationPanel.ResetText();
        ChangeShopState();
        SetSellingItems();
    }

    public void OnTradeButton()
    {
        if (_shopState == EShopState.Buy)
        {
            Buy(_selectedItemIndex, _amount);
            SelectItem(-1);
            SetPlayerGoldText();
        }
        else if (_shopState == EShopState.Sell)
        {
            Sell(_selectedItemIndex, _amount);
            SelectItem(-1);
            SetSellingItems();
            SetPlayerGoldText();
        }
        DeactivateTrade();
    }

    public void OnAmountIncreaseButton()
    {
        ModifyAmount(1);
    }

    public void OnAmountDecreaseButton()
    {
        ModifyAmount(-1);
    }
    #endregion
}