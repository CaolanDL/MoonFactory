
using System.Collections.Generic;
using Unity.Mathematics;

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

    public DisplayObject displayObject;

    public Inventory Inventory = new();
     

    public void Init(int2 spawnLocation, DisplayObject displayObject)
    {
        this.displayObject = displayObject;

        JobQueue.Enqueue(new Job());
    } 

    public void Tick()
    {
        HandleActions();
    }

    // Task Management //

    Queue<Job> JobQueue = new Queue<Job>();

    Task activeTask = null;

    bool JobComplete = false;   

    void HandleActions()
    {

    } 

    // Jobs // 

    void FetchTask()
    {
        activeTask = null;

        // Task Fetching Logic; 

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
        if(Module != RoverModule.None) { return; }

        if(Module != module)
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