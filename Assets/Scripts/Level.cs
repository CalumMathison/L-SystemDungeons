using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int width, height;
    private GameObject[,] tiles;
    public Sprite sprite, blank, wall;
    public GameObject tilePrefab;
    Pathfinder pathfinder;
    GameObject start, end;

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

        pathfinder = new Pathfinder();
    }

    public void Setup(List<Node> nodes)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y].GetComponent<Tile>().sprite = blank;
                tiles[x, y].GetComponent<Tile>().Setup();
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (Node n in nodes)
                {
                    if (n == nodes[0])
                    {
                        start = tiles[x, y];
                    }
                    else if (n == nodes[nodes.Count - 1])
                    {
                        end = tiles[x, y];
                    }

                    if (tiles[x, y].GetComponent<Collider2D>().bounds.Contains(n.positon))
                    {
                       //tiles[x, y].GetComponent<Tile>().sprite = sprite;
                        if (n.width > 1 && n.height > 1)
                        {
                            for (int w = 0; w < n.width; w++)
                            {
                                for (int h = 0; h < n.height; h++)
                                {
                                    tiles[x + w, y + h].GetComponent<Tile>().sprite = sprite;
                                    tiles[x + w, y + h].GetComponent<Tile>().Setup();
                                }
                            }
                        }
                        else if (n.width > 1)
                        {
                            for (int w = 0; w < n.width; w++)
                            {
                                tiles[x + w, y].GetComponent<Tile>().sprite = sprite;
                                tiles[x + w, y].GetComponent<Tile>().Setup();
                            }
                        }
                        else if (n.height > 1)
                        {
                            for (int h = 0; h < n.height; h++)
                            {
                                tiles[x, y + h].GetComponent<Tile>().sprite = sprite;
                                tiles[x, y + h].GetComponent<Tile>().Setup();
                            }
                        }

                    }              
                }
                
            }
        }

        CalculateWalls();
        List<GameObject> paths = pathfinder.FindPath(tiles, start, end);
        Debug.Log(paths.Count);
        foreach (GameObject t in paths)
        {
            t.GetComponent<Tile>().sprite = sprite;
            t.GetComponent<Tile>().Setup();
        }
    }

    public void CalculateWalls()
    {
        for (int x = 1; x < width - 1 ; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (tiles[x,y].GetComponent<Tile>().sprite != sprite && 
                    (tiles[x - 1, y].GetComponent<Tile>().sprite == sprite || tiles[x, y - 1].GetComponent<Tile>().sprite == sprite ||
                    tiles[x - 1, y - 1].GetComponent<Tile>().sprite == sprite || tiles[x + 1, y + 1].GetComponent<Tile>().sprite == sprite ||
                    tiles[x + 1, y].GetComponent<Tile>().sprite == sprite || tiles[x, y + 1].GetComponent<Tile>().sprite == sprite ||
                    tiles[x + 1, y - 1].GetComponent<Tile>().sprite == sprite || tiles[x - 1, y + 1].GetComponent<Tile>().sprite == sprite)
                    )
                {
                    tiles[x, y].GetComponent<Tile>().sprite = wall;
                    tiles[x, y].GetComponent<Tile>().Setup();
                }
            }
        }
    }

}
