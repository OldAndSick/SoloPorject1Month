using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionPanel;

    [Header("진엔딩 타이틀 씬 이름")]
    public string clearedTitleSceneName = "Title2"; // 새로 파신 씬 이름 적어주세요!
    [Header("디버그/테스트용 (체크하면 기록 초기화!)")]
    public bool resetClearData = false;

    private void Awake()
    {
        // 씬이 켜지자마자 가장 먼저 실행되는 곳!
        // 만약 주인님이 인스펙터에서 리셋 버튼을 체크해뒀다면?!
        if (resetClearData)
        {
            PlayerPrefs.SetInt("GameCleared", 0); // 도장을 0(초기화)으로 바꿔버림!
            PlayerPrefs.Save();
            Debug.Log("클리어 기록이 초기화되었습니다");
        }
    }
    private void Start()
    {
        if (PlayerPrefs.GetInt("GameCleared", 0) == 1)
        {
            // 현재 열려있는 씬이 진엔딩 씬이 '아닐 때만' 넘어간다!!
            if (SceneManager.GetActiveScene().name != clearedTitleSceneName)
            {
                SceneManager.LoadScene(clearedTitleSceneName);
            }
        }
        // 클리어 안 한 유저면 이 코드는 무시되고 그냥 원래 타이틀 화면이 뜹니다.
    }

    public void StartGame()
    {
        SceneManager.LoadScene("1Stage"); // 게임 시작 버튼 누르면 1스테이지로!
    }

    public void ToggleOp()
    {
        if (optionPanel != null)
        {
            bool isActive = optionPanel.activeSelf;
            optionPanel.SetActive(!isActive);
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}