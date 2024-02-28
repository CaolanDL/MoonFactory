using System.Collections.Generic;

public class Item
{
    public ResourceData data;
}


public class Resource
{
    public ResourceData data;
}

public class ResourceStack
{
    public Resource resource;

    public int quantity;

    float weight;
}

public class Inventory
{ 
    public List<ResourceStack> stacks;

    public int maxWeight;
    public int totalWeight;
    bool atCapacity;
}
