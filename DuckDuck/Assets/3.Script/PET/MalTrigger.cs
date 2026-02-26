using UnityEngine;

public class MalTrigger : MonoBehaviour
{
    [Header("����")]
    [TextArea]
    public string message; //���������Ѥ��� ������Ҥ��� �ꤷ�� �� ���
    public float delay = 0.2f; // ���� �� �� �� �ڿ� ������

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
