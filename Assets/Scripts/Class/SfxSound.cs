using UnityEngine;


[System.Serializable]
public class SfxSound
{
    public SfxEnums name;
    public AudioClip clip;
    public string description;
    
    public float minRandomPitch = 0.9f;
    public float maxRandomPitch = 1.1f;
}