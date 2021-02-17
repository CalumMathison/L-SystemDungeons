using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Sprite sprite;
    public Vector2 position;
    public int score = 1;
    public bool isFloor = false;
    public int xIndex, yIndex;
    public float gScore = 1;
    public Tile parent;

    public float X
    {
        get { return position.x; }
        set { position.x = value; }
    }

    public float Y
    {
        get { return position.y; }
        set { position.y = value; }
    }

    public void Setup()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;    
        
    }
}
