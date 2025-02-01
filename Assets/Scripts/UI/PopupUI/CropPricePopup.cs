using System.Collections.Generic;
using UnityEngine;

public class CropPricePopup : PopupUI
{
    [SerializeField] private CropPriceObject _object;
    [SerializeField] private Transform _container;

    private List<CropPriceObject> _cropPrices = new List<CropPriceObject>();

    private void Start()
    {
        SetCropPrices(GameManager.Instance.DataBaseM.ItemDatabase);
    }

    private void SetCropPrices(ItemDatabase p_itemDatabase)
    {
        List<ItemSO> itemSOs = p_itemDatabase.GetAllEntries();

        for (int i = 0; i < itemSOs.Count; i++)
        {
            if (itemSOs[i] is CropsSO crop)
            {
                CropPriceObject test = Instantiate(_object.gameObject, _container).GetComponent<CropPriceObject>();
                test.SetInfromation(crop);
                _cropPrices.Add(test);
            }
        }
    }
}