using UnityEngine;

public class MalTrigger : MonoBehaviour
{
    [Header("설정")]
    [TextArea]
    public string message; //오브젝ㅌㅡㄱㅏ 등ㅈㅏㅇ할ㄸㅐ 펫ㅇㅣ 할 대사
    public float delay = 0.2f; // 등장 후 몇 초 뒤에 말할지

    private static BubblePet _bubble;

    private void Start()
    {
        if(_bubble==null)
        {
            _bubble = FindAnyObjectByType<BubblePet>();
        }
        if (_bubble != null && !string.IsNullOrEmpty(message))
        {
            Invoke("ExecuteSpeech", delay);
        }
    }

    void ExecuteSpeech()
    {
        _bubble.ShowMessage(message);
        this.enabled = false;
    }
}
