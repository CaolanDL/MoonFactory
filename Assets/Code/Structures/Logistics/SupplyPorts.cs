using RoverTasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

//! Supply Ports
/* 
Why I need it:
I need to be able to search through all structures that are able to supply a
resource to a rover.

What I need it to do:
I need the port to read the available resources from one or more inventories inside the structure.
I need the port to be able to be disabled dynamically. Likely it will simply be destroyed when all of a machines outputs are connected.
I need hoppers to be able to be prioritised over machines when rovers are searching through ports.
*/

public class Port
{
    public List<Inventory> LinkedInventories = new();

    public void AddInventory(Inventory inventory)
    {
        LinkedInventories.Add(inventory);
    }

    public void AddInventories(List<Inventory> inventories)
    {
        this.LinkedInventories.AddRange(inventories);
    }

    public bool HasResource(ResourceData resource)
    {
        foreach (Inventory inventory in LinkedInventories)
        {
            if (inventory == null) { continue; }
            if (inventory.GetQuantityOf(resource) > 0) return true;
        }
        return false;
    }

    public bool HasResourceQuantity(ResourceQuantity resourceQuantity)
    {
        int quantityFound = 0;
        foreach (Inventory inventory in LinkedInventories)
        {
            if (inventory == null) { continue; }
            quantityFound += inventory.GetQuantityOf(resourceQuantity.resource);
            if (quantityFound >= resourceQuantity.quantity) return true;
        }
        return false;
    }

    public int GetQuantity(ResourceData resource)
    {
        int quantityFound = 0;
        foreach (Inventory inventory in LinkedInventories)
        {
            if (inventory == null) { continue; }
            quantityFound += inventory.GetQuantityOf(resource);
        }
        return quantityFound;
    }

    public int GetUnreservedQuantity(ResourceData resource)
    {
        int quantityFound = 0;

        foreach (Inventory inventory in LinkedInventories)
        {
            if (inventory == null) { continue; }
            quantityFound += inventory.GetUnreservedQuantityOf(resource);
        }
        return quantityFound;
    }

    public int GetReservedQuantity(ResourceData resource)
    {
        int quantityFound = 0;
        foreach (Inventory inventory in LinkedInventories)
        {
            if (inventory == null) { continue; }
            if (inventory.reservedResources.TryGetValue(resource, out int quantity))
            {
                quantityFound += quantity;
            }
        }
        return quantityFound;
    }

    public virtual void Delete()
    {
        foreach (Inventory inventory in LinkedInventories)
        {
            if (inventory == null) { continue; }
            inventory.reservedResources.Clear();
        }
    }
}

public class SupplyPort : Port
{
    public static List<SupplyPort> Pool = new();

    public Structure parent;

    //public Dictionary<ResourceData, int> reservedResources;

    public SupplyPort(Structure parent)
    {
        Pool.Add(this);
        this.parent = parent;
    }

    public void ReserveResource(ResourceQuantity resourcesToReserve)
    {
        foreach (Inventory inventory in LinkedInventories)
        {
            int freeQuantity = inventory.GetUnreservedQuantityOf(resourcesToReserve.resource);

            if (freeQuantity == 0) continue;
            var quantityToReserve = Mathf.Clamp(freeQuantity, 0, resourcesToReserve.quantity);

            inventory.ReserveResource(new ResourceQuantity(resourcesToReserve.resource, quantityToReserve));

            resourcesToReserve.quantity -= quantityToReserve;
            if (resourcesToReserve.quantity <= 0) { return; }
        }
    }

    //? There is a bug where rovers will sometimes fail to free resources
    //? I think this function might be the problem.
    public void FreeResource(ResourceQuantity resourcesToFree, bool removeResources = false)
    {
        foreach (Inventory inventory in LinkedInventories)
        {
            int reservedQuantity = inventory.reservedResources[resourcesToFree.resource];

            if (reservedQuantity == 0) continue;
            var quantityToFree = Mathf.Clamp(resourcesToFree.quantity, 0, reservedQuantity);

            inventory.FreeResource(resourcesToFree.resource, quantityToFree);
            if (removeResources) inventory.RemoveResource(resourcesToFree.resource, quantityToFree);

            resourcesToFree.quantity -= quantityToFree;
            if (resourcesToFree.quantity <= 0) { return; }
        }
    }


    public void CollectResource(ResourceQuantity resourceQuantity)
    {
        Debug.Log($"Collected:{resourceQuantity.quantity} {resourceQuantity.resource}s");

        if (GetReservedQuantity(resourceQuantity.resource) == 0) throw new System.Exception("Tried to collect a resource that doesnt exist");

        FreeResource(resourceQuantity, removeResources: true);
    }

    public override void Delete()
    {
        base.Delete();
        Pool.Remove(this);
    }
}

public class RequestPort : Port
{
    public static List<RequestPort> Pool = new();
    public Structure parent;
    ManagedTask ManagedTask = new();
    public ResourceData TargetResource;
    int TargetQuantity;

    public RequestPort(Structure parent)
    {
        Pool.Add(this);
        this.parent = parent;
    }

    public void SetRequest(ResourceData resource, int quantity)
    {
        TargetResource = resource;
        TargetQuantity = quantity;
    }

    public void TryRequest()
    {
        if (TargetResource == null) return;
        if (ManagedTask.taskExists) return;

        Debug.Log("Created Request Task");

        ManagedTask.TryCreateTask(new SoftRequestResourceTask(TargetResource, TargetQuantity, parent.position));
    }

    /// <returns>Remainder after maximum resources accepted</returns>
    public int SupplyResources(ResourceQuantity resourceQuantity)
    {
        return SupplyResources(resourceQuantity.resource, resourceQuantity.quantity);
    }
    public int SupplyResources(ResourceData resource, int quantity)
    {
        int remainder = quantity;

        foreach (var inventory in LinkedInventories)
        {
            var maxAcceptable = inventory.GetMaxAcceptable(resource);

            if (inventory.GetQuantityOf(resource) > 0 && inventory.GetMaxAcceptable(resource) > 0)
            {
                var n = math.clamp(remainder, 0, maxAcceptable);
                if (inventory.TryAddResource(resource, n)) { remainder -= n; }
                if (n <= 0) { return 0; }
            }
        }

        foreach (var inventory in LinkedInventories)
        {
            var maxAcceptable = inventory.GetMaxAcceptable(resource);

            if (inventory.GetMaxAcceptable(resource) > 0)
            {
                var n = math.clamp(remainder, 0, maxAcceptable);
                if (inventory.TryAddResource(resource, n)) { remainder -= n; }
                if (n <= 0) { return 0; }
            }
        }

        return remainder;
    }

    public override void Delete()
    {
        base.Delete();
        Pool.Remove(this);
    }
}