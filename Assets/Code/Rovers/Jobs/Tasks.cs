using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using RoverJobs;
using System;

namespace RoverTasks
{  
    public enum TaskCategory
    {
        None,
        All,
        Construction,
        Logistics,
        Mining
    }

    public class Task
    {
        public static LinkedList<Task> Pool { get => TaskManager.Tasks; }

        public Rover rover;

        public TaskCategory Category = TaskCategory.None;

        public List<Job> jobs;

        public bool isComplete = false; 
        public bool isCancelled = false;

        public Action OnFetched;
        public Action OnFailed;

        public Action OnCompleteCallback;
        public Action OnCancelledCallback;

        public void BuildJobs()
        {
            jobs = SetJobs();

            if (jobs != null)
            {
                EnqueueJobs(jobs);
            }
        }

        public virtual List<Job> SetJobs()
        {
            return null;
        } 

        public void EnqueueJob(Job job)
        {
            rover.EnqueueJob(job);
        }

        public void EnqueueJobs(List<Job> jobs)
        {
            foreach (var job in jobs)
            {
                EnqueueJob(job);
            }
        } 
    }

    public class ManagedTask
    {
        public Task linkedTask = null; 
        public bool taskExists = false;

        public bool onRover = false;

        public Action OnTaskAvailable;
        public Action OnTaskComplete;

        public void TryCreateTask(Task task)
        {
            if (linkedTask != null) return;

            linkedTask = task;
            taskExists = true;
            TaskManager.QueueTask(task);

            linkedTask.OnCompleteCallback += OnComplete;
            linkedTask.OnCancelledCallback += OnCancelled;

            linkedTask.OnFetched += OnFetched;
            linkedTask.OnFailed += OnFailed;
        }

        public void CancelTask()
        {
            if (!taskExists) return;

            linkedTask.isCancelled = true;
            TaskManager.CancelTask(linkedTask);

            OnCancelled();
        }

        public void OnComplete()
        {
            linkedTask = null;
            taskExists = false;
            onRover = false;

            OnTaskAvailable?.Invoke();
            OnTaskComplete?.Invoke();   
        }

        public void OnCancelled()
        {
            linkedTask = null;
            taskExists = false;
            onRover = false;

            OnTaskAvailable?.Invoke();
        }

        public void OnFetched()
        {
            onRover = true;
        }

        public void OnFailed()
        {
            onRover = false;
        }
    }

    // Debugging Tasks Parent Class

    public class DebuggingTask : Task { }
     
    public class AutoPathfind : DebuggingTask
    {
        public override List<Job> SetJobs()
        { 
            var origin = (int2)rover.SmallTransform;
            var destination = (int2)rover.SmallTransform + new int2(20, 20);
            var path = PathFinder.FindPath(origin, destination);

            if (path == null) { rover.TaskFailed(); Debug.Log("Auto Path Failed"); return null; }

            return new List<Job>
            {
                 new TraversePath(path)
            };
        }
    }

     
    // Construction Tasks Parent Class
    public class ConstructionTask : Task
    {
        public ConstructionTask()
        {
            Category = TaskCategory.Construction;
        } 
    }
     
    //TODO Modify collect and deliver resources to reserve resources in hoppers.
    //TODO Stretch Goal: This task should attempt to collect the resources necessary to build more than one structure if possible. 
    public class BuildStructureTask : ConstructionTask
    {
        StructureGhost ghost;

        public BuildStructureTask(StructureGhost ghost)
        {
            this.ghost = ghost; 
        }

        public override List<Job> SetJobs()
        {
            return new()
            {
                new CollectDeliverExactly(ghost.structureData.requiredResources, ghost.position),
                new BuildStructure(ghost),
                new FinishTask()
            };
        }
    }
     
    public class DemolishStructureTask : ConstructionTask
    {
        public Structure structure; 

        public DemolishStructureTask(Structure structure)
        {
            this.structure = structure;  
        }

        public override List<Job> SetJobs()
        {
            return new()
            {
                new GotoEntity(structure),
                new DemolishStructure(structure),
                new FinishTask()
            };
        }
    }


    // Logistics Tasks Parent Class
    public class LogisticsTask : Task
    {
        public LogisticsTask()
        {
            Category = TaskCategory.Logistics;
        }
    }
    
    public class SoftRequestResourceTask : LogisticsTask
    {
        ResourceData resource;
        int quantity;
        int2 destination;

        public SoftRequestResourceTask(ResourceData resource, int quantity, int2 destination)
        {
            this.resource = resource;
            this.quantity = quantity; 
            this.destination = destination;
        }

        public override List<Job> SetJobs()
        {
            return new()
            {
                new CollectAndDeliverAsManyAsPossible(new ResourceQuantity(resource, quantity), destination), 
                new FinishTask()
            };
        }
    }  

    // Mining Tasks Parent Class
    public class MiningTask : Task
    {
        public MiningTask()
        {
            Category = TaskCategory.Mining;
        }
    }
     
    public class DestroyMeteorTask : MiningTask
    {

    }

}