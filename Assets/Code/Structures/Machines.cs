
using Logistics;
using System.Collections.Generic;
using Unity.Mathematics;

public abstract class Machine : Structure
{
    protected Machine()
    {
        InputInventories = new Inventory[data.inputLocations.Count];
        OutputInventories = new Inventory[data.outputLocations.Count];
    }

    public virtual Inventory[] InputInventories { get; set; }
    public virtual Inventory[] OutputInventories { get; set; }

    public bool TryOutputItem(ResourceData resource, int2 location)
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        var entityAtLocation = worldGrid.GetEntityAt(location);

        if (entityAtLocation.GetType() == typeof(Conveyor))
        {

        }
    }
}

public class Crusher : Machine
{

}

public class DebugOutput : Machine 
{ 
    ResourceData resource; 

    public override void OnConstructed()
    {
        resource = GlobalData.Instance.resources[0];
    }

    public override void OnTick()
    {
        FillInventory();


    }

    void FillInventory()
    {
        var onlyInventory = OutputInventories[0];

        var n = onlyInventory.GetMaxAcceptable(resource);

        onlyInventory.TryAddResource(resource, n);
    }
}