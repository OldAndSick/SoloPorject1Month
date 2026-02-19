using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interact
{
    [Header("Door Settings")]
    public bool isOpen = false;
    public Vector3 slideOffset = new Vector3(2f, 0f, 0f);
    public float slideSpeed = 5f;

    private Vector3 closedPoisiton;
    private Vector3 openPosition;
    private Coroutine doorCoroutine;
    private void Start()
    {
        closedPoisiton = transform.localPosition;
        openPosition = closedPoisiton + slideOffset;
    }
    public void Interact(PlayerController player)
    {
        isOpen = !isOpen;
        if(doorCoroutine != null)
        {
            StopCoroutine(doorCoroutine);
        }
        doorCoroutine = StartCoroutine(SlideDoor());
    }

    private IEnumerator SlideDoor()
    {
        Vector3 targetPos = isOpen ? openPosition : closedPoisiton;
        while(Vector3.Distance(transform.localPosition, targetPos) >0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * slideSpeed);
            yield return null;
        }
        transform.localPosition = targetPos;
    }
}
