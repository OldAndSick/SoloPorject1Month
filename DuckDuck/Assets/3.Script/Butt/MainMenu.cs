using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public GameObject optionPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("1Stage");
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
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 테스트할 때
#else
            Application.Quit(); // 빌드된 게임에서 실행할 때
#endif
    }
}
