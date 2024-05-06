using System.Collections;
using UnityEngine;


public class StaticDrill : Structure
{ 
    public Inventory inventory = new Inventory();

    ResourceData outputResource;

    public override void OnInitialise()
    {
        base.OnInitialise();

        outputResource = StructureData.CraftableResources[0];

        inventory.maxItems = 10;

        SupplyPort = new SupplyPort(this);
        SupplyPort.AddInventory(inventory);
    }

    static int miningDelay = 50; //1 second
    int miningCountdown = miningDelay;

    public override void OnTick()
    {
        base.OnTick();
        if (inventory.GetMaxAcceptable(outputResource) > 0)
        {
            miningCountdown--;

            if(miningCountdown <= 0)
            {
                miningCountdown = miningDelay;
                inventory.TryAddResource(outputResource, 1);
            }
        }
    }
}