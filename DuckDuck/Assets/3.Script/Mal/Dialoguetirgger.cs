using System.Collections.Generic;
using UnityEngine;

public class Dialoguetirgger : MonoBehaviour
{
    public List<DialogueLine> myDialogue; // 이 트리거 전용 대화 내용
    private bool hasPlayed = false;

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            FindObjectOfType<Malmanager>().StartDialogue(myDialogue);
            hasPlayed = true; // 한 번만 실행되도록
        }
    }
}
