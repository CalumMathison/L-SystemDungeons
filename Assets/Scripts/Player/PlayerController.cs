using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    float horizontal;
    float vertical;

    public float runSpeed = 20.0f;
    public Level currLevel;
    public Vector2 startPos;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = currLevel.start.GetComponent<Tile>().position;
        startPos = currLevel.start.GetComponent<Tile>().position;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (startPos != currLevel.start.GetComponent<Tile>().position)
        {
            transform.position = currLevel.start.GetComponent<Tile>().position;
            startPos = currLevel.start.GetComponent<Tile>().position;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}
