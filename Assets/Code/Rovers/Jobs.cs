
using Logistics;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

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

    public class FindPath : Job
    {
        public int2 destination;

        public List<int2> path;

        public FindPath(int2 origin, int2 destination)
        {
            this.destination = destination;


        }
    }

    public class TraversePath : Job
    {
        public int2[] path;

        ushort pathNodeIndex = 0;

        public TraversePath(int2[] path)
        {
            this.path = path;
        }

        int2[] CompressPath()
        {
            List<int2> newPath = path.ToList();

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

            return newPath.ToArray();
        }

        void UpdatePosition()
        {

        }
    }

    public class CollectAndDeliverResources : Job
    { 
        SortedList<float, Hopper> foundHoppers = new SortedList<float, Hopper>();

        List<int2[]> pathSegments;

        List<ResourceQuantity> resourceQuantities = new();

        public CollectAndDeliverResources(ResourceQuantity[] resourceQuantities, int2 destination)
        {
            this.resourceQuantities = resourceQuantities.ToList();
        }

        public override void OnTick()
        {
            
        }

        bool TryFindHoppers()
        {
            bool foundResourceInHopper = false;

            float2 hopperRoverOffset

            float hopperDistanceMagnitude

            foreach (var resourceQuantity in resourceQuantities)
            {
                foreach (var hopper in Hopper.pool)
                {
                    if (hopper.storageInventory.GetQuantityOf(resourceQuantity.resource) >= resourceQuantity.quantity)
                    {
                        hopperRoverOffset = rover.position - hopper.position;

                        hopperDistanceMagnitude = (hopperRoverOffset.x * hopperRoverOffset.x) + (hopperRoverOffset.y * hopperRoverOffset.y);

                        foundHoppers.Add(hopperDistanceMagnitude, hopper);
                    }
                }

                if(foundResourceInHopper == false)
                {
                    return false;
                }
            } 
        }
    } 

    public class CollectResource : Job
    {

    }

    public class DeliverResource : Job
    {

    }
}

