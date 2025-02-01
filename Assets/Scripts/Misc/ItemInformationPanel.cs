using TMPro;
using UnityEngine;

public class ItemInformationPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _itemPriceText;

    public void OnEnable()
    {
        ResetText();
    }

    /// <summary>
    /// 선택한 아이템의 정보를 표시하는 메서드
    /// </summary>
    /// <param name="p_itemSO"></param>
    /// <param name="p_selling">true시 판매시 가격 출력, false시 구매시 가격 출력</param>
    public void SetText(ItemSO p_itemSO, bool p_selling)
    {
        _itemNameText.text = p_itemSO.itemName;
        _itemDescriptionText.text = p_itemSO.itemDescription;
        _itemTypeText.text = p_itemSO.itemType.ToString();
        _itemPriceText.text = p_selling ? ((int)(p_itemSO.sellPrice * GameManager.Instance.DynamicPricingSystem.GetSellPriceModifier(p_itemSO))).ToString() : p_itemSO.buyPrice.ToString();
    }

    public void ResetText()
    {
        _itemNameText.text = string.Empty;
        _itemDescriptionText.text = string.Empty;
        _itemTypeText.text = string.Empty;
        _itemPriceText.text = string.Empty;
    }
}
