using UnityEngine;
using ExtensionMethods;
using System.Collections.Generic;
using RoverTasks;

namespace Logistics
{
    public class Hopper : Machine
    {
        public static List<Hopper> pool = new List<Hopper>();

        public static int maxItems = 16;
        public static int renderGap = 1;

        public Inventory inputInventory;
        public Inventory storageInventory;

        public bool isRequestor;
        public bool isSupplier;

        public override void OnInitialise()
        {
            //base.OnInitialise();

            InputInventories = new Inventory[1];
            OutputInventories = new Inventory[1];

            InputInventories[0] = new Inventory();
            OutputInventories[0] = new Inventory();

            inputInventory = InputInventories[0];
            storageInventory = OutputInventories[0];

            inputInventory.maxItems = 1;

            storageInventory.maxItems = maxItems;
            storageInventory.maxWeight = int.MaxValue;

            pool.Add(this);

            AddPorts();
        }

        public override void OnConstructed()
        {
            if (TutorialProxy.IsActive)
            {
                TutorialProxy.Action?.Invoke(TutorialEvent.HopperBuilt);
            }
        }

        // Add supply request port components
        public override void AddPorts()
        {
            RequestPort = new(this);
            RequestPort.AddInventory(storageInventory);
        }

        public override void OnDemolished()
        {
            pool.Remove(this);
        }

        int timer = 0;
        int timerMax = 30;

        public override void OnTick()
        {
            timer++;
            if (timer > timerMax)
            {
                timer = 0;
                TryOutputAnything(0);
            }

            TransferAnythingRandom(InputInventories[0], OutputInventories[0]); 

            //Debug.Log(RequestPort.TargetResource);

            if (isRequestor && RequestPort.TargetResource != null)
            {
                //Debug.Log(storageInventory.GetMaxAcceptable(RequestPort.TargetResource));

                if (storageInventory.GetMaxAcceptable(RequestPort.TargetResource) > 0)
                {
                    RequestPort.TryRequest();
                }
            }
        }

        // This item rendering could be modified to update an array OnItemRecieved and OnItemOutput to reduce per frame overhead
        public override void OnFrameUpdate()
        {
            if (storageInventory.totalItems > 0)
            {
                for (int i = 0; i < storageInventory.totalItems; i += renderGap)
                {
                    var n = i / renderGap;
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

            if (TutorialProxy.IsActive)
            {
                TutorialProxy.SetPopupPosition?.Invoke(GameManager.Instance.CameraController.activeMainCamera.WorldToScreenPoint(DisplayObject.transform.position), TutorialTag.HopperPosition);
            }
        }

        public override void OnClicked(Vector3 mousePosition)
        {
            OpenInterfaceOnHUD(MenuData.Instance.HopperInterface, mousePosition);
        }

        void HandleRequest()
        {
            /*if (!isRequestor || requestTask.taskExists) return;

            requestTask.TryCreateTask(new SoftRequestResourceTask(requestResource, 1));*/
        } 
    }

    public class Silo : Hopper
    {
        public override void OnConstructed()
        {
            maxItems = 48;

            base.OnConstructed();
        }
    }
}