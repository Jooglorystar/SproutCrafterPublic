using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TitleOption : PopupUI
{
    [Header("음량 조절")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _footStepSlider;
    
    [SerializeField] private Toggle _isMuteToggle;
    
    
    [Header("화면 조절")]
    [SerializeField] private Toggle _isFullScreenToggle;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    
    private List<Resolution> _resolutions;
    
    private int _resolutionIndex;
    private bool _isFullScreen;


    public override void Init()
    {
        base.Init();
        
        SetDropdownScreenResolutionList();
        ShowResolutionDropdownList();
    }


    private void OnEnable()
    {
        SetSystemDataValue();
    }


    public override void Close()
    {
        base.Close();
        Managers.Sound.PlaySfx(SfxEnums.ButtonClickPositive);
    }


    private void SetSystemDataValue()
    {
        OnChangeMuteToggle(Managers.Data.isMute);
        
        _masterSlider.value = Managers.Data.masterVolume;
        _musicSlider.value = Managers.Data.musicVolume;
        _sfxSlider.value = Managers.Data.sfxVolume;
        _ambientSlider.value = Managers.Data.ambientVolume;
        _footStepSlider.value = Managers.Data.footstepVolume;
        
        _isMuteToggle.isOn = Managers.Data.isMute;

        _resolutionIndex = Managers.Data.screenResolutionIndex;
        _isFullScreen = Managers.Data.isFullScreen;
        
        _resolutionDropdown.value = _resolutionIndex;
        _isFullScreenToggle.isOn = _isFullScreen;
        
        _resolutionDropdown.RefreshShownValue();
    }
    

# region 해상도 조절
    
    /// <summary>
    /// 사용 가능한 스크린 사이즈를 확인해서 리스트로 만들어줌
    /// </summary>
    private void SetDropdownScreenResolutionList()
    {
        _resolutions = new List<Resolution>(Screen.resolutions);
        _resolutions.Reverse();

        if (_resolutions.Count > 0)
        {
            List<Resolution> tempResolutions = new List<Resolution>();
            
            int currentWidth = _resolutions[0].width;
            int currentHeight = _resolutions[0].height;
            
            tempResolutions.Add(_resolutions[0]);

            foreach (Resolution item in _resolutions)
            {
                if (currentWidth != item.width || currentHeight != item.height)
                {
                    tempResolutions.Add(item);
                    
                    currentWidth = item.width;
                    currentHeight = item.height;
                }
            }
            
            _resolutions = tempResolutions;
        }
    }


    /// <summary>
    /// 만들어진 리스트를 실제로 Dropdown에 보여줌
    /// </summary>
    private void ShowResolutionDropdownList()
    {
        List<string> options = new List<string>();

        foreach (Resolution item in _resolutions)
        {
            string option = $"{item.width} x {item.height}";
            
            options.Add(option);
        }
        
        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(options);
        
        _resolutionDropdown.value = Managers.Data.screenResolutionIndex;
        _isFullScreenToggle.isOn = Managers.Data.isFullScreen;
        
        _resolutionDropdown.RefreshShownValue();
    }


    /// <summary>
    /// 특정 항목을 선택하여 실제 해상도를 변경시켜줌
    /// </summary>
    /// <param name="p_Index"></param>
    public void DropdownOptionChanged(int p_Index)
    {
        if (_resolutions == null) return;
        
        Resolution resolution = _resolutions[p_Index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        
        Managers.Data.screenResolutionIndex = p_Index;
    }


    /// <summary>
    /// 전체화면 여부를 변경시켜줌
    /// </summary>
    /// <param name="p_Value"></param>
    public void FullScreenOptionChanged(bool p_Value)
    {
        Screen.fullScreen = p_Value;
        
        Managers.Data.isFullScreen = p_Value;
    }
    
# endregion
    

# region 사운드 조절
    
    public void OnChangeMasterSoundVolume()
    {
        Managers.Sound.ChangeVolume(AudioTypeEnum.Master, _masterSlider.value);
        Managers.Data.masterVolume = _masterSlider.value;
    }
    
    
    public void OnChangeMusicSoundVolume()
    {
        Managers.Sound.ChangeVolume(AudioTypeEnum.Music, _musicSlider.value);
        Managers.Data.musicVolume = _musicSlider.value;
    }
    
    
    public void OnChangeSFXSoundVolume()
    {
        Managers.Sound.ChangeVolume(AudioTypeEnum.SFX, _sfxSlider.value);
        Managers.Data.sfxVolume = _sfxSlider.value;
    }
    
    
    public void OnChangeAmbientSoundVolume()
    {
        Managers.Sound.ChangeVolume(AudioTypeEnum.Ambient, _ambientSlider.value);
        Managers.Data.ambientVolume = _ambientSlider.value;
    }
    
    
    public void OnChangeFootstepSoundVolume()
    {
        Managers.Sound.ChangeVolume(AudioTypeEnum.Footstep, _footStepSlider.value);
        Managers.Data.footstepVolume = _footStepSlider.value;
    }
    
    
    public void OnChangeMuteToggle(bool p_Flag)
    {
        Managers.Sound.MuteAllVolume(p_Flag);
        Managers.Data.isMute = p_Flag;
    }
    
# endregion
    
}