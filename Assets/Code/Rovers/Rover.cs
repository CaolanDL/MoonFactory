
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using RoverJobs;
using UnityEngine;
using UnityEngine.UIElements;

public enum RoverModule
{
    None,
    Logistics,
    Construction,
    Mining
}

public class Rover
{
    public const float _MoveSpeed = 8f;
    public float MoveSpeed { get { return _MoveSpeed; } }

    public const float _TurnSpeed = 12f;
    public float TurnSpeed { get { return _TurnSpeed; } }

    public const float _CollectSpeed = 6f;
    public float CollectSpeed { get { return _CollectSpeed; } }

    public const float _MiningSpeed = 6f;
    public float MiningSpeed { get { return _MiningSpeed; } }


    public static List<Rover> Pool = new();

    public Inventory Inventory = new();
    public RoverModule Module = RoverModule.Construction;

    public Task ActiveTask;
    public readonly Queue<Job> JobQueue = new Queue<Job>();
    public readonly Stack<Job> JobStack = new Stack<Job>();

    public SmallTransform SmallTransform = new();
    // Add tiny transform for grid operations without float rounding

    public float2 position
    {
        get => SmallTransform.position;
        set => SmallTransform.position = value;
    }
    public float rotation
    {
        get => SmallTransform.rotation;
        set => SmallTransform.rotation = value;
    }

    public DisplayObject DisplayObject;

    public void Init(int2 spawnLocation, DisplayObject displayObject)
    {
        this.DisplayObject = displayObject;
    }

    public void OnFrameUpdate() { }

    public void Tick()
    {
        HandleJobs();

        UpdateDOPosition();
    }

    public void UpdateDOPosition()
    {
        DisplayObject.transform.position = new Vector3(position.x, 0, position.y);
    }

    public void UpdateDoRotation()
    {
        DisplayObject.transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    // Task & Job Management //

    void HandleJobs()
    {
        if (JobStack.Count > 0)
        {
            Job job = JobStack.Peek();

            if (job.lifeSpan < 0) job.Start();

            if (ActiveTask != null) job.Tick();

            return;
        }

        if (JobQueue.Count == 0)
        {
            EnqueueJob(new FetchTask());

            return;
        }

        if (JobStack.Count == 0 && JobQueue.Count > 0)
        {
            StackJob(JobQueue.Dequeue());
            //Debug.Log($"Stacked job {JobStack.Peek()}");
            return;
        }
    }

    public void StackJob(Job job)
    {
        job.rover = this;
        JobStack.Push(job);
    }


    public void PopJob()
    {
        JobStack.Pop();
    }

    public void EnqueueJob(Job job)
    {
        job.rover = this;
        JobQueue.Enqueue(job);
    }

    public void TaskFailed()
    {
        /*        var taskType = ActiveTask.GetType();

                if (taskType == typeof(BuildStructureTask)) TaskManager.QueueTask(ActiveTask as BuildStructureTask);
                else if (taskType == typeof(LogisticsTasks)) TaskManager.QueueTask(ActiveTask as LogisticsTasks);
                else if (taskType == typeof(MiningTasks)) TaskManager.QueueTask(ActiveTask as MiningTasks);
                else { throw new Exception("task had no type"); }*/

        TaskManager.QueueTask(ActiveTask);
        ActiveTask.rover = null;
        ActiveTask = null;
        JobQueue.Clear();
        JobStack.Clear();
    }

    public void TaskFinished()
    {
        ActiveTask = null;
        JobQueue.Clear();
        JobStack.Clear();
    }


    // Actions //

    void Goto()
    {

    }


    // Modules // 

    public void RemoveModule()
    {
        if (Module == RoverModule.None) return;
        Module = RoverModule.None;
    }

    public void SetModule(RoverModule module)
    {
        if (Module != RoverModule.None) { return; }

        if (Module != module)
        {
            Module = module;
            UpdateModuleModel();
        }
    }

    void UpdateModuleModel()
    {
        switch (Module)
        {
            case RoverModule.None:
                break;

            case RoverModule.Logistics:
                break;

            case RoverModule.Construction:
                break;

            case RoverModule.Mining:
                break;
        }
    }
}

public class Widget : Rover
{

}