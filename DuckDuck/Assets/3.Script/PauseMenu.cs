using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI; // 일시정지 창 (설정창)

    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private bool isPaused = false;

    void Start()
    {
        // 슬라이더 기본값 세팅 (0.0001 ~ 1 사이로 슬라이더 세팅 필요!)
        masterSlider.value = PlayerPrefs.GetFloat("MasterVol", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVol", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1f);
    }

    void Update()
    {
        // ESC 키로 일시정지 켜고 끄기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 시간 정상화
        isPaused = false;

        // 마우스 커서 숨기기 (다시 겜 시작)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // 시간 정지!
        isPaused = true;

        // 마우스 커서 보이게 하기 (설정창 눌러야 하니까)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVol", volume); // 저장
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGMVol", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVol", volume);
    }
    public void GoToTitle()
    {
        Time.timeScale = 1f;

        // "TitleScene" 부분은 주인님의 실제 타이틀 씬 이름과 토씨 하나 안 틀리고 똑같이 적으셔야 합니다!
        SceneManager.LoadScene("Title1");
    }

    public void QuitGame()
    {
        // 유니티 에디터 창에서는 Application.Quit()이 안 먹히기 때문에, 확인용 로그를 띄웁니다!
        Debug.Log("게임을 종료합니다! (에디터에선 안 꺼지지만 빌드하면 꺼짐!)");

        // 실제 게임을 빌드(exe)했을 때 프로그램 자체를 꺼버리는 마법의 코드!
        Application.Quit();
    }
}