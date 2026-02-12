using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class BubblePet : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("bubble background image")]
    public GameObject bubblebackground;
    public Transform chatParent;

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
    public void ShowMessage(string message, Sprite portrait = null)
    {
        if (chatParent == null || bubblebackground == null) return;

        GameObject newBubble = Instantiate(bubblebackground, chatParent);
        newBubble.SetActive(true);

        Text txt = newBubble.GetComponentInChildren<Text>();
        
        if (txt != null)
        {
            txt.text = message;
        }

        Image portraitImg = newBubble.transform.Find("Portrait")?.GetComponent<Image>();
        if(portraitImg != null && portrait !=null)
        {
            portraitImg.sprite = portrait;
        }
        Destroy(newBubble, displayTime);
    }
    private IEnumerator HideBubbleRoutine()
    {
        yield return new WaitForSeconds(displayTime);
        bubblebackground.SetActive(false);
    }
}
