using System;
using System.Collections.Generic;

using UnityEngine;
using Unity.Mathematics;

using RoverJobs;
using RoverTasks;
using Random = UnityEngine.Random;
using ExtensionMethods;
using UnityEngine.Rendering;

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

    const float _MoveSpeed = 3f;
    public virtual float MoveSpeed { get { return _MoveSpeed; } }

    const float _TurnSpeed = 3f;
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
    public List<Guid> recentTaskIDs = new();
    public int MemoryClearDelay = 50 * 5;
    public int MemoryClearTimer = 0;

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

    public void OnFrameUpdate()
    {
        if(Inventory.stacks.Count > 0)
        {
            var r = Inventory.stacks[0].resource;
            var trs = Matrix4x4.TRS(
                DisplayObject.itemRenderPoint.transform.position,
               DisplayObject.itemRenderPoint.transform.rotation,
               Vector3.one * 0.8f);
            Graphics.DrawMesh(r.mesh, trs, r.material, 0);
        }

        if (Inventory.stacks.Count > 1)
        {
            var r = Inventory.stacks[1].resource;
            var trs = Matrix4x4.TRS(
                DisplayObject.itemRenderPoint_Secondary.transform.position,
               DisplayObject.itemRenderPoint_Secondary.transform.rotation,
               Vector3.one * 0.8f);
            Graphics.DrawMesh(r.mesh, trs, r.material, 0);
        }
    }

    public void Tick()
    {
        HandleJobs();
        HandleTaskMemory();

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

    void HandleTaskMemory()
    {
        MemoryClearTimer++;

        if(MemoryClearTimer > MemoryClearDelay)
        {
            recentTaskIDs.Clear();
            MemoryClearTimer = 0;
        }
    }

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

        Inventory.DumpToLander();

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

    public void PlaySound(AudioClip audioClip, float volume) => DisplayObject.PlaySound(audioClip, volume); 

    public override void RenderSelectionOutline(Material material)
    { 
        float sineTime = (Mathf.Sin(Time.time * 2f) + 1) / 2; // Sine 0 - 1 
        var p = new Vector3(VisualPosition.x, 0, VisualPosition.y) + (Vector3.up * (sineTime / 8 + 0.01f));
        var r = Quaternion.Euler(0, Time.time * 120f, 0);
        var s = Vector3.one;
        var matrix = Matrix4x4.TRS(p, r, s);
        Graphics.DrawMesh(RenderData.Instance.SelectionGizmo, matrix, RenderData.Instance.SelectionGizmoMaterial, 0);
    }
}
