using System;
using System.Collections.Generic;


public static class EnumsToStringEx
{
    private static readonly Dictionary<Enum, string> enumToStringDic = new Dictionary<Enum, string>();

    
    /// <summary>
    /// enum을 string으로 변환 함수, CollectionUI.Crops.EnumToString();
    /// </summary>
    public static string EnumToString<T>(this T p_Type) where T : Enum
    {
        if (!enumToStringDic.TryGetValue(p_Type, out string value))
        {
            value = p_Type.ToString();
            enumToStringDic[p_Type] = value;
        }

        return value;
    }
}