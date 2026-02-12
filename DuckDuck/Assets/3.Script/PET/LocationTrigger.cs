using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationTrigger : MonoBehaviour
{
    [Header("Text Settings")]
    [TextArea]
    public string message;
    public bool oneTime = true;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            BubblePet.Instance.ShowMessage(message);
            if(oneTime)
            {
                _hasTriggered = true;
            }
        }
        else
        {
            Debug.Log("no bubblepet");
        }
    }
}
