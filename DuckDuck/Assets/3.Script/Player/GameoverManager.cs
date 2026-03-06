using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("게임 오버 UI")]
    public GameObject gameOverPanel; // "죽었습니다" 화면 전체 묶음

    // 플레이어가 죽었을 때 부를 함수!!
    public void ShowGameOver()
    {
        Debug.Log("플레이어 사망... 저승문 열립니다.");
        gameOverPanel.SetActive(true); // 죽음 화면 켜기
        Time.timeScale = 0f;           // [띠또 마법 ⭐] 게임 시간 정지! (적들 멈춤)
    }

    // [처음으로] 버튼용 함수
    public void RestartGame()
    {
        Time.timeScale = 1f; // [초핵심!!] 멈췄던 시간을 다시 흐르게 고침!
        SceneManager.LoadScene("1Stage"); // 1스테이지 씬 이름 적어주세요!
    }

    // [타이틀로] 버튼용 함수
    public void GoToTitle()
    {
        Time.timeScale = 1f; // 시간 복구!
        SceneManager.LoadScene("TitleScene"); // 타이틀 씬 이름 적어주세요!
    }
}