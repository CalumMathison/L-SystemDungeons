using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen;
    public Sprite closed, open;

    void Start()
    {
        isOpen = false;
    }

    void Update()
    {
        if (!isOpen)
        {
            UpdateDoor();
        }
    }

    void UpdateDoor()
    {
        if (isOpen)
        {
            this.GetComponentInChildren<SpriteRenderer>().sprite = open;
            this.GetComponent<Collider2D>().enabled = false;
        }
        else
        {
            this.GetComponentInChildren<SpriteRenderer>().sprite = closed;
        }
    }
}
