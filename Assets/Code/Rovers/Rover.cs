
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using RoverJobs;
using UnityEngine;

public enum RoverModule
{
    None,
    Logistics,
    Construction,
    Mining
}

public class Rover
{
    public static List<Rover> Pool = new();

    public static float moveSpeed = 1.0f;

    public DisplayObject displayObject;

    public SmallTransform smallTransform = new();

    public float2 position
    {
        get {  return smallTransform.position; }
        set { smallTransform.position = value; }
    }
    public ushort rotation
    {
        get {  return smallTransform.rotation; }
        set { smallTransform.rotation = value; }
    }

    public Inventory Inventory = new();

    public Task activeTask;
    public Queue<Job> JobQueue = new Queue<Job>();
    public Stack<Job> JobStack = new Stack<Job>(); 

    public void Init(int2 spawnLocation, DisplayObject displayObject)
    {
        this.displayObject = displayObject;

        JobQueue.Enqueue(new FetchTask());
    }

    public void Tick()
    {
        HandleJobs();
    }

    // Task & Job Management //

    void HandleJobs()
    {
        if (JobStack.Count > 0)
        {
            Job job = JobStack.Peek();
            job.Tick();
            return;
        }

        if (JobQueue.Count == 0)
        {
            JobQueue.Enqueue(new FetchTask());
            return;
        }

        if (JobStack.Count == 0)
        {
            JobStack.Push(JobQueue.Dequeue());
            return;
        }
    }

    public void OnTaskFailed()
    {
        TaskManager.AddTask(activeTask);
        activeTask = null;
        JobQueue.Clear();
        JobStack.Clear();
    }

    // Actions //

    void Goto()
    {

    }


    // Modules //

    public RoverModule Module = RoverModule.None;

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