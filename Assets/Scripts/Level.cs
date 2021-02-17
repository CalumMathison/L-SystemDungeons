using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

public class Level : MonoBehaviour
{
    public int width, height;
    private GameObject[,] tiles;
    public Sprite sprite, blank, wall, sFloor, cFloor, lFloor, pFloor, bFloor;
    public GameObject tilePrefab;
    Pathfinder pathfinder;
    GameObject start, end;
    bool[,] graph;
    UnityEngine.Color color;

    public void Awake()
    {
        width = 140;
        height = 140;
        tiles = new GameObject[width, height];
        graph = new bool[width, height];
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
                tiles[x, y].GetComponent<Tile>().xIndex = x;
                tiles[x, y].GetComponent<Tile>().yIndex = y;
                tiles[x, y].GetComponent<Tile>().position = tiles[x, y].transform.position;
                graph[x, y] = true;
            }
        }

        List<Tile> originPoints = new List<Tile>();


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (tiles[x, y].GetComponent<Collider2D>().bounds.Contains(nodes[i].positon))
                    {
                        originPoints.Add(tiles[x, y].GetComponent<Tile>());
                    }
                }
            }
        }

        for (int i = 0; i < originPoints.Count-1; i++)
        {
            MakePath(originPoints[i], originPoints[i+1]);
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foreach (Node n in nodes)
                {
                    if (tiles[x, y].GetComponent<Collider2D>().bounds.Contains(n.positon))
                    {
                        if (n == nodes.First<Node>())
                        {
                            start = tiles[x, y];
                        }
                        if (n == nodes.Last<Node>())
                        {
                            end = tiles[x, y];
                        }

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
        Debug.Log("Start index = " + start.GetComponent<Tile>().xIndex + ", " + start.GetComponent<Tile>().yIndex);
        Debug.Log("End index = " + end.GetComponent<Tile>().xIndex + ", " + end.GetComponent<Tile>().yIndex);
        Debug.Log("Start tile position is " + start.GetComponent<Tile>().position.x + ", " + start.GetComponent<Tile>().position.y);
        Debug.Log("Start tile position is " + end.GetComponent<Tile>().position.x + ", " + end.GetComponent<Tile>().position.y);
       // MakePath(start.GetComponent<Tile>(), end.GetComponent<Tile>());

        CalculateWalls();
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

    void MakePath(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        Debug.Log("Added tile of index " + start.xIndex + ", " + start.yIndex + "to open list");
        openList.Add(tiles[start.xIndex, start.yIndex].GetComponent<Tile>());
        openList[0].gScore = Vector2.Distance(start.position, end.position);
        List<Tile> closedList = new List<Tile>();
        List<Tile> neighbours = new List<Tile>();
        Tile current = openList[0];
        bool pathfound = false;
        bool error = false;
        float g = 0, h = 0, f = 0, nh, nf;
        g = 0;
        h = 0;
        f = Vector2.Distance(start.position, end.position);
        float currG = 0;
        float timer = 0;
        f = Vector2.Distance(tiles[start.xIndex, start.yIndex].GetComponent<Tile>().position, tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position);

        while (!pathfound && !error)
        {
            timer += 0.00001f;
            if (timer > 1)
            {
                Debug.Log("Error: ALgorithm took more than 10000 ticks");
                error = true;
            }

            foreach (Tile i in openList)
            {
                nh = Vector2.Distance(i.position, tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position);

                nf = i.gScore;

                if (nf < f)
                {
                    current = i;
                    f = current.gScore;
                }

                //current = openList.First<Tile>();
                //openList.Remove(current);

                h = 0;
               // f = float.MaxValue;

            }
            if (current.xIndex == end.xIndex &&
                current.yIndex == end.yIndex)
            {
                Debug.Log("Path found");
                pathfound = true;
            }
            else
            {
                openList.Remove(current);
                closedList.Add(current);
                neighbours.Clear();
                if (current.xIndex >= 1 && current.xIndex < 139)
                {
                    neighbours.Add(tiles[current.xIndex + 1, current.yIndex].GetComponent<Tile>());
                    neighbours.Add(tiles[current.xIndex - 1, current.yIndex].GetComponent<Tile>());
                }
                if (current.yIndex >= 1 && current.yIndex < 139)
                {
                    neighbours.Add(tiles[current.xIndex, current.yIndex + 1].GetComponent<Tile>());
                    neighbours.Add(tiles[current.xIndex, current.yIndex - 1].GetComponent<Tile>());
                }

                foreach (Tile i in neighbours)
                {
                    if (i.gScore < current.gScore)// && (closedList.Contains(i) || openList.Contains(i)))
                    {
                        Debug.Log("Added tile " + current.xIndex + ", " + current.yIndex + " as parent of tile " + i.xIndex + ", " + i.yIndex);
                        tiles[i.xIndex, i.yIndex].GetComponent<Tile>().parent = tiles[current.xIndex, current.yIndex].GetComponent<Tile>();
                       // i.parent = current;
                        //current = i;
                        if (openList.Contains(i))
                        {
                            Debug.Log("Removed tile at index " + i.xIndex + ", " + i.yIndex + " from open list");
                            openList.Remove(i);
                        }
                        if (closedList.Contains(i))
                        {
                            Debug.Log("Removed tile at index " + i.xIndex + ", " + i.yIndex + " from closed list");
                            closedList.Remove(i);
                        }
                    }
                    
                    if (!closedList.Contains(i) && (!openList.Contains(i)))
                    {
                        Debug.Log("Added tile at index " + i.xIndex + ", " + i.yIndex + " to open list");                     
                        i.gScore = Vector2.Distance(i.position, end.position);
                        i.parent = current;
                        Debug.Log("Gscore of tile at index " + i.xIndex + ", " + i.yIndex + " is " + i.gScore);
                        openList.Add(i);
                    }
                }
            }
                  
        }

        Debug.Log("Closed list contains " + closedList.Count + " entries.");
        Debug.Log("Open list contains " + openList.Count + " entries.");

        List<Point> path = Reconstruct(start, end);

        //foreach (Tile tile in closedList)
        //{
        //    tiles[tile.xIndex, tile.yIndex].GetComponent<Tile>().sprite = sprite;
        //    tiles[tile.xIndex, tile.yIndex].GetComponent<Tile>().isFloor = true;
        //    tiles[tile.xIndex, tile.yIndex].GetComponent<Tile>().Setup();
        //}

        foreach (Point p in path)
        {
            tiles[p.X, p.Y].GetComponent<Tile>().sprite = sprite;
            tiles[p.X, p.Y].GetComponent<Tile>().isFloor = true;
            tiles[p.X, p.Y].GetComponent<Tile>().Setup();
        }
    }

    List<Point> Reconstruct(Tile start, Tile end)
    {
        List<Point> indexes = new List<Point>();
        bool constructed = false;
        indexes.Add(new Point(end.xIndex, end.yIndex));
        Debug.Log("End parent is tile " + end.parent.xIndex + ", " + end.parent.yIndex);
        Tile next = tiles[tiles[end.xIndex, end.yIndex].GetComponent<Tile>().parent.xIndex, tiles[end.xIndex, end.yIndex].GetComponent<Tile>().parent.yIndex].GetComponent<Tile>();       
        bool error = false;
        float timer = 0;


        while (!constructed && !error)
        {
            timer += 0.0001f;
            if (timer > 1)
            {
                Debug.Log("Error: ALgorithm took more than 10000 ticks");
                error = true;
            }

            Debug.Log("Adding tile " + next.xIndex + ", " + next.yIndex + " to path.");
            indexes.Add(new Point(next.xIndex, next.yIndex));
            Debug.Log("Next parent is tile " + next.parent.xIndex + ", " + next.parent.yIndex);
            int x = next.parent.xIndex;
            int y = next.parent.yIndex;
            next = tiles[x, y].GetComponent<Tile>();
            if (next.xIndex == start.xIndex && next.yIndex == start.yIndex)
            {
                constructed = true;
            }
        }

        return indexes;
    }
}
