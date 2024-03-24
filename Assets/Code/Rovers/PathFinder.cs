using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Path
{
    public int2[] nodes;

    public int length;

    public Path(int2[] nodes)
    {
        this.nodes = nodes;
        length = nodes.Length;
    }

    public void Compress()
    {
        List<int2> newPath = nodes.ToList();

        for (int i = 0; i < newPath.Count; i++)
        {
            if (i == 0) { continue; }

            if (newPath[i].x == newPath[i - 1].x)
            {
                newPath.RemoveAt(i);
            }
            if (newPath[i].y == newPath[i - 1].y)
            {
                newPath.RemoveAt(i);
            }
        }

        nodes = newPath.ToArray();
        length = newPath.Count;
    }
} 


public static class PathFinder
{
    private static Grid _worldGrid;

    //static PathFinder() { GameWorld.WorldInstanciated += () => _worldGrid = GameManager.Instance.gameWorld.worldGrid; }

    public static LinkedList<Path> FindSuperPath(int2 origin, int2[] destinations)
    {
        LinkedList<Path> superPath = new();

        int2 lastDestination = origin;

        for (int i = 0; i < destinations.Length; i++)
        {
            Path subPath = FindPathToAnyFreeNeighbor(lastDestination, destinations[i]);

            if (subPath == null) { return null; } // No path found;

            //if (subPath.length > 8) { subPath.Compress(); } // Should add a static variable. Needs perf test to determine path length where compression becomes beneficial. 

            lastDestination = destinations[i];

            superPath.AddLast(subPath);
        }

        return superPath;
    }

    public static Path FindPathToAnyFreeNeighbor(int2 origin, int2 destination)
    {
        _worldGrid = GameManager.Instance.gameWorld.worldGrid;

        Location location = _worldGrid.GetLocationAt(destination);

        var neighbors = location.GetNeighbors(); 

        foreach (var neighbor in neighbors)
        {
            if(neighbor == null) { continue; }
            if(neighbor.entity == null || neighbor.entity.GetType() != typeof(Structure))
            {
                return FindPath(origin, neighbor.position);
            }
        }

        return null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static Path FindPath(int2 origin, int2 destination)
    {
        _worldGrid = GameManager.Instance.gameWorld.worldGrid;

        var originLocation = _worldGrid.GetLocationAt(origin);
        var destinationLocation = _worldGrid.GetLocationAt(destination);
        
        //Debug.Log($"Finding Path from {origin} to {destination}");

        if (destinationLocation == null) { return null; }
        if (originLocation == null) { return null; }

        Queue<Location> frontier = new();
        frontier.Enqueue(originLocation);

        Dictionary<Location, Location> cameFrom = new();
        cameFrom.Add(originLocation, null);

        Location current;
        var neighbors = new Location[4];

        // Basic BFS implementation
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            
            if (current.position.Equals(destination)) break; 
            
            current.GetNeighbors().CopyTo(neighbors, 0);
            
            foreach (Location neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (cameFrom.ContainsKey(neighbor)) continue;

                // Block path conditions ->
                if(neighbor.entity != null && neighbor.entity.GetType().IsSubclassOf(typeof(Structure))) continue; 
                // <-

                frontier.Enqueue(neighbor);
                cameFrom.Add(neighbor, current);
            }
        }

        // Reconstruct Path
        current = destinationLocation;
        var path = new List<int2>(64); 

        while(current != originLocation)
        { 
            path.Add(current.position);
            current = cameFrom[current];
        }

        path.Add(origin);
        path.Reverse();

        Debug.Log($"path length: {path.Count}");

        return new Path(path.ToArray());
        
        //return new Path(new int2[] { origin, destination }); // Just return a path that only consists of the start and end nodes.
    }

    private class Node
    {
        
    }
} 