using System;
using System.Collections.Generic;

using UnityEngine;
using Unity.Mathematics;

using RoverJobs;
using RoverTasks;
using Random = UnityEngine.Random;

public enum RoverModule
{
    None,
    Logistics,
    Construction,
    Mining,
    Widget
}


//? There is something going wrong when cancelling tasks that a rover is actively completing. 
//? The rover halts and blocks new tasks from being accepted by itself and other rovers.
//? Other rovers will begin to accept new tasks once an unkown number tasks have been added, but the issue rover will never move again.

public class Rover : Entity
{
    public const float _MoveSpeed = 8f;
    public float MoveSpeed { get { return _MoveSpeed; } }

    public const float _TurnSpeed = 12f;
    public float TurnSpeed { get { return _TurnSpeed; } }

    public const float _CollectSpeed = 6f;
    public float CollectSpeed { get { return _CollectSpeed; } }

    public const float _MiningSpeed = 6f;
    public float MiningSpeed { get { return _MiningSpeed; } }

    public Inventory Inventory = new();
    public RoverModule Module = RoverModule.Widget;

    public Task ActiveTask;
    public readonly Queue<Job> JobQueue = new Queue<Job>();
    public readonly Stack<Job> JobStack = new Stack<Job>();
    public bool JobWasPopped= false;
    public bool JobWasStacked = false;
    public bool TaskWasFailed = false;
    public int FetchDelay = 0;

    public static bool DebugEnabled = true;

    // Float transform for visual position
    public SmallTransform SmallTransform = new();
    // Tiny transform for grid position
    public TinyTransform TinyTransform = new();

    public float2 VisualPosition
    {
        get => SmallTransform.position;
        set => SmallTransform.position = value;
    }
    public float VisualRotation
    {
        get => SmallTransform.rotation;
        set => SmallTransform.rotation = value;
    }

    public int2 GridPosition
    {
        get => TinyTransform.position;
        set
        {
            TinyTransform.position = value;
            RoverManager.RoverPositions[this] = value;
            position = value;
        }
    }
    public sbyte GridRotation
    {
        get => TinyTransform.rotation;
        set => TinyTransform.rotation = value;
    }

    public DisplayObject DisplayObject;

    public void SetDisplayObject(DisplayObject displayObject)
    {
        this.DisplayObject = displayObject;
        DisplayObject.parentEntity = this;
    }

    public void SetPosition(int2 position)
    {
        GridPosition = position;
        VisualPosition = position; 
    }

    public void OnFrameUpdate() { }

    public void Tick()
    {
        HandleJobs();

        UpdateDOPosition();
    }

    public void Clicked(Vector3 mousePosition)
    {
        GameManager.Instance.HUDManager.OpenInterface(MenuData.Instance.RoverInterface, this, mousePosition);
    }

    public void UpdateDOPosition()
    {
        DisplayObject.transform.position = new Vector3(VisualPosition.x, 0, VisualPosition.y);
    }

    public void UpdateDoRotation()
    {
        DisplayObject.transform.rotation = Quaternion.Euler(0, VisualRotation, 0);
    }

    // Task & Job Management //

    void HandleJobs()
    {
        if(FetchDelay > 0) FetchDelay--;

        // Cancel the active task if requested
        if (ActiveTask != null && ActiveTask.isCancelled)
        {
            TaskCancelled();
            //Debug.Log("Cancelled Task");
        }
        // Handle the active Job
        if (JobStack.Count > 0)
        {
            JobWasPopped = false;
            JobWasStacked = false;
            TaskWasFailed = false;

            Job job = JobStack.Peek();
            if (job.wasStarted == false) job.Start();
            if (!JobWasPopped && !JobWasStacked && !TaskWasFailed) job.Tick();
            return;
        }
        // Add a fetch task job if no other job exists
        if (FetchDelay < 1 && JobStack.Count == 0 && JobQueue.Count == 0)
        {
            EnqueueJob(new FetchTask());
            ResetFetchDelay();
            //Debug.Log("Queue Fetchtask");
            return;
        }
        // Dequeue a job if the stack is empty
        if (JobStack.Count == 0 && JobQueue.Count > 0)
        {
            StackJob(JobQueue.Dequeue());
            //Debug.Log("Dequeue Task");
            return;
        }
    }

    public void StackJob(Job job)
    { 
        job.rover = this; 
        JobStack.Push(job);
        JobWasStacked = true;

        if(DebugEnabled) Debug.Log($"Rover Stacked Job: {job}");
    }


    public void PopJob()
    {
        if(JobStack.Count == 0) { return; }
        var job = JobStack.Pop();
        JobWasPopped = true;

        if (DebugEnabled) Debug.Log($"Rover Popped Job: {job}");
    }

    public void EnqueueJob(Job job)
    {
        job.rover = this;
        JobQueue.Enqueue(job);

        if (DebugEnabled) Debug.Log($"Rover Enqueued Job: {job}");
    }

    public void TaskFailed()
    {  
        var taskToFail = ActiveTask;
        TaskManager.QueueTask(taskToFail);
        ActiveTask.rover = null;
        ClearTask();
        taskToFail.OnFailed?.Invoke(); 

        TaskWasFailed = true;
        if (DebugEnabled) Debug.Log($"Rover Failed Task: {taskToFail}");
    }

    public void TaskFinished()
    {
        if (DebugEnabled) Debug.Log($"Rover Finished Task: {ActiveTask}");

        ActiveTask.OnCompleteCallback?.Invoke();
        ClearTask(); 
    }

    public void TaskCancelled()
    {
        if (DebugEnabled) Debug.Log($"Rover Task Cancelled: {ActiveTask}");

        ActiveTask.OnCancelledCallback?.Invoke();
        ClearTask(); 
    }

    public void ClearTask()
    {
        ActiveTask = null;
        JobQueue.Clear();
        JobStack.Clear();

        DisplayObject.StopParticleEffect("MovingParticles"); 
        //Debug.Log("Rover Cleared Task");
    } 

    public void ResetFetchDelay()
    {
        FetchDelay = Random.Range(20, 120); 
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