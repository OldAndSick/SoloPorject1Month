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
        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� �׽�Ʈ�� ��
#else
            Application.Quit(); // ���� ���ӿ��� ������ ��
#endif
    }
}
