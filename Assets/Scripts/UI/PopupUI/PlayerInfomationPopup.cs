using TMPro;
using UnityEngine;


public class PlayerInfomationPopup : PopupUI
{
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _calenderText;
    [SerializeField] private TextMeshProUGUI _playerLevelText;
    [SerializeField] private TextMeshProUGUI _playerGoldText;
    
    
    public override void Init()
    {
        base.Init();
    }


    private void OnEnable()
    {
        ReFreshUI();
    }

    private void ReFreshUI()
    {
        _playerNameText.SetText($"{Managers.Data.playerName} 의 농장");
        _calenderText.SetText($"{GameManager.Instance.TimeM.InGameYear} 년차, {GameManager.Instance.TimeM.InGameDay} 일");
        _playerLevelText.text = $"레벨 : {Managers.Data.currentLevel}";
        _playerGoldText.text = $"소지금 : {Managers.Data.gold:N0}";
        
        
        // _playerName.text = $"{Managers.Data.playerName} <font=\"PF_StarDust_B\">의 농장</font>";
        // _calender.text = $"<font=\"PF_StarDust_B\">999</font>년차 <font=\"PF_StarDust_B\">겨울</font> <font=\"PF_StarDust_B\">31</font> 일";
        // _playerLevel.text = $"레벨 : <font=\"PF_StarDust_\">99</font>";
    }
}