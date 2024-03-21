using ExtensionMethods;
using Logistics;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;

public class PathFinder
{
    public static Path FindPath(int2 origin, int2 destination)
    {
        // A* Algorith here

        return new Path(new int2[] { origin, destination }, 2); // Just return a path that only consists of the start and end nodes.
    }
}

public class Path
{
    public int2[] nodes;

    public int length;

    public Path(int2[] nodes, int length)
    {
        this.nodes = nodes;
        this.length = length;
    }

    public void CompressPath()
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
    }
}

namespace RoverJobs
{
    public class Job
    {
        public Rover rover;

        public ushort lifeSpan = 0;

        public bool isComplete = false;

        public void Tick()
        {
            lifeSpan++;
            OnTick();
        }

        public virtual void OnTick()
        {

        }

        public void StackJob(Job job)
        {
            rover.JobStack.Push(job);
        }

        public void QueueJob(Job type)
        {
            rover.JobQueue.Enqueue(type);
        }
    }

    public class FetchTask : Job
    {
        public FetchTask() { }

        public override void OnTick()
        {
            rover.JobStack.Pop();

            if (DevFlags.RoverTaskOverrideToPathfind)
            {
                StackJob(new FindPath((int2)rover.smallTransform, (int2)rover.smallTransform + new int2(0, 20)));
                return;
            }

            switch (rover.Module)
            {
                case RoverModule.None: break;

                case RoverModule.Construction:
                    rover.activeTask = TaskManager.constructionTasks.Last();
                    break;

                case RoverModule.Logistics:
                    rover.activeTask = TaskManager.logisticsTasks.Last();
                    break;

                case RoverModule.Mining:
                    rover.activeTask = TaskManager.miningTasks.Last();
                    break;
            }

            if (rover.activeTask != null)
            {
                rover.activeTask.BuildJobs();
            }
        }
    }

    public class TraversePath : Job
    {
        public Path path;

        ushort currentNodeIndex = 0;

        float2 currentNodeWorldPosition;
        float2 nextNodeWorldPosition;

        float interpolant = 0;

        float distanceToNextNode = 0;

        public TraversePath(Path path)
        {
            this.path = path;

            currentNodeWorldPosition = path.nodes[0];
            nextNodeWorldPosition = path.nodes[1];

            distanceToNextNode = Float2Extensions.DistanceBetween(currentNodeWorldPosition, nextNodeWorldPosition);
        }

        public override void OnTick()
        {
            UpdatePosition();
        }

        void UpdatePosition()
        {
            interpolant += Rover.moveSpeed / distanceToNextNode;

            rover.position = (currentNodeWorldPosition * interpolant) + (nextNodeWorldPosition * (1 - interpolant)); // Lerp between current node and next node;

            if (interpolant >= 1)
            {
                AdvanceNode();
            } 
        }

        void AdvanceNode()
        {
            if(currentNodeIndex + 1 >= path.length) { Finished(); return; }

            currentNodeWorldPosition = nextNodeWorldPosition;
            nextNodeWorldPosition = path.nodes[currentNodeIndex + 1];

            distanceToNextNode = Float2Extensions.DistanceBetween(currentNodeWorldPosition, nextNodeWorldPosition);

            currentNodeIndex++; 
        }

        void Finished()
        {
            rover.JobStack.Pop();
        }
    }

    public class CollectAndDeliverResources : Job
    {
        SortedList<float, Hopper> foundHoppers = new SortedList<float, Hopper>();

        List<int2[]> pathSegments = new();

        List<ResourceQuantity> resourceQuantities = new();

        public CollectAndDeliverResources(ResourceQuantity[] resourceQuantities, int2 destination)
        {
            this.resourceQuantities = resourceQuantities.ToList();
        }

        public override void OnTick()
        {
            if (!TryFindHoppers()) return;

            TryFindSuperPath();
        }

        void TryFindSuperPath() // Should return bool
        {
            int2 nodeLocation = (int2)rover.position;

            for(int i = 0; i < foundHoppers.Count; i++)
            {
                Path path = PathFinder.FindPath(nodeLocation, foundHoppers.Values[i].position);

                if(path.length > 8) { path.CompressPath(); } // Should add a static variable. Needs perf test to determine path length where compression becomes beneficial.


            }
        }

        bool TryFindHoppers()
        {
            bool foundResourceInHopper = false;

            float2 hopperRoverOffset;

            float hopperDistanceMagnitude;

            foreach (var resourceQuantity in resourceQuantities)
            {
                foreach (var hopper in Hopper.pool)
                {
                    if (hopper.storageInventory.GetQuantityOf(resourceQuantity.resource) >= resourceQuantity.quantity)
                    {
                        if (foundHoppers.ContainsValue(hopper))
                        {
                            foundResourceInHopper = true;
                            break;
                        }

                        hopperRoverOffset = rover.position - hopper.position;

                        hopperDistanceMagnitude = (hopperRoverOffset.x * hopperRoverOffset.x) + (hopperRoverOffset.y * hopperRoverOffset.y);

                        foundHoppers.Add(hopperDistanceMagnitude, hopper);

                        foundResourceInHopper = true;
                        break;
                    }
                }

                if (foundResourceInHopper == false)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class CollectResource : Job
    {

    }

    public class DeliverResource : Job
    {

    }
}

