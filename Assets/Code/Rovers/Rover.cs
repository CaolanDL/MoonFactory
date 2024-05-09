﻿using System;
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

public class Rover : Entity
{
    public static bool DebugEnabled = false;

    const float _MoveSpeed = 1f;
    public virtual float MoveSpeed { get { return _MoveSpeed; } }

    const float _TurnSpeed = 2f;
    public virtual float TurnSpeed { get { return _TurnSpeed; } }

    public const float _CollectSpeed = 6f;
    public float CollectSpeed { get { return _CollectSpeed; } }

    public const float _MiningSpeed = 6f;
    public float MiningSpeed { get { return _MiningSpeed; } }

    public Inventory Inventory = new();
    public RoverModule Module = RoverModule.None;

    public Task ActiveTask;
    public readonly Queue<Job> JobQueue = new Queue<Job>();
    public readonly Stack<Job> JobStack = new Stack<Job>();
    public bool JobWasPopped= false;
    public bool JobWasStacked = false;
    public bool TaskWasFailed = false;
    public int FetchDelay = 0;

    public static float maxPowerLevel = 1f;
    public float powerLevel = maxPowerLevel;
    static float taskPowerConsumption = maxPowerLevel / 5;

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

        OnTick();
    }

    public virtual void OnTick() { }

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
        if (FetchDelay > 0) FetchDelay--;

        // Cancel the active task if requested
        if (ActiveTask != null && ActiveTask.isCancelled)
        {
            TaskCancelled(); 
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
        if(JobStack.Count == 0 && JobQueue.Count == 0)
        {
            if (FetchDelay < 1 && powerLevel > 0)
            {
                EnqueueJob(new FetchTask());
                ResetFetchDelay();
                return;
            }
            if (powerLevel <= 0 && ActiveTask == null)
            {
                EnqueueJob(new GoCharge());
                return;
            } 
        } 
        // Dequeue a job if the stack is empty
        if (JobStack.Count == 0 && JobQueue.Count > 0)
        {
            StackJob(JobQueue.Dequeue()); 
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
        if(ActiveTask != null)
        {
            var taskToFail = ActiveTask;
            TaskManager.QueueTask(taskToFail);
            ActiveTask.rover = null;
            taskToFail.OnFailed?.Invoke();
        }

        if (DebugEnabled) Debug.Log($"Rover Failed Task");

        ClearTask(); 
        TaskWasFailed = true; 
    }

    public void TaskFinished()
    {
        if (DebugEnabled) Debug.Log($"Rover Finished Task: {ActiveTask}");

        powerLevel -= taskPowerConsumption;

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
    } 

    public void ResetFetchDelay()
    {
        FetchDelay = Random.Range(1, 3); 
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
    static float _MoveSpeed = 1.5f;
    public override float MoveSpeed => _MoveSpeed;

    static float _TurnSpeed = 3f;
    public override float TurnSpeed => _TurnSpeed;


    public Widget()
    {
        Module = RoverModule.Widget; 
    }

    public override void OnTick()
    {
        base.OnTick();

        powerLevel = Mathf.Clamp01(powerLevel + 0.001f);
    }
}