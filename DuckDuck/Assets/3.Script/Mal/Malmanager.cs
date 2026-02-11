using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class Malmanager : MonoBehaviour
{
    [Header("UI 연결")]
    public Image leftCharImage;
    public Image rightCharImage;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;

    [Header("설정")]
    public float typingSpeed = 0.05f; // 글자 출력 속도
    public List<DialogueLine> dialogueList;

    private int currentIndex = 0;
    private bool isTyping = false;   // 현재 글자가 써지는 중인가?
    private string currentFullSentence; // 현재 출력할 전체 문장

    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    public void StartDialogue()
    {
        if (dialogueList.Count == 0) return;

        Time.timeScale = 0f;
        dialogueCanvas.SetActive(true);
        currentIndex = 0;
        DisplayNext();
    }

    public void DisplayNext()
    {
        // 1. 글자가 써지는 도중에 클릭하면 문장을 한 번에 완성함
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentFullSentence;
            isTyping = false;
            return;
        }

        // 2. 대화가 끝났는지 확인
        if (currentIndex >= dialogueList.Count)
        {
            EndDialogue();
            return;
        }

        // 3. 이미지 강조 처리
        DialogueLine line = dialogueList[currentIndex];
        UpdateVisuals(line.isLeftSpeaker);

        // 4. 타이핑 코루틴 시작
        currentFullSentence = line.sentence;
        StartCoroutine(TypeSentence(line.sentence));

        currentIndex++;
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = ""; // 텍스트 초기화

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            // Time.timeScale이 0일 때도 작동하게 하기 위해 WaitForSecondsRealtime 사용
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    private void UpdateVisuals(bool isLeftSpeaker)
    {
        leftCharImage.color = isLeftSpeaker ? activeColor : inactiveColor;
        rightCharImage.color = isLeftSpeaker ? inactiveColor : activeColor;

        if (isLeftSpeaker) leftCharImage.transform.SetAsLastSibling();
        else rightCharImage.transform.SetAsLastSibling();
    }

    void Update()
    {
        if (dialogueCanvas.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            DisplayNext();
        }
    }

    public void EndDialogue()
    {
        dialogueCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}