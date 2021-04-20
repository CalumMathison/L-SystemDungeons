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
    public GameObject tilePrefab, doorPrefab;
    Pathfinder pathfinder;
    public GameObject start, end;
    bool[,] graph;
    UnityEngine.Color color;
    private GameObject[] doors;

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
                tiles[x, y].GetComponent<Collider2D>().enabled = true;
                graph[x, y] = true;
            }
        }

        List<Tile> originPoints = new List<Tile>();

        if (nodes.Count < 1)
        {
            Debug.LogError("Problem with nodes");
        }

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                for (int i = 0; i < nodes.Count - 1; i++)
                {
                    if (tiles[x, y].GetComponent<Collider2D>().bounds.Contains(nodes[i].positon))
                    {
                        int nx, ny;
                        nx = x + (int)Random.Range(0, nodes[i].width);
                        ny = y + (int)Random.Range(0, nodes[i].height);

                        if (nx > width)
                        {
                            nx = 140;
                        }

                        if (ny > height)
                        {
                            nx = 140;
                        }

                        originPoints.Add(tiles[nx, ny].GetComponent<Tile>());
                    }
                }
            }
        }

        foreach (Node n in nodes)
        {
            Tile c = null;
            Tile p = null;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y].GetComponent<Collider2D>().bounds.Contains(n.positon))
                    {
                        int nx, ny;
                        nx = x + (int)Random.Range(0, n.width - 1);
                        ny = y + (int)Random.Range(0, n.height - 1);

                        if (nx > width)
                        {
                            nx = 140;
                        }

                        if (ny > height)
                        {
                            nx = 140;
                        }

                        c = tiles[nx, ny].GetComponent<Tile>();
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (n.Parent != null)
                    {
                        int nx, ny;
                        nx = x + (int)Random.Range(0, n.Parent.width - 1);
                        ny = y + (int)Random.Range(0, n.Parent.height - 1);

                        if (nx > width)
                        {
                            nx = 140;
                        }

                        if (ny > height)
                        {
                            nx = 140;
                        }

                        if (tiles[x, y].GetComponent<Collider2D>().bounds.Contains(n.Parent.positon))
                        {
                            p = tiles[nx, ny].GetComponent<Tile>();
                        }
                    }
                }
            }

            if (c != null && p != null)
            {
                MakePath(c, p);
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
                        Debug.LogError("Tile pos: " + tiles[x, y].GetComponent<Tile>().position);
                        Debug.LogError("Node pos: " + n.positon);

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
                                    tiles[x + w, y + h].GetComponent<Collider2D>().enabled = false;
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
                                tiles[x + w, y].GetComponent<Collider2D>().enabled = false;
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
                                tiles[x, y + h].GetComponent<Collider2D>().enabled = false;
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
        //CalculateDoors();
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

    public void CalculateDoors()
    {
        doors = GameObject.FindGameObjectsWithTag("Door");
        if (doors.Length > 0)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                GameObject.Destroy(doors[i]);
            }
        }


        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (tiles[x, y].GetComponent<Tile>().sprite == sprite &&
                    tiles[x + 1, y].GetComponent<Tile>().sprite == wall &&
                    tiles[x - 1, y].GetComponent<Tile>().sprite == wall &&
                    ((tiles[x, y + 1].GetComponent<Tile>().sprite != wall && tiles[x, y + 1].GetComponent<Tile>().sprite != sprite) ||
                    (tiles[x, y - 1].GetComponent<Tile>().sprite != wall && tiles[x, y - 1].GetComponent<Tile>().sprite != sprite)))
                {
                    GameObject i = Instantiate(doorPrefab, tiles[x, y].transform.position, Quaternion.identity) as GameObject;
                }

                if (tiles[x, y].GetComponent<Tile>().sprite == sprite &&
                    tiles[x, y + 1].GetComponent<Tile>().sprite == wall &&
                    tiles[x, y - 1].GetComponent<Tile>().sprite == wall &&
                    ((tiles[x - 1, y].GetComponent<Tile>().sprite != wall && tiles[x - 1, y].GetComponent<Tile>().sprite != sprite) ||
                    (tiles[x + 1, y].GetComponent<Tile>().sprite != wall && tiles[x + 1, y].GetComponent<Tile>().sprite != sprite)))
                {
                    GameObject i = Instantiate(doorPrefab, tiles[x, y].transform.position, Quaternion.identity) as GameObject;
                }
            }
        }


    }

    void MakePath(Tile start, Tile end)
    {
        List<Tile> openList = new List<Tile>();
        Debug.Log("Added tile of index " + start.xIndex + ", " + start.yIndex + "to open list");
        openList.Add(tiles[start.xIndex, start.yIndex].GetComponent<Tile>());
        openList[0].gScore = 1;
        openList[0].hScore = 1 * (Mathf.Abs(openList[0].position.x - tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position.x) + Mathf.Abs(openList[0].position.y - tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position.y));
        openList[0].fScore = openList[0].gScore + openList[0].hScore;
        List<Tile> closedList = new List<Tile>();
        List<Tile> neighbours = new List<Tile>();
        Tile current = openList[0];
        bool pathfound = false;
        bool error = false;
      

        //f = Vector2.Distance(start.position, end.position);
        float timer = 0;
        //f = Vector2.Distance(tiles[start.xIndex, start.yIndex].GetComponent<Tile>().position, tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position);

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
                //Euclidean Distance
                //nh = Vector2.Distance(i.position, tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position);

                //Manhatten Distance
                //nh = (i.position.x - tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position.x) + (i.position.y - tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position.y);

                //nf = i.fScore;
                //f = current.fScore;

                if (i.fScore < current.fScore)
                {
                    current = i;
                }

                //current = openList.First<Tile>();
                //openList.Remove(current);

                //h = 0;
                // f = float.MaxValue;

            }
            if (current.xIndex == end.xIndex &&
                current.yIndex == end.yIndex)
            {
                end.parent = current.parent;
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
                    if (i.fScore < current.fScore)
                    {
                        Debug.Log("Added tile " + current.xIndex + ", " + current.yIndex + " as parent of tile " + i.xIndex + ", " + i.yIndex);
                        tiles[i.xIndex, i.yIndex].GetComponent<Tile>().parent = tiles[current.xIndex, current.yIndex].GetComponent<Tile>();
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
                        i.parent = current;
                        i.gScore = 1;

                        i.hScore = 1 * (Mathf.Abs(i.position.x - tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position.x) + 
                            Mathf.Abs(i.position.y - tiles[end.xIndex, end.yIndex].GetComponent<Tile>().position.y));

                        i.fScore = i.gScore + i.hScore;


                        Debug.Log("fscore of tile at index " + i.xIndex + ", " + i.yIndex + " is " + i.fScore);
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
            tiles[p.X, p.Y].GetComponent<Collider2D>().enabled = false;
        }
    }

    List<Point> Reconstruct(Tile start, Tile end)
    {
        List<Point> indexes = new List<Point>();
        bool constructed = false;
        indexes.Add(new Point(end.xIndex, end.yIndex));
        //Debug.Log("End parent is tile " + end.parent.xIndex + ", " + end.parent.yIndex);
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
            indexes.Add(new Point(next.xIndex, next.yIndex));
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
