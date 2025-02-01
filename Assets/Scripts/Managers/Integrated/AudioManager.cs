using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour, IInit
{
    private AudioMixer _audioMixer;
    private AudioListSO _audioList;
    
    private Dictionary<int, AudioClip> _audioClipsDic;
    
    [SerializeField] private AudioSource _musicPlayer = null;
    [SerializeField] private AudioSource _footStepPlayer = null;
    [SerializeField] private AudioSource[] _sfxPlayer = null;
    [SerializeField] private AudioSource[] _ambientPlayer = null;

    
    public void Init()
    {
        _audioMixer = Resources.Load<AudioMixer>("Audio/MainMixer");
        _audioList = Resources.Load<AudioListSO>("Audio/AudioListSO");
        AddAudioListDic();
    }
    
    
    private void Start()
    {
        _audioMixer.SetFloat("Master", Mathf.Log10(Managers.Data.masterVolume) *20);
        _audioMixer.SetFloat("Music", Mathf.Log10(Managers.Data.musicVolume) *20);
        _audioMixer.SetFloat("SFX", Mathf.Log10(Managers.Data.sfxVolume) *20);
        _audioMixer.SetFloat("Ambient", Mathf.Log10(Managers.Data.ambientVolume) *20);
        _audioMixer.SetFloat("Footstep", Mathf.Log10(Managers.Data.footstepVolume) *20);
    }


# region 소리 재생
    
    public void PlayMusic(MusicEnums p_Name)
    {
        if (_audioClipsDic.TryGetValue((int)p_Name, out AudioClip clip))
        {
            _musicPlayer.clip = clip;
            _musicPlayer.Play();
        }
    }
    

    public void PlaySfx(SfxEnums p_Name)
    {
        if (_audioClipsDic.TryGetValue((int)p_Name, out AudioClip clip))
        {
            for (int j = 0; j < _sfxPlayer.Length; j++)
            {
                if (!_sfxPlayer[j].isPlaying)
                {
                    _sfxPlayer[j].clip = clip;
                    _sfxPlayer[j].pitch = Random.Range(Setting.MinRandomPitch, Setting.MaxRandomPitch); 
                    _sfxPlayer[j].Play();
                    return;
                }
            }

            // Debug.Log("모든 플레이어가 재생중");
        }
    }
    

    public void PlayAmbient(AmbientEnums p_Name)
    {
        if (_audioClipsDic.TryGetValue((int)p_Name, out AudioClip clip))
        {
            for (int j = 0; j < _ambientPlayer.Length; j++)
            {
                if (!_ambientPlayer[j].isPlaying)
                {
                    _ambientPlayer[j].clip = clip;
                    _ambientPlayer[j].pitch = Random.Range(Setting.MinRandomPitch, Setting.MaxRandomPitch); 
                    _ambientPlayer[j].Play();
                    return;
                }
            }

            // Debug.Log("모든 플레이어가 재생중");
        }
    }
    

    public void PlayFootStep(FootStep p_Name)
    {
        if (_audioClipsDic.TryGetValue((int)p_Name, out AudioClip clip))
        {
            _footStepPlayer.clip = clip;
            _footStepPlayer.Play();
        }
    }


    public void PlayRandomMusic()
    {
        int number = Random.Range((int)MusicEnums.InGame2, (int)MusicEnums.InGame2);
        
        PlayMusic((MusicEnums)number);
    }
    
# endregion
    

# region 음량 조작

    public void MuteAllVolume(bool p_Flag)
    {
        if (p_Flag)
        {
            ChangeVolume(AudioTypeEnum.Master, 0.001f);
            
            return;
        }
        
        ChangeVolume(AudioTypeEnum.Master, Managers.Data.masterVolume);
    }
    
    
    public void ChangeVolume(AudioTypeEnum p_Type, float p_Volume)
    {
        float setVolume = Mathf.Clamp(p_Volume, 0.001f, 1f); 
        _audioMixer.SetFloat(p_Type.EnumToString(), Mathf.Log10(setVolume) * 20);
    }


    /// <summary>
    /// UI를 킬때 배경음악과 환경음을 줄여주는 역할
    /// </summary>
    public void OnFadeInAudio()
    {
        float fadeInTime = 1f;
        
        _audioMixer.DOSetFloat("Master", Mathf.Log10(0.1f) * 20, fadeInTime)
            .OnComplete(() => ControlMusicPlayer(MusicPlayerOption.Stop));
    }
    
    
    /// <summary>
    /// UI를 끌때 배경음악과 환경음을 복귀시키는 역할
    /// </summary>
    public void OnFadeOutAudio()
    {
        int fadeOutTime = 2;
        
        _audioMixer.DOSetFloat("Master", Mathf.Log10(Managers.Data.musicVolume) * 20, fadeOutTime)
            .OnComplete(PlayRandomMusic);
    }
    
    
    public void ControlMusicPlayer(MusicPlayerOption p_ControlType)
    {
        switch (p_ControlType)
        {
            case MusicPlayerOption.Pause:
                _musicPlayer.Pause();
                break;
            case MusicPlayerOption.UnPause:
                _musicPlayer.UnPause();
                break;
            case MusicPlayerOption.Stop:
                _musicPlayer.Stop();
                break;
            case MusicPlayerOption.Mute:
                _musicPlayer.mute = true;
                break;
            case MusicPlayerOption.UnMute:
                _musicPlayer.mute = false;
                break;
        }
    }
    
# endregion
    
    
    private void AddAudioListDic()
    {
        _audioClipsDic = new Dictionary<int, AudioClip>();
        
        foreach (var item in _audioList.musicSounds)
        {
            _audioClipsDic.Add((int)item.name, item.clip);
        }

        foreach (var item in _audioList.sfxSounds)
        {
            _audioClipsDic.Add((int)item.name, item.clip);
        }

        foreach (var item in _audioList.footStepSounds)
        {
            _audioClipsDic.Add((int)item.name, item.clip);
        }
        
        Resources.UnloadAsset(_audioList);
    }
}