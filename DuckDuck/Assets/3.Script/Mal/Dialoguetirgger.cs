using System.Collections.Generic;
using UnityEngine;

public class Dialoguetirgger : MonoBehaviour
{

    public List<DialogueLine> myDialogue; // 이 트리거 전용 대화 내용

    private bool hasPlayed = false;

    // [띠또 마법] 2D 글자를 빼고 3D 물리 엔진인 OnTriggerEnter로 바꿨습니다!!
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            // 최신 유니티 버전에 맞게 FindObjectOfType 대신 FindFirstObjectByType을 쓰면 더 좋습니다!
            FindAnyObjectByType<Malmanager>().StartDialogue(myDialogue);
            hasPlayed = true; // 한 번만 실행되도록
        }
    }
}