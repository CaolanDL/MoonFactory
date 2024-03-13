using UnityEngine;
using Unity.Mathematics;

using ExtensionMethods;

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
        public static int maxItems = 10;
        public static int renderGap = 4;

        public Inventory inputInventory;
        public Inventory ouputInventory;

        public override void OnConstructed()
        {
            inputInventory = InputInventories[0];
            ouputInventory = OutputInventories[0];

            inputInventory.maxItems = 1;

            ouputInventory.maxItems = maxItems; 
            ouputInventory.maxWeight = int.MaxValue;
        }

        public override void OnTick()
        { 
            TransferAnythingRandom(InputInventories[0], OutputInventories[0]);
            TryOutputAnything(OutputInventories[0], structureData.outputs[0]);
        }


        // This item rendering could be modified to update an array OnItemRecieved and OnItemOutput to reduce per frame overhead
        public override void OnFrameUpdate()
        {
            if (OutputInventories[0].totalItems > 0)
            {
                for (int i = 0; i < OutputInventories[0].totalItems; i += renderGap)
                {
                    var n =  i / renderGap;
                    DrawResourceAtIndex(n);
                }
            }

            void DrawResourceAtIndex(int i)
            {
                float verticalOffset = ((i / 4) * 0.25f);

                Graphics.DrawMesh(
                    OutputInventories[0].GetResourceAtIndex(i).mesh,
                    transform.position.ToVector3()
                    + (Vector3.up * (i / 4) * 0.1f)
                    + (Vector3.up * 0.1f)
                    + (Vector2.up.Rotate(i % 4 * 90 + 45).ToWorldspaceV3() * 0.15f),
                    Quaternion.Euler(0, i * 11.5f, 0),
                    GlobalData.Instance.mat_DevUniversal,
                    0); ; ;
            }
        }
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