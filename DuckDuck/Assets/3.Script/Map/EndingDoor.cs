using UnityEngine;
using UnityEngine.SceneManagement; // [띠또 마법 ⭐] 씬을 이동하려면 반드시 필요합니다!

// 주인님의 기존 상호작용 시스템(Interact)을 그대로 물려받습니다!
public class EndingDoor : MonoBehaviour, Interact
{
    [Header("이동할 엔딩 씬 이름")]
    public string endingSceneName = "EndingScene";

    // 플레이어가 문 앞에서 'E'를 누르면 이 함수가 자동으로 실행됩니다!
    public void Interact(PlayerController player)
    {
        Debug.Log("대망의 엔딩 씬으로 넘어갑니다...!!");

        // 엔딩 씬으로 순간이동!
        SceneManager.LoadScene(endingSceneName);
    }
}