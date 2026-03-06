using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class Malmanager : MonoBehaviour
{
    [Header("UI Settings")]
    public Image leftCharImage;
    public Image rightCharImage;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;
    public GameObject speedchbubble;

    [Header("Settings")]
    public float typingSpeed = 0.05f;
    private List<DialogueLine> dialogueList; 

    [Header("End event")]
    public UnityEvent onDialogueEnd; 


    private int currentIndex = 0;
    private bool isTyping = false;
    private string currentFullSentence;
    private Coroutine typingCoroutine;

    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    
    [Header("Start text")]
    public bool playOnStart = true; 
    public List<DialogueLine> startDialogueList; 

    void Start()
    {
        if (playOnStart)
        {
            
            StartDialogue(startDialogueList);
        }
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        // [방어막 1] 트리거에 대화 내용이 비어있으면 아예 무시!
        if (lines == null || lines.Count == 0) return;

        // [방어막 2] 만약 이전 대화가 아직 타자 치는 중이었다면 강제 종료!
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
        }

        dialogueList = lines;
        Time.timeScale = 0f;
        dialogueCanvas.SetActive(true);
        if (speedchbubble != null) speedchbubble.SetActive(true);

        currentIndex = 0;
        DisplayNext();
    }

    public void DisplayNext()
    {
        // [방어막 3] 리스트가 꼬여서 날아갔을 때 에러 방지
        if (dialogueList == null) return;

        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            // [방어막 4] 문장이 null일 때 뻗는 것 방지
            dialogueText.text = currentFullSentence != null ? currentFullSentence : "";
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

        // [방어막 5 ⭐] 인스펙터에서 대화 내용을 안 적고 빈칸으로 뒀을 때 에러 방지!!
        if (string.IsNullOrEmpty(line.sentence))
        {
            currentFullSentence = "..."; // 안 적어뒀으면 점점점 출력
        }
        else
        {
            currentFullSentence = line.sentence;
        }

        typingCoroutine = StartCoroutine(TypeSentence(currentFullSentence));
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

        leftCharImage.color = isLeftSpeaker ? activeColor : inactiveColor;
        rightCharImage.color = isLeftSpeaker ? inactiveColor : activeColor;

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
        if (speedchbubble != null)
            speedchbubble.SetActive(false);
        dialogueCanvas.SetActive(false);
        Time.timeScale = 1f; // ���� �簳

        if (onDialogueEnd != null)
        {
            onDialogueEnd.Invoke();
        }
    }
}