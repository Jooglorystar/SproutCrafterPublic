

public static class ItemCodeMapper
{
    /// <summary>
    /// ItemSo를 이용하여 itemCode를 반환
    /// </summary>
    public static int GetItemCode(this ItemSO p_ItemSo)
    {
        return p_ItemSo.itemCode;
    }


    /// <summary>
    /// 아이템 코드를 이용하여 ItemSo를 반환
    /// </summary>
    public static ItemSO GetItemSo(this int p_ItemCode)
    {
        return GameManager.Instance.DataBaseM.ItemDatabase.GetByID(p_ItemCode);
    }
}