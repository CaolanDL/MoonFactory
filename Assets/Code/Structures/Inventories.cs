using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

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

public class Inventory
{
    public Entity parentEntity;

    public List<ResourceStack> stacks = new();

    public int maxItems = int.MaxValue;
    public int totalItems = 0;
    public int maxWeight = 1;
    public int totalWeight = 0;

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

        if (AtCapacitytByWeight || AtCapacityByCount) return false;

        AddStackIfNoneExist(resource);

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
        }
    }

    void AddStackIfNoneExist(ResourceData resource)
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