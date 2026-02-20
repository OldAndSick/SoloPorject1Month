using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; // 이벤트 처리를 위해 추가

public class Malmanager : MonoBehaviour
{
    [Header("UI 연결")]
    public Image leftCharImage;
    public Image rightCharImage;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;

    [Header("설정")]
    public float typingSpeed = 0.05f;
    private List<DialogueLine> dialogueList; // 외부에서 주입받도록 수정

    [Header("종료 후 실행할 이벤트")]
    public UnityEvent onDialogueEnd; // 여기에 몹 소환 함수를 연결


    private int currentIndex = 0;
    private bool isTyping = false;
    private string currentFullSentence;
    private Coroutine typingCoroutine;

    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    // 기존 변수들 선언 아래에 추가
    [Header("시작 대화 설정")]
    public bool playOnStart = true; // 시작하자마자 재생할 것인가?
    public List<DialogueLine> startDialogueList; // 시작할 때 나올 대화 내용

    void Start()
    {
        if (playOnStart)
        {
            // 내 인스펙터에 적어둔 대화 내용을 가지고 대화 시작!
            StartDialogue(startDialogueList);
        }
    }
    // 원하는 타이밍에 이 함수를 호출 (대화 데이터 전달)
    public void StartDialogue(List<DialogueLine> lines)
    {
        dialogueList = lines;
        if (dialogueList == null || dialogueList.Count == 0) return;

        Time.timeScale = 0f; // 게임 정지
        dialogueCanvas.SetActive(true);
        currentIndex = 0;
        DisplayNext();
    }

    public void DisplayNext()
    {
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = currentFullSentence;
            isTyping = false;
            return;
        }

        if (currentIndex >= dialogueList.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueList[currentIndex];
        UpdateVisuals(line.isLeftSpeaker);
        currentFullSentence = line.sentence;
        typingCoroutine = StartCoroutine(TypeSentence(line.sentence));

        currentIndex++;
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    private void UpdateVisuals(bool isLeftSpeaker)
    {
        if (leftCharImage == null || rightCharImage == null) return;

        // 왼쪽이 말할 때: 왼쪽(Active), 오른쪽(Inactive)
        // 오른쪽이 말할 때: 왼쪽(Inactive), 오른쪽(Active)
        leftCharImage.color = isLeftSpeaker ? activeColor : inactiveColor;
        rightCharImage.color = isLeftSpeaker ? inactiveColor : activeColor;

        // 말하는 사람을 레이어 맨 앞으로 보내기
        if (isLeftSpeaker) leftCharImage.transform.SetAsLastSibling();
        else rightCharImage.transform.SetAsLastSibling();
    }

    void Update()
    {
        if (dialogueCanvas.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)))
        {
            DisplayNext();
        }
    }

    public void EndDialogue()
    {
        dialogueCanvas.SetActive(false);
        Time.timeScale = 1f; // 게임 재개

        // 등록된 이벤트(몹 소환 등) 실행
        if (onDialogueEnd != null)
        {
            onDialogueEnd.Invoke();
        }
    }
}