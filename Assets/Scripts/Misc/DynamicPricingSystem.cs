using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropPriceData
{
    public int cropCode;
    public int cropStockQuantity;
    public float priceModifier;

    private int _defaultStockQuantity = 100;
    private float _defaultPriceModifier = 1f;

    private int _minStockQuantity;
    private int _maxStockQuantity;

    public CropPriceData(CropsSO cropsSO)
    {
        cropCode = cropsSO.itemCode;
        cropStockQuantity = _defaultStockQuantity;
        priceModifier = _defaultPriceModifier;

        _minStockQuantity = cropStockQuantity / 2;
        _maxStockQuantity = cropStockQuantity * 2;
    }

    public void AddStockQuantity(int p_value)
    {
        cropStockQuantity += p_value;
        cropStockQuantity = Mathf.Clamp(cropStockQuantity, _minStockQuantity, _maxStockQuantity);
    }

    public float SetPriceModifier()
    {
        float modifier = ((float)_defaultStockQuantity / (float)cropStockQuantity);
        return Mathf.Clamp(modifier, 0.5f, 2.0f);
    }
}

public class DynamicPricingSystem
{
    /// <summary>
    /// Key: 작물, Value: 가격변동 변수
    /// </summary>
    private Dictionary<CropsSO, CropPriceData> _cropPriceDictionary = new Dictionary<CropsSO, CropPriceData>();

    public DynamicPricingSystem(ItemDatabase p_itemDatabase)
    {
        List<ItemSO> itemSOs = p_itemDatabase.GetAllEntries();

        for (int i = 0; i < itemSOs.Count; i++)
        {
            if (itemSOs[i] is CropsSO crop)
            {
                _cropPriceDictionary.Add(crop, new CropPriceData(crop));
            }
        }
    }

    public List<CropPriceData> SavePriceDatas()
    {
        List<CropPriceData> cropPriceDatas = new List<CropPriceData>();

        foreach (var (crop, price) in _cropPriceDictionary)
        {
            cropPriceDatas.Add(price);
        }

        return cropPriceDatas;
    }

    public void LoadPriceDatas(List<CropPriceData> p_cropPriceData)
    {
        for (int i = 0; i < p_cropPriceData.Count; i++)
        {
            if (ItemCodeMapper.GetItemSo(p_cropPriceData[i].cropCode) is CropsSO cropsSO && cropsSO != null)
            {
                LoadPriceData(cropsSO, p_cropPriceData[i]);
            }
        }
    }

    private void LoadPriceData(CropsSO p_cropsSO, CropPriceData p_cropPriceData)
    {
        _cropPriceDictionary[p_cropsSO].cropCode = p_cropPriceData.cropCode;
        _cropPriceDictionary[p_cropsSO].cropStockQuantity = p_cropPriceData.cropStockQuantity;
        _cropPriceDictionary[p_cropsSO].priceModifier = p_cropPriceData.priceModifier;
    }

    private void ModifyStocks()
    {
        foreach (var (crop, value) in _cropPriceDictionary)
        {
            ModifyStock(value);
        }
    }

    // 값을 조정하는 더 좋은 방법이 있을지도 모름
    private void ModifyStock(CropPriceData p_cropData)
    {
        if (p_cropData.priceModifier > 1f)
        {
            p_cropData.AddStockQuantity(Random.Range(-5, 10));
        }
        else if (p_cropData.priceModifier <= 1f)
        {
            p_cropData.AddStockQuantity(Random.Range(-5, 5));
        }
    }

    public void UpdatePrices()
    {
        ModifyStocks();
        foreach (var (crop, value) in _cropPriceDictionary)
        {
            value.priceModifier = value.SetPriceModifier();
        }
    }

    public void SellCrop(CropsSO p_cropSO, int p_amount)
    {
        if (!_cropPriceDictionary.ContainsKey(p_cropSO)) return;

        _cropPriceDictionary[p_cropSO].AddStockQuantity(p_amount);
    }

    public float GetSellPriceModifier(ItemSO p_itemSO)
    {
        if (p_itemSO is CropsSO crop)
        {
            return _cropPriceDictionary[crop].priceModifier;

        }
        return 1;
    }

}