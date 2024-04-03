using Logistics;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditor.ShaderData;

public class Path
{
    public int2[] nodes;

    /// <summary> When pathing to a free neighbor, this is the target location </summary>
    public int2 target;
    /// <summary> When pathing this is the ultimate destination </summary>
    public int2 destination;

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

public class SuperPath
{
}


public static class PathFinder
{
    private static Grid _worldGrid;
     
    public static LinkedList<Path> FindSuperPath(int2 origin, int2[] destinations)
    {
        LinkedList<Path> superPath = new();

        int2 lastDestination = origin;

        for (int i = 0; i < destinations.Length; i++)
        {
            Path subPath = FindPathToAnyFreeNeighbor(lastDestination, destinations[i]);

            if (subPath == null) return null;  // No path found;

            superPath.AddLast(subPath);

            lastDestination = superPath.Last().nodes.Last();
        }

        return superPath;
    }

    public static Path FindPathToAnyFreeNeighbor(int2 origin, int2 destination)
    {
        _worldGrid = GameManager.Instance.gameWorld.worldGrid;

        Location location = _worldGrid.GetLocationAt(destination);
        Location[] neighbors = location.GetNeighbors();
        List<Location> validNeighbors = new();
        Path path = null;

        // First pass to find visually appealing rover distribution
        PathValidationPass(0);
        if(path != null) return path;
        // Second pass to find locations without ghosts
        PathValidationPass(1);
        if (path != null) return path;
        // Last resort pass to find any traversable locations
        PathValidationPass(3);
        if (path != null) return path;


        /*        // First pass to find visually appealing rover distribution
                validNeighbors.Clear();
                foreach (var neighbor in neighbors)
                {
                    if (LocationValid(neighbor) == false) continue;

                    if (Rover.Pool.Exists(rover => rover.GridPosition.Equals(neighbor.position))) continue; // Is there a rover here?

                    validNeighbors.Add(neighbor);
                }
                if (TryFindPathToAny(validNeighbors)) return path;


                // Second pass to find locations without ghosts
                validNeighbors.Clear();
                foreach (var neighbor in neighbors)
                {
                    if (LocationValid(neighbor) == false) continue;

                    if (location.entity != null)
                        if (neighbor.entity.GetType() == typeof(StructureGhost)) continue;

                    validNeighbors.Add(neighbor);
                }
                if (TryFindPathToAny(validNeighbors)) return path;


                // Last resort pass to find any traversable locations
                validNeighbors.Clear();
                foreach (var neighbor in neighbors)
                {
                    if (LocationValid(neighbor) == false) continue;
                    validNeighbors.Add(neighbor);
                }
                if (TryFindPathToAny(validNeighbors)) return path;*/

        Path PathValidationPass(int pass)
        {
            validNeighbors.Clear();
            foreach (var neighbor in neighbors)
            {
                if (LocationValid(neighbor) == false) continue;

                // First pass to find visually appealing rover distribution
                if (pass == 0)
                {
                    if (RoverManager.RoverPositions.ContainsValue(neighbor.position)) continue; // Is there a rover here?
                }

                // Second pass to find locations without ghosts
                if (pass == 0 || pass == 1)
                {
                    if (neighbor.entity != null)
                        if (neighbor.entity.GetType() == typeof(StructureGhost)) continue;
                }

                // Last resort pass to find any traversable locations 

                validNeighbors.Add(neighbor);
            }
            if (TryFindPathToAny(validNeighbors)) return path;
            return null;
        }

        bool TryFindPathToAny(List<Location> locations)
        {
            foreach (var location in locations)
                if (TryFindPath(location)) return true;
            return false;
        }

        bool TryFindPath(Location location)
        {
            path = FindPath(origin, location.position);
            if (path == null) return false;

            path.target = destination;
            return true;
        }

        bool LocationValid(Location location)
        {
            if (location == null) return false;

            if (location.entity != null)
                if (location.entity.GetType() == typeof(Structure)) return false;

            return true;
        }

        return null;
    }


    public static Path FindPath(int2 origin, int2 destination)
    {
        _worldGrid = GameManager.Instance.gameWorld.worldGrid;

        var originLocation = _worldGrid.GetLocationAt(origin);
        var destinationLocation = _worldGrid.GetLocationAt(destination);

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
                if (IsTraversable(neighbor) == false) continue;
                // <-

                frontier.Enqueue(neighbor);
                cameFrom.Add(neighbor, current);
            }
        }

        bool IsTraversable(Location location)
        {
            if (location.entity == null) return true;

            var entityType = location.entity.GetType();

            // Success Conditions
            if (entityType == typeof(Conveyor)
                 )
            {
                return true;
            }

            // Fail Conditions
            if (entityType.IsSubclassOf(typeof(Structure))) return false;

            // Default to traversable if no conditions met
            return true;
        }


        // Reconstruct Path
        current = destinationLocation;
        var path = new List<int2>(64);

        while (current != originLocation)
        {
            path.Add(current.position);
            if (!cameFrom.ContainsKey(current)) return null;
            current = cameFrom[current];
        }

        path.Add(origin);
        path.Reverse();

        var newPath = new Path(path.ToArray());

        newPath.destination = destination;

        return newPath;
    }
}