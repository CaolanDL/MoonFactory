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
        weight -= resource.weight * quantity;
    }

    public void IncreaseBy(int quantity)
    {

    }
}

public class Inventory // 56 bytes
{
    public Entity parentEntity; // 8 bytes

    public List<ResourceStack> stacks = new(); // Min 32 bytes

    public int maxItems = int.MaxValue; // 4 bytes
    public int totalItems = 0; // 4 bytes
    public int maxWeight = int.MaxValue; // 4 bytes
    public int totalWeight = 0; // 4 bytes

    public int maxTypes = int.MaxValue; // 4 bytes
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

    public int GetMaxAcceptable(ResourceData resource)
    {
        int nByWeight = (int)MathF.Floor(AvailableCapacityByWeight / resource.weight); 

        if (nByWeight < AvailableCapacityByCount)
        {
            return nByWeight;
        }
        else
        {
            return AvailableCapacityByCount;
        } 
    }

    public int GetQuantityOf(ResourceData resource)
    {
        var stack = GetStack(resource);

        if (stack == null) { return 0; }

        return GetStack(resource).quantity;
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

        if (totalWeight + (resource.weight * stack.quantity) > maxWeight) { throw new System.Exception($" Attempted to exceed maximum inventory capacity on {parentEntity}"); }

        stack.quantity += quantity;

        totalWeight += resource.weight * quantity;
        totalItems += quantity;

        return true;
    }

    public void RemoveResource(ResourceData resource, int quantity)
    {
        var stack = GetStack(resource) ?? throw new Exception("Attempted to remove non existant resource from inventory");

        if (stack.quantity - quantity < 0) { throw new Exception("Attempted to remove more resources from an invenotry than exist"); }

        stack.ReduceBy(quantity);

        totalWeight -= resource.weight * quantity;
        totalItems -= quantity;

        if (stack.quantity <= 0)
        {
            stacks.Remove(stack);
            totalTypes--;
        }
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

    public void ClearInventory(ResourceData resource, int quantity)
    {

    }
}