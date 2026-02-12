using UnityEngine;

public class MalTrigger : MonoBehaviour
{
    [Header("설정")]
    [TextArea]
    public string message; //오브젝ㅌㅡㄱㅏ 등ㅈㅏㅇ할ㄸㅐ 펫ㅇㅣ 할 대사
    public float delay = 0.2f; // 등장 후 몇 초 뒤에 말할지

    private void Start()
    {
        if (!string.IsNullOrEmpty(message))
        {
            Invoke("ExecuteSpeech", delay);
        }
    }
    private void ExecuteSpeech()
    {
        if (BubblePet.Instance != null)
        {
            BubblePet.Instance.ShowMessage(message);
        }
        else
        {
            Debug.LogWarning("No BubblePet Manager");
        }
        this.enabled = false;
    }
}
