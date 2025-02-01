using UnityEngine;


[System.Serializable]
public class AmbientSound
{
    public AmbientEnums name;
    public AudioClip clip;
    public string description;
    
    public float minRandomPitch = 0.9f;
    public float maxRandomPitch = 1.1f;
}