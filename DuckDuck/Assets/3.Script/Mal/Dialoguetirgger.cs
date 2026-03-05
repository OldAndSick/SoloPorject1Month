using System.Collections.Generic;
using UnityEngine;

public class Dialoguetirgger : MonoBehaviour
{
    public List<DialogueLine> myDialogue; // �� Ʈ���� ���� ��ȭ ����
    private bool hasPlayed = false;

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            FindObjectOfType<Malmanager>().StartDialogue(myDialogue);
            hasPlayed = true; // �� ���� ����ǵ���
        }
    }
}
