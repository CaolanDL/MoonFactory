
using RoverJobs;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

public class TaskManager
{
    public static List<Task> allTasks;

    public static List<ConstructionTask> constructionTasks;

    public static List<LogisticsTask> logisticsTasks;

    public static List<MiningTask> miningTasks;

    public TaskManager()
    {
        GameManager.OnGameExit += () =>
        {
            allTasks.Clear();
            constructionTasks.Clear();
            logisticsTasks.Clear();
            miningTasks.Clear();
        };
    }

    static Type ConstructionTaskType = typeof(ConstructionTask);
    static Type LogisticsTaskType = typeof(LogisticsTask);
    static Type MiningTaskType = typeof(MiningTask);

    public static void AddTask(Task task)
    {
        allTasks.Add(task);

        var taskType = task.GetType();

        if (taskType == ConstructionTaskType)
        {
            constructionTasks.Add((ConstructionTask)task);
        }
        else if (taskType == LogisticsTaskType)
        {
            logisticsTasks.Add((LogisticsTask)task);
        }
        else if (taskType == MiningTaskType)
        {
            miningTasks.Add((MiningTask)task);
        }
    }
}

public class Task
{
    public Rover rover;

    public bool isComplete = false;

    public virtual void BuildJobs() { }

    public void EnqueueJob(Job job)
    {
        rover.JobQueue.Enqueue(job);
    }

    public void EnqueueJobs(List<Job> jobs)
    {
        foreach (var job in jobs)
        {
            rover.JobQueue.Enqueue(job);
        }
    }
}

// Construction Tasks
public class ConstructionTask : Task { }

public class BuildStructure : ConstructionTask
{
    public int2 ghostLocation;
    public StructureData structureData;

    public BuildStructure(StructureGhost ghost)
    {
        this.ghostLocation = ghost.position;
        this.structureData = ghost.structureData;
    }

    public override void BuildJobs()
    {
        List<Job> jobs = new List<Job>
        { 
        };
        EnqueueJobs(jobs);
    }
}

public class DemolishStructure : ConstructionTask
{
    public int2 structureLocation;
    public StructureData structureData;
}

// Logistics Tasks
public class LogisticsTask : Task { }

public class HopperRequest : LogisticsTask
{

}

public class ResearchSampleRequest : LogisticsTask
{

}

// Mining Tasks
public class MiningTask : Task { }

public class DestroyMeteor : MiningTask
{

}

