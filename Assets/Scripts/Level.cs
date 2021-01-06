using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int width, height;
    private GameObject[,] tiles;
    public Sprite sprite, blank;
    public GameObject tilePrefab;

    public void Awake()
    {
        width = 100;
        height = 100;
        tiles = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = Instantiate(tilePrefab, new Vector3(x - (width / 2) - 0.5f, y - (height / 2) - 0.5f, 0), Quaternion.identity) as GameObject;
            }
        }
    }

    public void Setup(List<Node> nodes)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].GetComponent<Tile>().sprite = blank;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (Node n in nodes)
                {
                    if (tiles[x, y].GetComponent<Collider2D>().bounds.Contains(n.positon))
                    {
                        tiles[x, y].GetComponent<Tile>().sprite = sprite;
                    }                   
                }
                tiles[x, y].GetComponent<Tile>().Setup();
            }
        }
    }  
}
