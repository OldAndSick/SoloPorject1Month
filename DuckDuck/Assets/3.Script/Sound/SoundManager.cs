using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public List<SoundData> soundDatabase = new List<SoundData>();

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundList type)
    {
        SoundData data = soundDatabase.Find(s => s.soundType == type);

        if(data!=null&&data.clip!=null)
        {
            audioSource.PlayOneShot(data.clip, data.volume);
        }
        else
        {
            Debug.LogWarning(type + "사운드 데이터가 비어있슴..");
        }
    }
}
