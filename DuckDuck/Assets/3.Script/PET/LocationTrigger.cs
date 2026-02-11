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

    private static BubblePet _bubble;
    private bool _hasTriggered = false;

    private void Start()
    {
        if(_bubble == null)
        {
            _bubble = FindAnyObjectByType<BubblePet>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            if(_bubble != null)
            {
                _bubble.ShowMessage(message);
                if(oneTime)
                {
                    _hasTriggered = true;
                }
            }
        }
    }
}
