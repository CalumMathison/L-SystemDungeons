using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Sprite sprite;
    public Vector2 position;
    public int score = 1;

    public void Setup()
    {
        GetComponent<SpriteRenderer>().sprite = sprite;       
    }
}
