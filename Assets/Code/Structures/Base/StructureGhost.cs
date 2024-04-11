using DataStructs;
using System;
using RoverTasks;

public class StructureGhost : Entity
{
    public StructureData structureData;

    private ManagedTask _managedBuildTask = new();

    public StructureGhost(StructureData structureData)
    {
        this.structureData = structureData;
        size.x = (byte)structureData.size.x;
        size.y = (byte)structureData.size.y;
    }

    public void OnPlaced()
    { }

    int queueDelay = 75;

    public void OnTick()
    {
        if(queueDelay > 0) { queueDelay--; }
        else if (_managedBuildTask.taskExists == false)
        { 
            _managedBuildTask.TryCreateTask(new BuildStructureTask(this)); 
        } 
    }

    public void Cancel()
    { 
        _managedBuildTask.CancelTask(); 
        GameManager.Instance.ConstructionManager.Ghosts.Remove(this);
        RemoveEntity(); 
    }

    public void FinishConstruction()
    {
        var worldGrid = GameManager.Instance.GameWorld.worldGrid;

        worldGrid.RemoveEntity(position); 
         
        Structure newStructure = StructureFactory.CreateStructure(structureData);

        worldGrid.TryAddEntity(newStructure, position, rotation);

        GameManager.Instance.ConstructionManager.Ghosts.Remove(this);

        newStructure.Constructed();
    }
}