using UnityEngine;

public class TwoByOne : Machine
{

}

public class DebugOutput : Machine
{
    ResourceData resource;

    public override void OnConstructed()
    {
        resource = GameManager.Instance.GlobalData.Resources[0];

        OutputInventories[0].maxWeight = 1;

        FillInventory();

        //This sucks. Please refactor crafting to sub class of machine, then have hoppers and debug outputs just inherit from machine.
        SupplyPort.Delete();
        SupplyPort = null;
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

        TryOutputItemFromInventory(resource, OutputInventories[0], StructureData.outputs[0]);
    }

    void FillInventory()
    {
        Inventory onlyInventory = OutputInventories[0];

        var n = onlyInventory.GetMaxAcceptable(resource);

        onlyInventory.TryAddResource(resource, n);

        //Debug.Log(onlyInventory.GetQuantityOf(resource));
    }
}