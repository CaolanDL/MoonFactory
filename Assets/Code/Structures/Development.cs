using UnityEngine;

public class TwoByOne : Machine
{

}

public class DebugOutput : Machine
{
    ResourceData resource;

    public override void OnConstructed()
    {
        resource = GameManager.Instance.globalData.resources[0];

        OutputInventories[0].maxWeight = 1;

        FillInventory();
    }

    static float OutputInterval = 0.5f;
    int outDelayMod = (int)(1f / Time.fixedDeltaTime * OutputInterval);

    int outDelayLoop;
    int OutDelayLoop
    {
        get { return outDelayLoop; }
        set { outDelayLoop = value % outDelayMod; }
    }

    public override void OnTick()
    {
        OutDelayLoop += 1;

        if (OutDelayLoop != 0) { return; }

        FillInventory();

        TryOutputItem(resource, OutputInventories[0], structureData.outputs[0]);
    }

    void FillInventory()
    {
        Inventory onlyInventory = OutputInventories[0];

        var n = onlyInventory.GetMaxAcceptable(resource);

        onlyInventory.TryAddResource(resource, n);

        //Debug.Log(onlyInventory.GetQuantityOf(resource));
    }
}