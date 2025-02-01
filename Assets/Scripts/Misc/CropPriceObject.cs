using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CropPriceObject : MonoBehaviour
{
    private Image _cropIcon;
    private TextMeshProUGUI _cropPriceText;

    private void Awake()
    {
        _cropIcon = GetComponentInChildren<Image>();
        _cropPriceText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetInfromation(CropsSO p_cropSO)
    {
        _cropIcon.sprite = p_cropSO.itemSprite;
        _cropPriceText.text = ((int)(p_cropSO.sellPrice * GameManager.Instance.DynamicPricingSystem.GetSellPriceModifier(p_cropSO))).ToString();
    }
}