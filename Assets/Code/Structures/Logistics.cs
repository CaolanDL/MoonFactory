using UnityEngine;

namespace Logistics
{
    public class Merger : Machine
    {
        public override void OnConstructed()
        {
            foreach (var inventory in InputInventories) inventory.maxItems = 1;

            OutputInventories[0].maxItems = 1;
        }

        private int iterator;
        private int Iterator
        {
            get { return iterator; }
            set { iterator = value % 3; }
        }

        public override void OnTick()
        {
            for (int i = 0; i < 3; i++)
            {
                Iterator++;
                if (TransferAnythingRandom(InputInventories[Iterator], OutputInventories[0]))
                {
                    Iterator++;
                    break;
                }
            }

            TryOutputAnything(OutputInventories[0], structureData.outputs[0]);
        }
    }

    public class Splitter : Machine
    {
        public override void OnConstructed()
        {
            foreach (var inventory in OutputInventories) inventory.maxItems = 1;

            InputInventories[0].maxItems = 1;
        }

        private int iterator;
        private int Iterator
        {
            get { return iterator; }
            set { iterator = value % 3; }
        }

        public override void OnTick()
        { 
            for (int i = 0; i < 3; i++)
            {
                TransferAnythingRandom(InputInventories[0], OutputInventories[Iterator]);
                Iterator++;

                if (TryOutputAnything(OutputInventories[Iterator], structureData.outputs[Iterator]))
                { 
                    Iterator++;
                    break;
                }
            }
        }
    }

    public class Hopper : Machine
    {
        public Hopper() { }
    }

    public class Silo : Hopper
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
}