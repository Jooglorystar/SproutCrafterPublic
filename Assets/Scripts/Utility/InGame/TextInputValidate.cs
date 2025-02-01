using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextInputValidate : MonoBehaviour
{
    [SerializeField] private TMP_InputField _playerNameInputField = null;
    

    /// <summary>
    /// 플레이어 이름을 받는데 범위를 제한하는 역할 
    /// </summary>
    public void OnEndTextValueChanged()
    {
        string str = _playerNameInputField.text;

        char[] chars = str.Replace(" ", "").ToCharArray();

        List<char> charList = new List<char>();

        foreach (char item in chars)
        {
            if (char.IsLetterOrDigit(item) && item < 128)
            {
                Managers.Sound.PlaySfx(SfxEnums.Texting);
                charList.Add(item);
            }
            else
            {
                Managers.Sound.PlaySfx(SfxEnums.ButtonClickNegative);
                charList.Clear();
                _playerNameInputField.text = string.Empty;
            }
        }

        string result = new string(charList.ToArray());

        _playerNameInputField.text = result;
    }
}