using Logistics;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Utils;
//using static UnityEditor.ShaderData;

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


public static class PathFinder
{
    private static Grid _worldGrid;

    public static Path FindPath(int2 origin, int2 destination)
    {
        _worldGrid = GameManager.Instance.GameWorld.worldGrid;

        var originLocation = _worldGrid.GetLocationAt(origin);
        var destinationLocation = _worldGrid.GetLocationAt(destination);

        if (destinationLocation == null) { return null; }
        if (originLocation == null) { return null; }

        
        PriorityQueue<Location, float> frontier = new(); // lhs Location; rhs Cost;
        frontier.Enqueue(originLocation, 0);

        Dictionary<Location, Location> cameFrom = new(); // lhs Location; rhs The location that preceded it
        cameFrom.Add(originLocation, null);

        Dictionary<Location, float> accumulatedCost = new(); // lhs Location; rhs The total accumulated cost of that location by its predecessors
        accumulatedCost.Add(originLocation, 0);

        Location current;
        var neighbors = new Location[4];

        // Greedy search implementation
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            // Early exit if destination reached
            if (current.position.Equals(destination)) break;

            // Store a list of current locations neighbors
            current.GetNeighbors().CopyTo(neighbors, 0);

            // Increase cost as we get further from origin (Dijkstra’s  Algorithm)
            float newCost = accumulatedCost[current] + 1; 

            foreach (Location next in neighbors)
            {   
                if (next == null) continue;
                 
                if (cameFrom.ContainsKey(next)) continue;
                if (cameFrom.ContainsKey(next)) continue;

                // Block path conditions ->
                if (IsTraversable(next) == false) continue;
                // <-
                 
                // Apply hueristic
                var priority = Hueristic(next) + newCost;

                frontier.Enqueue(next, priority);
                cameFrom.Add(next, current);
                accumulatedCost.Add(next, newCost);
            }
        }

        float Hueristic(Location location)
        {
            return Mathf.Abs(location.position.x - destination.x) + Mathf.Abs(location.position.y - destination.y);
        }

        bool IsTraversable(Location location)
        {
            if (location.entity == null) return true;

            var entityType = location.entity.GetType();

            // Success Conditions
            if (entityType == typeof(Conveyor) ||
                entityType == typeof(PowerPylon)
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
        _worldGrid = GameManager.Instance.GameWorld.worldGrid;

        Location location = _worldGrid.GetLocationAt(destination);
        Location[] neighbors = location.GetNeighbors();
        List<Location> validNeighbors = new();
        Path path = null;

        for (int i = 0; i < 5; i++)
        { 
            PathValidationPass(i);
            if (path != null) return path;
        }  

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

                // find visually appealing rover distribution
                if (pass < 1)
                {
                    if (RoverManager.RoverPositions.ContainsValue(neighbor.position)) continue; // Is there a rover here?
                }

                // find locations without conveyors
                if (pass < 2)
                {
                    if (neighbor.entity != null)
                        if (neighbor.entity.GetType() == typeof(Conveyor)) continue;
                }

                // find for locations without ghosts
                if (pass < 4)
                {
                    if (neighbor.entity != null)
                        if (neighbor.entity.GetType() == typeof(StructureGhost)) continue;
                }

                // Last resort find any traversable locations 

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
}