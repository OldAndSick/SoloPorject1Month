using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class BubblePet : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("bubble background image")]
    public GameObject bubblebackground;

    [Tooltip("UI text component")]
    public Text textComponet;

    [Header("Settings")]
    [Tooltip("Bubble displayTime")]
    public float displayTime = 3.0f;

    private Coroutine _hideCoroutine;

    private void Start()
    {
        if (bubblebackground != null)
        {
            bubblebackground.SetActive(false);
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
