using UnityEngine;

public class MainBGM : MonoBehaviour
{
    private static MainBGM instance;

    void Awake()
    {
        // 씬이 넘어가도 나(BGM 매니저)를 파괴하지 마라!!
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 어? 이미 BGM 매니저가 있네? 그럼 새로 생긴 짝퉁은 자살(삭제)해라!!
            Destroy(gameObject);
        }
    }
}