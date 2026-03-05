using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour, Interact
{
    public ItemData itemData;
    public float rotationSpeed = 100f;
    public AudioClip pickUpSound;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = Mathf.Sin(Time.time * 2f) * 0.1f;
        transform.position += new Vector3(0, newY * Time.deltaTime, 0);
    }
    public void Interact(PlayerController player)
    {
        if(itemData != null)
        {
            if (pickUpSound != null)
            {
                AudioSource.PlayClipAtPoint(pickUpSound, transform.position);
            }
            player.AcquireItem(itemData);
            Destroy(gameObject);
        }
    }
}
