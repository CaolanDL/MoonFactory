using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using ExtensionMethods;
using Logistics;

public class Machine : Structure
{
    public List<Inventory> InputInventories = new();
    public List<Inventory> OutputInventories = new();

    public override void OnInitialise()
    {
        for (int i = 0; i < structureData.inputs.Count; i++) { InputInventories.Add(new()); }
        for (int i = 0; i < structureData.outputs.Count; i++) { OutputInventories.Add(new()); }
    }

    public bool TryOutputItem(ResourceData resource, Inventory inventory, PositionAndRotation outputTransform)
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        var entityAtLocation = worldGrid.GetEntityAt(position + (outputTransform.position.ToVector2().Rotate(rotation * 90).ToInt2()));

        if (entityAtLocation == null) { return false; }

        // Output to first conveyor in chain.
        if (entityAtLocation.GetType() == typeof(Conveyor))
        {
            if (entityAtLocation.rotation == outputTransform.rotation.Rotate(rotation))
            {
                if (((Conveyor)entityAtLocation).parentChain.TryAddFirstItem(resource)) inventory.RemoveResource(resource, 1);
                return true;
            } 
        }
        else if (entityAtLocation.GetType().IsSubclassOf(typeof(Machine)))
        {
            return true;
        }

        return false;
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
        resource = GameManager.Instance.globalData.resources[0];

        OutputInventories[0].maxWeight = 100;

        FillInventory();
    }

    public override void OnTick()
    {
        TryOutputItem(OutputInventories[0].GetStack(resource).resource, OutputInventories[0], structureData.outputs[0]);
    }

    void FillInventory()
    {
        Inventory onlyInventory = OutputInventories[0];

        var n = onlyInventory.GetMaxAcceptable(resource);

        onlyInventory.TryAddResource(resource, n);
    }
}