using DataStructs;
using System;
using RoverTasks;

public class StructureGhost : Entity
{
    public StructureData structureData;

    private BuildStructureTask buildTask;

    public StructureGhost(StructureData structureData)
    {
        this.structureData = structureData;
        size.x = (byte)structureData.size.x;
        size.y = (byte)structureData.size.y;
    }

    public void OnPlaced()
    {
        buildTask = new BuildStructureTask(this); 
        ConstructionTasks.QueueTask(buildTask); 
    }

    public void Cancel()
    {
        ConstructionTasks.CancelTask(buildTask);
        GameManager.Instance.ConstructionManager.Ghosts.Remove(this);
        buildTask = null; 
    }

    public void FinishConstruction()
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        worldGrid.RemoveEntity(position); 
         
        Structure newStructure = StructureFactory.CreateStructure(structureData);

        worldGrid.TryAddEntity(newStructure, position, rotation);

        GameManager.Instance.ConstructionManager.Ghosts.Remove(this);

        newStructure.Constructed();
    }
}