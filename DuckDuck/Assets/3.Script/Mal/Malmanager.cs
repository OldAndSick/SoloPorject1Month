using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; // �̺�Ʈ ó���� ���� �߰�

public class Malmanager : MonoBehaviour
{
    [Header("UI ����")]
    public Image leftCharImage;
    public Image rightCharImage;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueCanvas;
    public GameObject speedchbubble;

    [Header("����")]
    public float typingSpeed = 0.05f;
    private List<DialogueLine> dialogueList; // �ܺο��� ���Թ޵��� ����

    [Header("���� �� ������ �̺�Ʈ")]
    public UnityEvent onDialogueEnd; // ���⿡ �� ��ȯ �Լ��� ����


    private int currentIndex = 0;
    private bool isTyping = false;
    private string currentFullSentence;
    private Coroutine typingCoroutine;

    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    // ���� ������ ���� �Ʒ��� �߰�
    [Header("���� ��ȭ ����")]
    public bool playOnStart = true; // �������ڸ��� ����� ���ΰ�?
    public List<DialogueLine> startDialogueList; // ������ �� ���� ��ȭ ����

    void Start()
    {
        if (playOnStart)
        {
            // �� �ν����Ϳ� ����� ��ȭ ������ ������ ��ȭ ����!
            StartDialogue(startDialogueList);
        }
    }
    // ���ϴ� Ÿ�ֿ̹� �� �Լ��� ȣ�� (��ȭ ������ ����)
    public void StartDialogue(List<DialogueLine> lines)
    {
        dialogueList = lines;
        if (dialogueList == null || dialogueList.Count == 0) return;

        Time.timeScale = 0f; // ���� ����
        dialogueCanvas.SetActive(true);
        if (speedchbubble != null)
            speedchbubble.SetActive(true);
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

        // ������ ���� ��: ����(Active), ������(Inactive)
        // �������� ���� ��: ����(Inactive), ������(Active)
        leftCharImage.color = isLeftSpeaker ? activeColor : inactiveColor;
        rightCharImage.color = isLeftSpeaker ? inactiveColor : activeColor;

        // ���ϴ� ����� ���̾� �� ������ ������
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

        // ��ϵ� �̺�Ʈ(�� ��ȯ ��) ����
        if (onDialogueEnd != null)
        {
            onDialogueEnd.Invoke();
        }
    }
}