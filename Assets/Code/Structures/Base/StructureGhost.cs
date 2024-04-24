using DataStructs;
using System;
using RoverTasks;
using System.Collections.Generic;
using Unity.Mathematics;

public class StructureGhost : Entity
{
    public StructureData structureData;

    private ManagedTask _managedBuildTask = new();

    private List<ResourceQuantity> deliveredResources = new();

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
 
    /// <returns>Remainder</returns>
    public int SupplyResources(ResourceQuantity resourceQuantity) { return SupplyResources(resourceQuantity.resource, resourceQuantity.quantity); }
    /// <returns>Remainder</returns>
    public int SupplyResources(ResourceData resource, int quantity)
    {
        int remainder = quantity;

        var existingAmount = deliveredResources.Find(x => x.resource == resource).quantity;
        var neededAmount = structureData.requiredResources.Find(x => x.resource == resource).quantity;

        if (existingAmount >= neededAmount) { return remainder; }

        var n = math.clamp(remainder, 0, neededAmount - existingAmount);
        if (inventory.TryAddResource(resource, n)) { remainder -= n; }
        if (n <= 0) { return 0; }

        return remainder;
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