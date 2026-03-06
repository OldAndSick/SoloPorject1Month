using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class TitleVolumeManager : MonoBehaviour
{
    [Header("오디오 믹서 & 슬라이더")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // 1. 저장된 볼륨값 불러와서 슬라이더 위치 맞추기
        if (masterSlider != null) masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 1f);
        if (bgmSlider != null) bgmSlider.value = PlayerPrefs.GetFloat("BGMVol", 1f);
        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);

        // 2. [⭐핵심] 타이틀 씬이 켜지자마자 오디오 믹서에 볼륨값 강제 주입!
        // (안 그러면 믹서는 기본값으로 틀어져 나옵니다)
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVol", 1f));
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVol", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVol", 1f));
    }

    public void SetMasterVolume(float volume)
    {
        // 소리가 완전히 꺼지는 오류 방지 (Clamp로 최소값 0.0001 보장)
        float val = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("MasterVol", Mathf.Log10(val) * 20);
        PlayerPrefs.SetFloat("MasterVol", val);
    }

    public void SetBGMVolume(float volume)
    {
        float val = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("BGMVol", Mathf.Log10(val) * 20);
        PlayerPrefs.SetFloat("BGMVol", val);
    }

    public void SetSFXVolume(float volume)
    {
        float val = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("SFXVol", Mathf.Log10(val) * 20);
        PlayerPrefs.SetFloat("SFXVol", val);
    }
}