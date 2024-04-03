using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using RoverJobs;

namespace RoverTasks
{ 
    public class Task
    {
        public static LinkedList<Task> Pool { get => TaskManager.AllTasks; }

        public Rover rover;

        public List<Job> jobs;

        public bool isComplete = false;

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
                new CollectAndDeliverResources(ghost.structureData.requiredResources, ghost.position),
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