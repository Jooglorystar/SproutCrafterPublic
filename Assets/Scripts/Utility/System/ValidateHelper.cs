using System.Collections;
using UnityEngine;


public static class ValidateHelper
{
    /// <summary>
    /// 아이템 SO의 데이터 무결성 검증
    /// </summary>
    /// <param name="p_Object">검사하고자하는 SO 자체, this 키워드 이용</param>
    /// <param name="p_FieldName">검사하고자하는 변수</param>
    /// <param name="p_codeToCheck">p_FieldName와 같은 변수 넣기</param>
    /// <returns></returns>
    public static bool ValidateCheckItemCode(Object p_Object, string p_FieldName, int p_codeToCheck)
    {
        if (p_codeToCheck == 0)
        {
            Debug.Log($"{p_FieldName} 변수가 비어있고 반드시 작성되어야 합니다. Assets 이름은 {p_Object.name.ToString()}입니다");
            return true;
        }
        
        return false;
    }


    /// <summary>
    /// 열거 가능한 객체 (순회 가능한)의 데이터 무결성 검증
    /// </summary>
    /// <param name="p_Object">검사하고자하는 SO 자체, this 키워드 이용</param>
    /// <param name="p_FieldName">검사하고자하는 배열, 리스트 등</param>
    /// <param name="p_EnumerableObject">p_FieldName와 같은 변수 넣기</param>
    /// <returns></returns>
    public static bool ValidateCheckEnumerableObject(Object p_Object, string p_FieldName, IEnumerable p_EnumerableObject)
    {
        bool error = false;
        int count = 0;
        
        if (p_EnumerableObject == null)
        {
            Debug.Log(p_FieldName + " 가 NULL 입니다, 에셋 이름은 " + p_Object.name.ToString() + " 입니다");
            return true;
        }

        foreach (var item in p_EnumerableObject)
        {

            if (item == null)
            {
                Debug.Log(p_FieldName + " 요소중 NUll인 요소가 존재합니다, 에셋 이름은 " + p_Object.name.ToString() + " 입니다");
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(p_FieldName + "이(가) 초기화 되었지만 요소가 없습니다, 에셋 이름은 " + p_Object.name.ToString() + " 입니다");
            error = true;
        }

        return error;
    }
}