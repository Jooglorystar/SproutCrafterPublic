using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GameDataSlot : MonoBehaviour
{
    [Header("슬롯 데이터")]
    [SerializeField] private int _slotNumber;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _slotStateText;
    
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private TextMeshProUGUI _playerGoldText;
    [SerializeField] private TextMeshProUGUI _playerLevelText;

    [SerializeField] private Image _sceneFader;

    [Header("데이터 슬롯 생성")]
    [SerializeField] private GameObject _PlayerNameInputPanel;
    [SerializeField] private TMP_InputField _playerNameInputField;


    /// <summary>
    /// 데이터 여부에 따라 표시해야할 정보를 다르게 함
    /// </summary>
    private void OnEnable()
    {
        SaveManager.Instance.SaveSlot = _slotNumber;
        
        if (Directory.Exists(SaveManager.Instance.SaveSlotPath))
        {
            _slotStateText.gameObject.SetActive(false);
            SaveManager.Instance.LoadTitleInfo(_slotNumber);
            
            _playerNameText.gameObject.SetActive(true);
            _playerGoldText.gameObject.SetActive(true);
            _playerLevelText.gameObject.SetActive(true);
            
            _playerNameText.text = Managers.Data.playerName;
            _playerGoldText.text = Managers.Data.gold.ToString();
            _playerLevelText.text = Managers.Data.currentLevel.ToString();
        }
        else
        {
            _slotStateText.gameObject.SetActive(true);
            _slotStateText.text = "새로 시작하기";
        }
    }


#region 버튼 기능

    /// <summary>
    /// 데이터가 있으면 바로 시작하고 없으면 플레이어 이름 인풋 패널을 띄워주는 역할
    /// </summary>
    public void OnClickPlayGameButton()
    {
        SaveManager.Instance.SaveSlot = _slotNumber;
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);

        if (Directory.Exists(SaveManager.Instance.SaveSlotPath))
        {
            FadeSceneAndGameStart(_slotNumber);
        }
        else
        {
            _slotStateText.gameObject.SetActive(false);
            _PlayerNameInputPanel.gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// 이름 입력 완료 후 게임 시작 버튼 클릭시 인게임 씬으로 넘어가는 역할
    /// </summary>
    public void OnClickFirstGamePlayButton()
    {
        if (_playerNameInputField.text.Length > 0)
        {        
            SaveManager.Instance.SaveSlot = _slotNumber;
        
            FadeSceneAndGameStart(_slotNumber);

            Managers.Data.playerName = _playerNameInputField.text;
        }
    }

    
    public void OnClickClearSlotDta()
    {
        SaveManager.Instance.SaveSlot = _slotNumber;
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
        
        if (Directory.Exists(SaveManager.Instance.SaveSlotPath))
        {
            Directory.Delete(SaveManager.Instance.SaveSlotPath, true);
            
            _playerNameText.gameObject.SetActive(false);
            _playerGoldText.gameObject.SetActive(false);
            _playerLevelText.gameObject.SetActive(false);
            
            _slotStateText.gameObject.SetActive(true);
            _slotStateText.text = "새로 시작하기";
        }
    }

#endregion


    private void FadeSceneAndGameStart(int p_SlotNumber)
    {
        _sceneFader.gameObject.SetActive(true);
        _sceneFader.DOFade(1f,1f).OnComplete(()=> SaveManager.Instance.SelectSaveSlotNumber(p_SlotNumber));
        Managers.Sound.OnFadeInAudio();
    }


    /// <summary>
    /// 설정을 끝마치기 전에 끄면 초기화 시켜주는 역할
    /// </summary>
    private void OnDisable()
    {
        _slotStateText.gameObject.SetActive(false);
        _PlayerNameInputPanel.gameObject.SetActive(false);
    }
}