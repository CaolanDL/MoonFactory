using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using RoverJobs;
using System;

namespace RoverTasks
{ 
    public class Task
    {
        public static LinkedList<Task> Pool { get => TaskManager.AllTasks; }

        public Rover rover;

        public List<Job> jobs;

        public bool isComplete = false;

        public bool isCancelled = false;

        public Action OnFetched;
        public Action OnFailed;

        public Action OnCompleteCallback;
        public Action OnCancelledCallback;

        public void BuildJobs()
        {
            if (jobs != null)
            {
                EnqueueJobs(jobs);
            }
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

        public void TryCreateTask(Task task)
        {
            if (linkedTask != null) return;

            linkedTask = task;
            taskExists = true;
            TaskManager.QueueTask(task);

            linkedTask.OnCompleteCallback += OnComplete;
            linkedTask.OnCancelledCallback += OnCancelled;

            linkedTask.OnFetched += OnFetched;
            linkedTask.OnFailed += OnFetched;
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

    // Debugging Tasks

    public class DebuggingTask : Task { }


    public class AutoPathfind : DebuggingTask
    {
        public AutoPathfind()
        {
            var origin = (int2)rover.SmallTransform;
            var destination = (int2)rover.SmallTransform + new int2(20, 20);
            var path = PathFinder.FindPath(origin, destination);

            if (path == null) { rover.TaskFailed(); Debug.Log("Auto Path Failed"); return; }

            jobs = new List<Job>
            {
                 new TraversePath(path)
            };
        }
    }

     
    // todo Bad smell here. Need to inherit the shared task management functionality. Use type passing for filtration.
    // todo These should be considered managers and not tasks. You are ignoring single resposibility principle.
    // Construction Tasks
    public class ConstructionTasks : Task
    {
        public static LinkedList<ConstructionTasks> constructionTasks = new();  //{ get => TaskManager.ConstructionTasks; } // Temporarily Mapped to Task Manager, should be migrated to be self contained.

        public static void QueueTask(ConstructionTasks constructionTask)
        {
            constructionTasks.AddLast(constructionTask);
        }

        public static ConstructionTasks PopTask()
        {
            if (constructionTasks.Count == 0) return null;
            var task = constructionTasks.First;
            constructionTasks.RemoveFirst();
            TaskManager.AllTasks.Remove(task.Value);
            return task.Value;
        }

        public static void CancelTask(ConstructionTasks constructionTask)
        {
            constructionTasks.Remove(constructionTask);
        }

        public static void Clear()
        {

        }
    }


    //TODO Modify collect and deliver resources to reserve resources in hoppers.
    //TODO Stretch Goal: This task should attempt to collect the resources necessary to build more than one structure if possible. 
    public class BuildStructureTask : ConstructionTasks
    {
        StructureGhost ghost;

        public BuildStructureTask(StructureGhost ghost)
        {
            this.ghost = ghost;

            jobs = new()
            {
                new CollectAndDeliverShoppingList(ghost.structureData.requiredResources, ghost.position),
                new BuildStructure(ghost),
                new FinishTask()
            };
        }
    }


    public class DemolishStructureTask : ConstructionTasks
    {
        public Structure structure; 

        public DemolishStructureTask(Structure structure)
        {
            this.structure = structure; 

            jobs = new()
            {
                new GotoEntity(structure),
                new DemolishStructure(structure),
                new FinishTask()
            };
        } 
    }


    // Logistics Tasks
    public class LogisticsTasks : Task { }


    public class HopperRequestTask : LogisticsTasks
    {
        ResourceData resource;

        public HopperRequestTask(ResourceData resource)
        {
            this.resource = resource;

            jobs = new()
            {

                new FinishTask()
            };
        } 
    }


    public class ResearchSampleRequestTask : LogisticsTasks
    {

    }


    // Mining Tasks
    public class MiningTasks : Task { }


    public class DestroyMeteorTask : MiningTasks
    {

    }

}