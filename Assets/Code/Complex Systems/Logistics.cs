using System.Collections.Generic;
using ExtensionMethods;
using Unity.Mathematics;
using UnityEngine;

namespace Logistics
{
    public class ChainManager
    {
        public static List<Chain> chains = new();
    }

    public class Chain
    {
        public ushort id;

        public Chain()
        {
            var chains = ChainManager.chains; 
            chains.Add(this);
            id = (byte)(chains[chains.Count - 1].id + 1);
        }
    }

    public class Conveyor : Machine
    {
        public ushort chainId;

        public Conveyor nextConveyor;
        public Conveyor lastConveyor;

        public Config config = new();
        
        public enum Config
        {
            Straight,
            LeftTurn,
            RightTurn
        }

        public override void Constructed()
        {
            GameObject newDisplayGameObject = UnityEngine.Object.Instantiate(data.displayObject, position.ToVector3(), rotation.ToQuaternion(), GameManager.Instance.transform);

            displayObject = newDisplayGameObject.GetComponent<DisplayObject>();

            TryAddConnections(); 
        }

        private void TryAddConnections()
        {
            var worldGrid = GameManager.Instance.gameWorld.worldGrid;

            int2 positionInfront = position + rotation.ToInt2();

            TryConveyorInfront(); 

            void TryConveyorInfront()
            {
                Entity entityInfront = worldGrid.GetEntityAt(positionInfront);

                if (entityInfront is null || entityInfront.GetType() != typeof(Conveyor)) return;

                Conveyor conveyorInfront = (Conveyor)entityInfront;

                if (conveyorInfront.rotation == rotation + 2) return;

                if (conveyorInfront.lastConveyor != null) return;

                switch (conveyorInfront.rotation - rotation)
                {
                    case 1:
                        conveyorInfront.SetConfig(Config.RightTurn); break;
                    case -1:
                        conveyorInfront.SetConfig(Config.LeftTurn); break;
                    default:
                        conveyorInfront.SetConfig(Config.Straight); break;
                } 
            }
        }

        private void SetConfig(Config config)
        {
            if(this.config == config) { return; }

            if(config == 0)
            {
                displayObject.SetActiveModel("Default");
            }
            if(config == Config.LeftTurn)
            {
                displayObject.SetActiveModel("Left Turn");
            }
            if (config == Config.RightTurn)
            {
                displayObject.SetActiveModel("Right Turn");
            }
        } 
    }

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

}