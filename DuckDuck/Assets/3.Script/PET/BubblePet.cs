using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class BubblePet : MonoBehaviour
{
    [Header("UI연결")]
    [Tooltip("말풍선 배경 이미지 오브젝트으")]
    public GameObject bubblebackground;

    [Tooltip("기분 UI 텍스트 컴포넌트")]
    public Text textComponet;

    [Header("설정")]
    [Tooltip("말풍선이 떠있는 시간")]
    public float displayTime = 3.0f;

    private Camera _mainCamera;
    private Coroutine _hideCoroutine;

    private void Start()
    {
        _mainCamera = Camera.main;

       // if (bubblebackground != null)
         //   bubblebackground.SetActive(false);
    }


    private void LateUpdate()
    {
        if(bubblebackground!=null&&bubblebackground.activeInHierarchy&&_mainCamera!=null)
        {
            transform.rotation = _mainCamera.transform.rotation;
        }
    }

    public void ShowMessage(string message)
    {
        if (textComponet == null || bubblebackground == null) return;

        textComponet.text = message;
        bubblebackground.SetActive(true);

        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        _hideCoroutine = StartCoroutine(HideBubbleRoutine());
    }
    private IEnumerator HideBubbleRoutine()
    {
        yield return new WaitForSeconds(displayTime);
        bubblebackground.SetActive(false);
    }
}
