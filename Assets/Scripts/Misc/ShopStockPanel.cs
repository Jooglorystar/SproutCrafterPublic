using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopItemPanel : MonoBehaviour
{
    [SerializeField] protected Image _panel;
    [SerializeField] protected Image _itemIcon;
    protected ItemSO _itemSO;

    protected int _slotIndex;

    protected ShopSystem _shopSystem;

    protected Color _defaultColor;
    protected Color _selectedColor;

    public ItemSO ItemSO { get { return _itemSO; } }

    public void SetIndex(int p_index)
    {
        _slotIndex = p_index;
    }

    public void ToggleSelect()
    {
        if (_defaultColor == null || _selectedColor == null) return;

        _panel.color = _shopSystem.GetSelectedItemIndex() == _slotIndex ? _panel.color = _selectedColor : _panel.color = _defaultColor;
    }
}

public class ShopStockPanel : ShopItemPanel
{
    private TextMeshProUGUI _itemNameText;
    private TextMeshProUGUI _itemPriceText;

    private TextMeshProUGUI[] _textMeshProUGUIs;


    private void Awake()
    {
        _textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
        _itemNameText = _textMeshProUGUIs[0];
        _itemPriceText = _textMeshProUGUIs[1];

        _defaultColor = new Color(1f, 1f, 1f);
        _selectedColor = new Color(0.8f, 0.8f, 0.8f);
    }

    public void SetShopStock(ItemSO p_itemSO, ShopSystem p_shopSystem)
    {
        _shopSystem = p_shopSystem;

        _panel.color = _defaultColor;
        _itemSO = p_itemSO;
        _itemIcon.sprite = _itemSO.itemSprite;
        _itemNameText.text = _itemSO.itemName;
        _itemPriceText.text = $"{_itemSO.buyPrice.ToString()} G";
    }

    public void ReleaseStock()
    {
        gameObject.SetActive(false);
    }

    public void SetBuyItem()
    {
        if (Managers.Data.gold < _itemSO.buyPrice) return;

        _shopSystem.SelectItem(_slotIndex);
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }
}
