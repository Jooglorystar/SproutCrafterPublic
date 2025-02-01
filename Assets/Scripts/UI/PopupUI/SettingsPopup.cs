using UnityEngine;
using UnityEngine.UI;


public class SettingsPopup : PopupUI
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _footStepSlider;
    [SerializeField] private Toggle _muteToggle;


    private void OnEnable()
    {
        _masterSlider.value = Managers.Data.masterVolume;
        _musicSlider.value = Managers.Data.musicVolume;
        _sfxSlider.value = Managers.Data.sfxVolume;
        _ambientSlider.value = Managers.Data.ambientVolume;
        _footStepSlider.value = Managers.Data.footstepVolume;

        _muteToggle.isOn = Managers.Data.isMute;
    }


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
        Managers.Sound.ChangeVolume(AudioTypeEnum.Footstep, _ambientSlider.value);
        Managers.Data.footstepVolume = _footStepSlider.value;
    }


    public void OnChangeMuteToggle(bool p_Flag)
    {
        Managers.Sound.MuteAllVolume(p_Flag);
        Managers.Data.isMute = p_Flag;
    }
}