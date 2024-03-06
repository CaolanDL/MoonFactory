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
            base.Constructed();

            TryAddConnections();
        }

        private void TryAddConnections()
        {
            var worldGrid = GameManager.Instance.gameWorld.worldGrid; 

            TryConveyorInfront();

            TryConveyorFacingMe();

            return;

            Entity GetNeighbor(sbyte rotationFactor)
            {
                //Entity _entity = worldGrid.GetEntityAt(position + rotation.Rotate(rotationFactor).ToInt2());
                //if (_entity != null) { Debug.Log($"{_entity} rotation: {_entity.rotation}"); } 
                return worldGrid.GetEntityAt(position + rotation.Rotate(rotationFactor).ToInt2());
            }  

            // Try to add any conveyors I am facing
            void TryConveyorInfront()
            {
                Entity entityInfront = GetNeighbor(0);

                if (entityInfront is null || entityInfront.GetType() != typeof(Conveyor)) return;

                Conveyor conveyorInfront = (Conveyor)entityInfront;

                if (conveyorInfront.rotation == rotation.Rotate(2)) return;

                if (conveyorInfront.lastConveyor != null) return;

                if (conveyorInfront.rotation == rotation.Rotate(0))
                {
                    conveyorInfront.SetConfig(Config.Straight);
                }
                else if (conveyorInfront.rotation == rotation.Rotate(-1))
                {
                    conveyorInfront.SetConfig(Config.LeftTurn);
                }
                else if (conveyorInfront.rotation == rotation.Rotate(+1))
                {
                    conveyorInfront.SetConfig(Config.RightTurn);
                }
                else
                {
                    return;
                }

                conveyorInfront.lastConveyor = this;
                this.nextConveyor = conveyorInfront;

                if (lastConveyor != null)
                {
                    // Call Merge on conveyorInfront.parentChain passing parentChain & conveyorInfront
                }
                else
                {
                    // Call AddConveyor on conveyorInfront.parentChain

                } 

            }

            // Try to add any conveyors facing me
            void TryConveyorFacingMe()
            {
                if (lastConveyor != null) { return; }

                Conveyor conveyorFacingMe = null;

                Entity rightNeighbor = GetNeighbor(+1);
                Entity leftNeighbor = GetNeighbor(-1);
                Entity rearNeighbor = GetNeighbor(-2);   

                if (IsConveyorFacingMe(rearNeighbor, 0)) { SetConfig(Config.Straight); }
                else if (IsConveyorFacingMe(leftNeighbor, +1)) { SetConfig(Config.LeftTurn); }
                else if (IsConveyorFacingMe(rightNeighbor, -1)) { SetConfig(Config.RightTurn); }
                else { return; }

                conveyorFacingMe.nextConveyor = this;
                lastConveyor = conveyorFacingMe;
                 
                if (nextConveyor != null)
                {
                    // Call Merge on conveyorInfront.parentChain passing parentChain & conveyorInfront
                }
                else
                {
                    // Call AddConveyor on conveyorInfront.parentChain
                } 

                bool IsConveyorFacingMe(Entity entity, sbyte rotationFactor)
                {
                    if (entity == null) { return false; }

                    if (entity.GetType() == typeof(Conveyor) &&
                    entity.rotation == rotation.Rotate(rotationFactor) &&
                    ((Conveyor)entity).nextConveyor == null)
                    {
                        conveyorFacingMe = ((Conveyor)entity); 
                        return true;
                    }
                    return false;
                } 

                /*                Entity entityInfront = worldGrid.GetEntityAt(positionInfront);

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
                                }*/
            }
        }

        private void SetConfig(Config config)
        {
            if (this.config == config) { return; }

            if (config == 0)
            {
                displayObject.SetActiveModel("Default");
            }
            if (config == Config.LeftTurn)
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