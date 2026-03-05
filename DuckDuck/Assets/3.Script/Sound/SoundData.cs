using UnityEngine;
[System.Serializable]
public class SoundData 
{
    public SoundList soundType;
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1f;
}
