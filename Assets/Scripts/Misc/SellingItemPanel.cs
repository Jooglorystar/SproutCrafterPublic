using TMPro;
using UnityEngine;

public class SellingItemPanel : ShopItemPanel
{
    [SerializeField] private TextMeshProUGUI _itemCountText;

    private void Awake()
    {
        _itemCountText = GetComponentInChildren<TextMeshProUGUI>();

        _defaultColor = new Color(1f, 1f, 1f);
        _selectedColor = new Color(0.8f, 0.8f, 0.8f);
    }

    public void SetSellingItem(int p_index, ShopSystem p_shopSystem)
    {
        _shopSystem = p_shopSystem;
        _itemSO = Managers.Data.inventorySlots[p_index].itemSo;

        if (_itemSO != null)
        {
            _itemIcon.gameObject.SetActive(true);
            _itemIcon.sprite = _itemSO.itemSprite;
            _itemCountText.text = SetItemCountText(Managers.Data.inventorySlots[p_index].itemCount);
        }
        else
        {
            _itemCountText.text = string.Empty;
            _itemIcon.gameObject.SetActive(false);
        }
    }

    public void SetSellItem()
    {
        _shopSystem.SelectItem(_slotIndex);
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }

    private string SetItemCountText(int p_count)
    {
        if(p_count > 1)
        {
            return p_count.ToString();
        }
        return string.Empty;
    }
}