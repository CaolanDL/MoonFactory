using System; 
using System.Collections.Generic;
using UnityEngine;

public class ResourceStack
{
    public ResourceData resource;

    public int quantity;

    float weight;

    public ResourceStack()
    {

    }

    public ResourceStack(ResourceData resource)
    {
        this.resource = resource;
    }

    public void ReduceBy(int quantity)
    {
        this.quantity -= quantity;
        weight -= resource.Weight * quantity;
    }

    public void IncreaseBy(int quantity)
    {

    }
}

public class Inventory // 56 bytes
{
    public Entity parentEntity; // 8 bytes

    public List<ResourceStack> stacks = new(); // Min 32 bytes

    public Dictionary<ResourceData, int> reservedResources = new();

    public Action InventoryUpdated;

    public int maxItems = 9999; // 4 bytes
    public int totalItems = 0; // 4 bytes
    public int maxWeight = 9999 ; // 4 bytes
    public int totalWeight = 0; // 4 bytes

    public int maxTypes = 9999; // 4 bytes
    public int totalTypes = 0; // 4 bytes

    int AvailableCapacityByWeight
    {
        get { return maxWeight - totalWeight; }
    }

    int AvailableCapacityByCount
    {
        get { return maxItems - totalItems; }
    }

    bool AtCapacitytByWeight
    {
        get { return totalWeight >= maxWeight; }
    }

    bool AtCapacityByCount
    {
        get { return totalItems >= maxItems; }
    }

    bool AtMaxTypes
    {
        get { return totalTypes >= maxTypes; }
    }


    public void ReserveResource(ResourceData resource, int quantity)
    {
        ReserveResource(new(resource, quantity));
    }
    public void ReserveResource(ResourceQuantity rq)
    {
       if(GetQuantityOf(rq.resource) < rq.quantity) throw new Exception("Tried to reserve a resource that does not exist"); ; 

       if (reservedResources.ContainsKey(rq.resource))
        {
            reservedResources[rq.resource] += rq.quantity;
        }
       else
        {
            reservedResources.Add(rq.resource, rq.quantity);
        } 
    } 

    public void FreeResource(ResourceData resource, int quantity)
    {
        FreeResource(new(resource, quantity));
    }
    public void FreeResource(ResourceQuantity rq)
    {
        if(reservedResources.ContainsKey(rq.resource) == false) { throw new Exception("Tried to free a reserved resource that does not exist"); }

        if (reservedResources[rq.resource] == 0 || reservedResources[rq.resource] < rq.quantity) { throw new Exception("Tried to free a reserved resource that does not exist"); }

        reservedResources[rq.resource] -= rq.quantity;

        if(reservedResources[rq.resource] <= 0) { reservedResources.Remove(rq.resource); } 
    }

    public int GetMaxAcceptable(ResourceData resource)
    {
        //int nByWeight = (int)MathF.Floor(AvailableCapacityByWeight / resource.Weight);  

        return AvailableCapacityByCount; 
    }

    public int GetQuantityOf(ResourceData resource)
    {
        var stack = GetStack(resource);

        if (stack == null) { return 0; }

        return stack.quantity;
    }

    public int GetUnreservedQuantityOf(ResourceData resource)
    {
        var quantity = GetQuantityOf(resource);

        if (quantity > 0 && reservedResources.ContainsKey(resource))
        {
            quantity -= reservedResources[resource]; 
        } 

        return quantity;
    } 

    public ResourceData GetResourceAtIndex(int index)
    {
        int stackIndex = 0;

        for(int i = 0; i <= index; i++)
        {
            if(i > stacks[stackIndex].quantity) { stackIndex++; } 
            if( i == index ) { return stacks[stackIndex].resource; }
        }

        return null;
    }

    public ResourceData GetRandomResource()
    {
        if (stacks.Count == 0)
        {
            return null;
        }

        int stackIndex = 0;
        if (stacks.Count > 1)
        {
            stackIndex = UnityEngine.Random.Range(0, stacks.Count - 1);
        }

        return stacks[stackIndex].resource;
    }

    public bool TryAddResource(ResourceQuantity rq)
    {
        return TryAddResource(rq.resource, rq.quantity);
    } 
    public bool TryAddResource(ResourceData resource, int quantity)
    { 
        if (resource == null) return false;

        if (AtCapacitytByWeight || AtCapacityByCount)
        { 
            return false;
        }

        if(GetStack(resource) == null && AtMaxTypes) return false;

        TryAddStack(resource);

        var stack = GetStack(resource);

        if (totalWeight + (resource.Weight * stack.quantity) > maxWeight) { throw new System.Exception($" Attempted to exceed maximum inventory capacity on {parentEntity}"); }

        stack.quantity += quantity;

        totalWeight += resource.Weight * quantity;
        totalItems += quantity;

        InventoryUpdated?.Invoke();

        return true;
    }

    public void RemoveResource(ResourceQuantity rq)
    {
        RemoveResource(rq.resource, rq.quantity);
    }
    public void RemoveResource(ResourceData resource, int quantity)
    {
        var stack = GetStack(resource) ?? throw new Exception("Attempted to remove non existant resource from inventory");

        if (stack.quantity - quantity < 0) { throw new Exception("Attempted to remove more resources from an invenotry than exist"); }

        stack.ReduceBy(quantity);

        totalWeight -= resource.Weight * quantity;
        totalItems -= quantity;

        if (stack.quantity <= 0)
        {
            stacks.Remove(stack);
            totalTypes--;
        }

        InventoryUpdated?.Invoke();
    }

    void TryAddStack(ResourceData resource)
    {
        bool resourceTypePresent = false;

        foreach (ResourceStack stack in stacks)
        {
            if (stack.resource == resource)
            {
                resourceTypePresent = true;
                break;
            }
        }

        if (!resourceTypePresent)
        {
            stacks.Add(new ResourceStack(resource));
            totalTypes++; 
        }
    }

    public ResourceStack GetStack(ResourceData resource)
    {
        return stacks.Find(i => i.resource == resource);
    } 

/*    public void ClearInventory(ResourceData resource, int quantity)
    {

    }*/
}