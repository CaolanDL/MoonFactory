using System;
using System.Collections.Generic;
 
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

    public int maxWeight = 1;
    public int totalWeight = 0;

    int availableCapacity
    {
        get { return maxWeight - totalWeight; } 
    }

    bool atCapacity
    {
        get { return totalWeight >= maxWeight; }
    } 

    public int GetMaxAcceptable(ResourceData resource)
    {
        return (int)MathF.Floor(availableCapacity / resource.weight);
    }


    public int GetQuantityOf(ResourceData resource)
    {
        var stack = GetStack(resource);

        if (stack == null) { return  0; } 

        return GetStack(resource).quantity;
    }

    public bool TryAddResource(ResourceData resource, int quantity)
    {
        if(atCapacity) { return false; }

        AddStackIfNoneExist(resource);

        var stack = GetStack(resource);

        if (totalWeight  +(resource.weight * stack.quantity) > maxWeight) { throw new System.Exception($" Attempted to exceed maximum inventory capacity on {parentEntity}"); }

        stack.quantity += quantity;

        totalWeight += resource.weight * quantity; 

        return true;
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

    public void RemoveResource(ResourceData resource, int quantity)
    {
        var stack = GetStack(resource) ?? throw new Exception("Attempted to remove non existant resource from inventory");

        if(stack.quantity - quantity <  0) { throw new Exception("Attempted to remove more resources from an invenotry than exist"); }

        stack.ReduceBy(quantity);

        totalWeight -= resource.weight * quantity;
        
        if(stack.quantity <= 0)
        {
            stacks.Remove(stack);
        } 
    }

    public void ClearInventory(ResourceData resource, int quantity)
    {

    }
}