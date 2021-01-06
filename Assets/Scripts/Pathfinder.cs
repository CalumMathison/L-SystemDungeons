using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder
{
    class Location
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Location Parent;
    }

    static int ComputeHScore(int x, int y, int targetX, int targetY)
    {
        return Mathf.Abs(targetX - x) + Mathf.Abs(targetY - y);
    }

    public void Finder()
    {
        Location current = null;
        Location start = new Location { X = 0, Y = 0 };
        Location target = new Location { X = 1, Y = 1 };
        List<Location> openList = new List<Location>();
        List<Location> closedList = new List<Location>();
        int g = 0;
        openList.Add(start);


        while (openList.Count > 0)
        {
            var lowest = openList.Min(l => l.F);
            current = openList.First(l => l.F == lowest);

            closedList.Add(current);

            openList.Remove(current);


        }
    }
}
