using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder
{
    Dictionary<GameObject, bool> closedSet = new Dictionary<GameObject, bool>();
    Dictionary<GameObject, bool> openSet = new Dictionary<GameObject, bool>();
    Dictionary<GameObject, int> gScore = new Dictionary<GameObject, int>();
    Dictionary<GameObject, int> fScore = new Dictionary<GameObject, int>();
    Dictionary<GameObject, GameObject> nodeLinks = new Dictionary<GameObject, GameObject>();

    public List<GameObject> FindPath(GameObject[,] tiles, GameObject start, GameObject end)
    {
        openSet[start] = true;
        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);

        while (openSet.Count > 0)
        {
            var current = nextBest();
            if (current.Equals(end))
            {
                return Reconstruct(current);
            }

            openSet.Remove(current);
            closedSet[current] = true;

            foreach (var neighbor in Neighbors(tiles, current))
            {
                if (closedSet.ContainsKey(neighbor))
                {
                    continue;
                }

                var projectedG = GetGScore(current) + 1;

                if (!openSet.ContainsKey(neighbor))
                {
                    openSet[neighbor] = true;
                }
                else if (projectedG >= GetGScore(neighbor))
                {
                    continue;
                }

                nodeLinks[neighbor] = current;
                gScore[neighbor] = projectedG;
                fScore[neighbor] = projectedG + Heuristic(neighbor, end);
            }
        }

        List<GameObject> path = new List<GameObject>();
        foreach (KeyValuePair<GameObject, GameObject> i in nodeLinks)
        {
            path.Add(i.Value);
        }

        return path;
    }

    private int Heuristic(GameObject start, GameObject end)
    {
        int dx = (int)(end.GetComponent<Tile>().position.x - start.GetComponent<Tile>().position.x);
        int dy = (int)(end.GetComponent<Tile>().position.y - start.GetComponent<Tile>().position.y);
        return Mathf.Abs(dx) + Mathf.Abs(dy);
    }

    private int GetGScore(GameObject pt)
    {
        int score = int.MaxValue;
        gScore.TryGetValue(pt, out score);
        return score;
    }

    private int GetFScore(GameObject pt)
    {
        int score = int.MaxValue;
        fScore.TryGetValue(pt, out score);
        return score;
    }

    public static IEnumerable<GameObject> Neighbors(GameObject[,] graph, GameObject center)
    {
        GameObject pt = new GameObject();
        pt.transform.position = new Vector3(center.transform.position.x - 1, center.transform.position.y - 1, 0);
        if (IsValidNeighbor(graph, pt))
        {
            yield return pt;
        }

        pt.transform.position = new Vector3(center.transform.position.x, center.transform.position.y - 1, 0);
        if (IsValidNeighbor(graph, pt))
        {
            yield return pt;
        }

        pt.transform.position = new Vector3(center.transform.position.x - 1, center.transform.position.y, 0);
        if (IsValidNeighbor(graph, pt))
        {
            yield return pt;
        }

        pt.transform.position = new Vector3(center.transform.position.x + 1, center.transform.position.y, 0);
        if (IsValidNeighbor(graph, pt))
        {
            yield return pt;
        }

        pt.transform.position = new Vector3(center.transform.position.x - 1, center.transform.position.y + 1, 0);
        if (IsValidNeighbor(graph, pt))
        {
            yield return pt;
        }

        pt.transform.position = new Vector3(center.transform.position.x, center.transform.position.y + 1, 0);
        if (IsValidNeighbor(graph, pt))
        {
            yield return pt;
        }

        pt.transform.position = new Vector3(center.transform.position.x + 1, center.transform.position.y + 1, 0);
        if (IsValidNeighbor(graph, pt))
        {
            yield return pt;
        }
    }

    public static bool IsValidNeighbor(GameObject[,] matrix, GameObject pt)
    {
        int x = (int)pt.transform.position.x;
        int y = (int)pt.transform.position.y;
        if (x < 0 || x >= matrix.Length)
        {
            return false;
        }

        if (y < 0 || y >= matrix.Length)
        {
            return false;
        }

        return true;
    }

    private List<GameObject> Reconstruct(GameObject current)
    {
        List<GameObject> path = new List<GameObject>();
        while(nodeLinks.ContainsKey(current))
        {
            path.Add(current);
            current = nodeLinks[current];
        }

        path.Reverse();
        return path;
    }

    private GameObject nextBest()
    {
        int best = int.MaxValue;
        GameObject bestPt = null;
        foreach (var node in openSet.Keys)
        {
            var score = GetFScore(node);
            if (score < best)
            {
                bestPt = node;
                best = score;
            }
        }

        return bestPt;
    }
}