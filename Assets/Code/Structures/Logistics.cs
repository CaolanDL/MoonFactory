using UnityEngine;  
using ExtensionMethods;
using System.Collections.Generic;
using RoverTasks;

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

            TryOutputAnything(0);
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

                if (TryOutputAnything(Iterator))
                { 
                    Iterator++;
                    break;
                }
            }
        }
    }

    public class Hopper : Machine
    {
        public static List<Hopper> pool = new List<Hopper>();

        public static int maxItems = 16;
        public static int renderGap = 1;

        public Inventory inputInventory;
        public Inventory storageInventory;

        public bool isRequestor;
        public bool isSupplier;
        public ResourceData requestResource;  
        public ManagedTask requestTask = new(); 

        public override void OnConstructed()
        {  
            inputInventory = InputInventories[0];
            storageInventory = OutputInventories[0];

            inputInventory.maxItems = 1;

            storageInventory.maxItems = maxItems; 
            storageInventory.maxWeight = int.MaxValue;

            pool.Add(this);
        }

        public override void OnDemolished()
        { 
            pool.Remove(this);
        }  

        public override void OnTick()
        { 
            TransferAnythingRandom(InputInventories[0], OutputInventories[0]);
            TryOutputAnything(0);
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
                ResourceData resource = OutputInventories[0].GetResourceAtIndex(i);

                Graphics.DrawMesh(
                    resource.mesh,
                    transform.position.ToVector3()
                    + (Vector3.up * (i / 4) * 0.1f)
                    + (Vector3.up * 0.1f)
                    + (Vector2.up.Rotate(i % 4 * 90 + 45).ToWorldspaceV3() * 0.15f),
                    Quaternion.Euler(0, i * 11.5f, 0),
                    resource.material,
                    0); ; ;
            }
        }

        public override void OnClicked(Vector3 mousePosition)
        {
            OpenInterfaceOnHUD(MenuData.Instance.HopperInterface, mousePosition);
        } 

        void HandleRequest()
        {
            if (!isRequestor || requestTask.taskExists) return;

            requestTask.TryCreateTask(new HopperRequestTask(requestResource));
        }
    }

    public class Silo : Hopper
    {
        public static new int maxItems = 48;

        public override void OnConstructed()
        { 
            inputInventory = InputInventories[0];
            storageInventory = OutputInventories[0];

            inputInventory.maxItems = 1;

            storageInventory.maxItems = maxItems;
            storageInventory.maxWeight = int.MaxValue;

            pool.Add(this);
        }
    } 
}