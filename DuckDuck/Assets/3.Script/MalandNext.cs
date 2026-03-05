using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro�� ����ϴ� ���

public class GameIntroManager : MonoBehaviour
{
    [Header("UI ����")]
    public Text descriptionText; // ���� �ؽ�Ʈ�� ǥ���� TMP ������Ʈ

    [Header("���� ����")]
    [TextArea(1, 3)]
    // ���⿡ ���� ���� �ؽ�Ʈ�� �� �پ� �Է��ϼ���.
    public string[] introTexts;
    public string nextSceneName = "GameScene";


    private int currentTextIndex = 0;
    private bool isTyping = false; // ���� �ؽ�Ʈ ��� ������ Ȯ��
    private bool isFullyDisplayed = false; // ���� �ؽ�Ʈ�� ��� ��µǾ����� Ȯ��

    public float typingSpeed = 0.05f; // ���ڴ� ����� �ð� (��)
    private Coroutine typingCoroutine;

    void Start()
    {
        // �� ���� �� ù ��° �ؽ�Ʈ ��� ����
        if (introTexts.Length > 0)
        {
            StartDisplayingText(introTexts[currentTextIndex]);
        }
    }

    void Update()
    {
        // �����̽��� �Է� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpaceInput();
        }
    }

    //�����̽���
    void HandleSpaceInput()
    {
        if (isTyping)
        {
            // 1. Ÿ���� ���� ��: ��� ��ü �ؽ�Ʈ ���
            StopCoroutine(typingCoroutine);
            descriptionText.text = introTexts[currentTextIndex];
            isTyping = false;
            isFullyDisplayed = true;
        }
        else if (isFullyDisplayed)
        {
            // 2. ��ü �ؽ�Ʈ�� ǥ�õǾ��� ��: ���� �ؽ�Ʈ�� �̵�
            currentTextIndex++;
            if (currentTextIndex < introTexts.Length)
            {
                // ���� �� ����
                StartDisplayingText(introTexts[currentTextIndex]);
            }
            else
            {
                // ��� �ؽ�Ʈ�� ������ ��: ���� ������ �̵�
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }


    void StartDisplayingText(string textToDisplay)
    {
        isFullyDisplayed = false;
        // ���� �ڷ�ƾ�� �ִٸ� ����
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        // Ÿ���� �ڷ�ƾ ����
        typingCoroutine = StartCoroutine(TypeSentence(textToDisplay));
    }

    // �ѱ��ھ�
    System.Collections.IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        descriptionText.text = ""; // �ؽ�Ʈ �ʱ�ȭ

        foreach (char letter in sentence.ToCharArray())
        {
            descriptionText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // �����
        }

        // ��� �Ϸ�
        isTyping = false;
        isFullyDisplayed = true;
    }
}