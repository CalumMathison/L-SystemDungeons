using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int width, height;
    private GameObject[,] tiles;
    public Sprite sprite, blank, wall, sFloor, cFloor, lFloor, pFloor, bFloor;
    public GameObject tilePrefab;
    Pathfinder pathfinder;
    GameObject start, end;

    public void Awake()
    {
        width = 140;
        height = 140;
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
                tiles[x, y].GetComponent<Tile>().isFloor = false;
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
                                    switch (n.type)
                                    {
                                        case 'S':
                                            tiles[x + w, y + h].GetComponent<Tile>().sprite = sFloor;
                                            break;
                                        case 'C':
                                            tiles[x + w, y + h].GetComponent<Tile>().sprite = cFloor;
                                            break;
                                        case 'P':
                                            tiles[x + w, y + h].GetComponent<Tile>().sprite = pFloor;
                                            break;
                                        case 'L':
                                            tiles[x + w, y + h].GetComponent<Tile>().sprite = lFloor;
                                            break;
                                        case 'B':
                                            tiles[x + w, y + h].GetComponent<Tile>().sprite = bFloor;
                                            break;
                                    }
                                    tiles[x + w, y + h].GetComponent<Tile>().Setup();
                                    tiles[x + w, y + h].GetComponent<Tile>().isFloor = true;
                                }
                            }
                        }
                        else if (n.width > 1)
                        {
                            for (int w = 0; w < n.width; w++)
                            {
                                switch (n.type)
                                {
                                    case 'S':
                                        tiles[x + w, y].GetComponent<Tile>().sprite = sFloor;
                                        break;
                                    case 'C':
                                        tiles[x + w, y].GetComponent<Tile>().sprite = cFloor;
                                        break;
                                    case 'P':
                                        tiles[x + w, y].GetComponent<Tile>().sprite = pFloor;
                                        break;
                                    case 'L':
                                        tiles[x + w, y].GetComponent<Tile>().sprite = lFloor;
                                        break;
                                    case 'B':
                                        tiles[x + w, y].GetComponent<Tile>().sprite = bFloor;
                                        break;
                                }
                                tiles[x + w, y].GetComponent<Tile>().Setup();
                                tiles[x + w, y].GetComponent<Tile>().isFloor = true;
                            }
                        }
                        else if (n.height > 1)
                        {
                            for (int h = 0; h < n.height; h++)
                            {
                                switch (n.type)
                                {
                                    case 'S':
                                        tiles[x, y + h].GetComponent<Tile>().sprite = sFloor;
                                        break;
                                    case 'C':
                                        tiles[x, y + h].GetComponent<Tile>().sprite = cFloor;
                                        break;
                                    case 'P':
                                        tiles[x , y + h].GetComponent<Tile>().sprite = pFloor;
                                        break;
                                    case 'L':
                                        tiles[x , y + h].GetComponent<Tile>().sprite = lFloor;
                                        break;
                                    case 'B':
                                        tiles[x, y + h].GetComponent<Tile>().sprite = bFloor;
                                        break;
                                }
                                tiles[x, y + h].GetComponent<Tile>().Setup();
                                tiles[x, y + h].GetComponent<Tile>().isFloor = true;
                            }
                        }

                    }              
                }
                
            }
        }

        CalculateWalls();
        //List<GameObject> paths = pathfinder.FindPath(tiles, start, end);
        //Debug.Log(paths.Count);
        //foreach (GameObject t in paths)
        //{
        //    t.GetComponent<Tile>().sprite = sprite;
        //    t.GetComponent<Tile>().Setup();
        //}
    }

    public void CalculateWalls()
    {
        for (int x = 1; x < width - 1 ; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (tiles[x,y].GetComponent<Tile>().sprite == blank && 
                    (tiles[x - 1, y].GetComponent<Tile>().isFloor == true || tiles[x, y - 1].GetComponent<Tile>().isFloor == true ||
                    tiles[x - 1, y - 1].GetComponent<Tile>().isFloor == true || tiles[x + 1, y + 1].GetComponent<Tile>().isFloor == true ||
                    tiles[x + 1, y].GetComponent<Tile>().isFloor == true || tiles[x, y + 1].GetComponent<Tile>().isFloor == true ||
                    tiles[x + 1, y - 1].GetComponent<Tile>().isFloor == true || tiles[x - 1, y + 1].GetComponent<Tile>().isFloor == true)
                    )
                {
                    tiles[x, y].GetComponent<Tile>().sprite = wall;
                    tiles[x, y].GetComponent<Tile>().Setup();
                }
            }
        }
    }

}
