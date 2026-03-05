using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro�� ����ϴ� ���
using System.Collections;

public class GameIntroManager : MonoBehaviour
{
    [Header("UI 설정")]
    public Text descriptionText;
    public Image fadeImage; // 에디터에서 검은색 Image를 연결하세요.
    [Header("콘텐츠 설정")]
    [TextArea(1, 3)]
    public string[] introTexts;
    public string nextSceneName = "GameScene";


    [Header("시간 설정")]
    public float typingSpeed = 0.05f;
    public float fadeDuration = 1.0f; // 페이드 아웃에 걸리는 시간

    private int currentTextIndex = 0;
    private bool isTyping = false;
    private bool isFullyDisplayed = false;
    private bool isExiting = false; // 중복 실행 방지
    private Coroutine typingCoroutine;

    void Start()
    {
        // 시작할 때 이미지를 투명하게 초기화
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }

        if (introTexts.Length > 0)
        {
            StartDisplayingText(introTexts[currentTextIndex]);
        }
    }

    void Update()
    {
        // �����̽��� �Է� ����
        if (Input.GetMouseButtonDown(0))
        {
            HandleSpaceInput();
        }
    }

    //�����̽���
    void HandleSpaceInput()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            descriptionText.text = introTexts[currentTextIndex];
            isTyping = false;
            isFullyDisplayed = true;
        }
        else if (isFullyDisplayed)
        {
            currentTextIndex++;
            if (currentTextIndex < introTexts.Length)
            {
                StartDisplayingText(introTexts[currentTextIndex]);
            }
            else
            {
                // 모든 텍스트 종료 시 페이드 아웃 시작
                StartCoroutine(FadeOutAndLoadScene());
            }
        }
    }
    IEnumerator FadeOutAndLoadScene()
    {
        isExiting = true; // 더 이상 클릭 안 되게 방지
        float timer = 0f;
        Color tempColor = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // 알파값을 0에서 1로 서서히 올림
            tempColor.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            fadeImage.color = tempColor;
            yield return null;
        }

        // 씬 전환
        SceneManager.LoadScene(nextSceneName);
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