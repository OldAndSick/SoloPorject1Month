using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData itemData;
    public float rotationSpeed = 100f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = Mathf.Sin(Time.time * 2f) * 0.1f;
        transform.position += new Vector3(0, newY * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if(pc != null && itemData != null)
            {
                pc.AcquireItem(itemData);
                Destroy(gameObject);
                Debug.Log($"{itemData} get");
            }
            else if(itemData == null)
            {
                Debug.LogError($"{gameObject.name}에 ItemData가 비어있다");
            }
        }
    }
}
