using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro를 사용하는 경우

public class GameIntroManager : MonoBehaviour
{
    [Header("UI 연결")]
    public Text descriptionText; // 설명 텍스트를 표시할 TMP 컴포넌트

    [Header("게임 정보")]
    [TextArea(1, 3)]
    // 여기에 게임 설명 텍스트를 한 줄씩 입력하세요.
    public string[] introTexts;
    public string nextSceneName = "GameScene";


    private int currentTextIndex = 0;
    private bool isTyping = false; // 현재 텍스트 출력 중인지 확인
    private bool isFullyDisplayed = false; // 현재 텍스트가 모두 출력되었는지 확인

    public float typingSpeed = 0.05f; // 글자당 딜레이 시간 (초)
    private Coroutine typingCoroutine;

    void Start()
    {
        // 씬 시작 시 첫 번째 텍스트 출력 시작
        if (introTexts.Length > 0)
        {
            StartDisplayingText(introTexts[currentTextIndex]);
        }
    }

    void Update()
    {
        // 스페이스바 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpaceInput();
        }
    }

    //스페이스바
    void HandleSpaceInput()
    {
        if (isTyping)
        {
            // 1. 타이핑 중일 때: 즉시 전체 텍스트 출력
            StopCoroutine(typingCoroutine);
            descriptionText.text = introTexts[currentTextIndex];
            isTyping = false;
            isFullyDisplayed = true;
        }
        else if (isFullyDisplayed)
        {
            // 2. 전체 텍스트가 표시되었을 때: 다음 텍스트로 이동
            currentTextIndex++;
            if (currentTextIndex < introTexts.Length)
            {
                // 다음 줄 시작
                StartDisplayingText(introTexts[currentTextIndex]);
            }
            else
            {
                // 모든 텍스트가 끝났을 때: 다음 씬으로 이동
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }


    void StartDisplayingText(string textToDisplay)
    {
        isFullyDisplayed = false;
        // 기존 코루틴이 있다면 중지
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        // 타이핑 코루틴 시작
        typingCoroutine = StartCoroutine(TypeSentence(textToDisplay));
    }

    // 한글자씩
    System.Collections.IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        descriptionText.text = ""; // 텍스트 초기화

        foreach (char letter in sentence.ToCharArray())
        {
            descriptionText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // 딜레이
        }

        // 출력 완료
        isTyping = false;
        isFullyDisplayed = true;
    }
}