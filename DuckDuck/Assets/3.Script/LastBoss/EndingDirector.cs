using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingDirector : MonoBehaviour
{
    [Header("시네마틱 오브젝트")]
    public Transform door;
    public Transform[] animals;

    [Header("화면 & UI 세팅")]
    public Image blackScreen;
    public GameObject creditsCanvas;
    public RectTransform creditsText;
    public float scrollSpeed = 50f;

    // ---------------------------------------------------
    [Header("뉴스 자막 세팅")]
    public TextMeshProUGUI newsText;                  // [띠또 마법 1] 자막이 뜰 텍스트 UI
    [TextArea(2, 3)]
    public string[] newsLines;             // [띠또 마법 2] 바뀔 자막 내용들 (인스펙터에서 작성)
    public float textChangeDelay = 3f;     // [띠또 마법 3] 몇 초마다 자막을 바꿀지
    // ---------------------------------------------------

    [Header("오디오 세팅")]
    public AudioSource audioSrc;
    public AudioClip doorOpenSound;
    public AudioClip gunshotSound;

    void Start()
    {
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        yield return new WaitForSeconds(1f);

        // 1. 문 열림
        if (doorOpenSound) audioSrc.PlayOneShot(doorOpenSound);
        float t = 0;
        Quaternion startRot = door.rotation;
        Quaternion endRot = door.rotation * Quaternion.Euler(0, -90f, 0);
        while (t < 1f)
        {
            t += Time.deltaTime * 0.5f;
            door.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        // 2. 동물들 돌아봄
        float turnTimer = 0f;
        Quaternion[] startRots = new Quaternion[animals.Length];
        Quaternion[] targetRots = new Quaternion[animals.Length];

        for (int i = 0; i < animals.Length; i++)
        {
            startRots[i] = animals[i].rotation;
            Vector3 lookDir = Camera.main.transform.position - animals[i].position;
            lookDir.y = 0;
            targetRots[i] = Quaternion.LookRotation(lookDir);
        }

        while (turnTimer < 1f)
        {
            turnTimer += Time.deltaTime * 1.5f;
            for (int i = 0; i < animals.Length; i++)
            {
                animals[i].rotation = Quaternion.Slerp(startRots[i], targetRots[i], turnTimer);
            }
            yield return null;
        }

        // 3. 정적
        yield return new WaitForSeconds(2f);

        // 4. 암전 (강제 기상 마법 포함!)
        blackScreen.gameObject.SetActive(true);
        blackScreen.color = new Color(0, 0, 0, 1f);

        // 5. 총소리
        yield return new WaitForSeconds(0.5f);
        if (gunshotSound) audioSrc.PlayOneShot(gunshotSound);

        // 6. 여운
        yield return new WaitForSeconds(3f);

        // 게임 클리어 저장
        PlayerPrefs.SetInt("GameCleared", 1);
        PlayerPrefs.Save();

        // 7. 크레딧 & 뉴스 화면 켜기!
        creditsCanvas.SetActive(true);

        // [띠또 마법 ⭐] 화면 켜짐과 동시에 뉴스 자막 교체 시작!!
        if (newsText != null && newsLines.Length > 0)
        {
            StartCoroutine(NewsTickerRoutine());
        }
    }

    // [띠또 마법 핵심] 자막을 일정 시간마다 갈아끼우는 코루틴!
    private IEnumerator NewsTickerRoutine()
    {
        int index = 0;

        // 크레딧 화면이 켜져 있는 동안 무한 반복!
        while (creditsCanvas.activeSelf)
        {
            newsText.text = newsLines[index]; // 자막 갈아끼우기
            yield return new WaitForSeconds(textChangeDelay); // 3초 대기

            index++;

            // 준비한 자막을 다 썼으면, 다시 첫 번째 자막으로 돌아가서 반복!
            if (index >= newsLines.Length)
            {
                break;
            }
        }
    }

    void Update()
    {
        if (creditsCanvas.activeSelf && creditsText != null)
        {
            creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
    }

    public void SkipCredits()
    {
        // 아까 만드신 일반 타이틀 씬으로 돌아가면, MainMenu.cs가 알아서 진엔딩 타이틀로 납치해 갑니다!
        SceneManager.LoadScene("Title2");
    }
}