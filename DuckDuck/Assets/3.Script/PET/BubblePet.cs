using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class BubblePet : MonoBehaviour
{
    public static BubblePet Instance;

    [Header("UI Settings")]
    [Tooltip("bubble background image")]
    public GameObject bubblebackground;
    public Transform chatParent;

    [Header("Settings")]
    [Tooltip("Bubble displayTime")]
    public float displayTime = 3.0f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
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

        Transform portraitTrans = newBubble.transform.Find("Portrait");
        if(portraitTrans != null)
        {
            Image portraitImg = portraitTrans.GetComponent<Image>();
            if (portraitImg != null && portrait != null)
            {
                portraitImg.sprite = portrait;
            }
        }
        Destroy(newBubble, displayTime);
    }
}
