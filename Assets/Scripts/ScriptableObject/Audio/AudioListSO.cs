using UnityEngine;


[CreateAssetMenu(menuName = "Audio/AudioListSO", fileName = "AudioListSO")]
public class AudioListSO : ScriptableObject
{
    public int fadeTime = 1;
    
    public MusicSound[] musicSounds;
    public SfxSound[] sfxSounds;
    public FootstepSound[] footStepSounds;
    public AmbientSound[] ambientSounds;
}