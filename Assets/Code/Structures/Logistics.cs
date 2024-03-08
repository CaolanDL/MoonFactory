using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using ExtensionMethods;
using static UnityEditor.Progress;
using System.Linq;

public enum Attempt
{
    Success,
    Fail
}

namespace Logistics
{
    public class ChainManager
    {
        public static List<Chain> chains = new();

        public void UpdateChains()
        {
            foreach(Chain chain in chains)
            {
                chain.wasUpdatedThisTick = false;
            }

            foreach(Chain chain in chains)
            {
                if (chain.wasUpdatedThisTick) continue;

                chain.Update();

                chain.wasUpdatedThisTick = true;
            }
        }
    }

    public class Chain
    {
        public List<Conveyor> conveyors = new();
        public List<Item> items = new();

        int chainCapacity { get { return conveyorCapacity * conveyors.Count; } }
        int chainLength { get { return Conveyor.Length * conveyors.Count; } }
 
        static int conveyorCapacity = Conveyor.Length / Item.Size;

        static int speed = 1;

        public bool wasUpdatedThisTick;
        bool ChainAtCapacity { get { return items.Count >= conveyorCapacity; } }


        public Chain()
        {
            ChainManager.chains.Add(this);
            Debug.Log("New chain created");
        }

        public void Update()
        {
            IterateItems();

            wasUpdatedThisTick = true;
        }

        public void IterateItems()
        {
            // TryTransferLastItem()

            if (items.Count == chainCapacity || items.Count == 0) return;

            if (items[0].distance! < chainLength)
            {
                items[0].distance += speed;
                if (items[0].distance > chainLength)
                {
                    items[0].distance = chainLength;
                    TryTransferLastItem();
                }
                items[0].UpdateWorldPosition(this);
            }

            for (int i = items.Count - 1; i > 0; i--)
            {
                int nextItemStart = items[i + 1].distance - Item.Size;

                if (items[i].distance! < nextItemStart) continue;

                items[i].distance += speed;

                if (items[i].distance > nextItemStart)
                {
                    items[i].distance = nextItemStart;
                }

                items[i].UpdateWorldPosition(this);
            }
        }

        public bool TryTransferLastItem()
        {
            return false;
        }

        public void TransferLastItem()
        {

        }

        public bool TryAddFirstItem(Item item)
        {
            if (ChainAtCapacity || items[0].distance <= Item.Size) return false;

            items.Insert(0, item); // Prepend Item

            return true;
        }

        public void AddConveyor(Conveyor newConveyor, Conveyor existingConveyor)
        {
            if (conveyors.IndexOf(existingConveyor) == 0)
            {
                conveyors.Insert(0, newConveyor);
            }
            if (conveyors.IndexOf(existingConveyor) == conveyors.Count)
            {
                conveyors.Add(newConveyor);
            }
        }

        public void RemoveConveyor(Conveyor conveyor)
        {
            DeleteItemsOnConveyor(conveyor);

            if (conveyors.IndexOf(conveyor) == 0 || conveyors.IndexOf(conveyor) == conveyors.Count-1)
            {
                conveyors.Remove(conveyor);
            }
            else
            {
                Split(conveyor);
            }
        }

        void DeleteItemsOnConveyor(Conveyor conveyor)
        {
            List<Item> itemsToDelete = new();

            foreach (var item in items.Where(i => i.GetConveyor(this) == conveyor))
            {
                itemsToDelete.Add(item);
            }
            foreach (var item in itemsToDelete)
            {
                items.Remove(item);
            }
        }

        public void Split(Conveyor conveyor)
        {

        }

        public void MergeWith()
        {

        }
    }

    public class Conveyor : Machine
    {
        public static int Length = 5;

        public static int TurnStartOffset = byte.MaxValue / 8;

        public Chain parentChain;

        public Conveyor nextConveyor;
        public Conveyor lastConveyor;

        public TurnConfig turnConfig = new();

        public enum TurnConfig
        {
            Straight,
            LeftTurn,
            RightTurn
        }

        public override void OnConstructed()
        {
            TryAddConnections();
        }

        private void TryAddConnections()
        {
            var worldGrid = GameManager.Instance.gameWorld.worldGrid;

            TryConveyorInfront();

            TryConveyorFacingMe();

            if (parentChain == null)
            {
                parentChain = new Chain();
            }

            return;

            Entity GetNeighbor(sbyte rotationFactor)
            {
                //Entity _entity = worldGrid.GetEntityAt(position + rotation.Rotate(rotationFactor).ToInt2());
                //if (_entity != null) { Debug.Log($"{_entity} rotation: {_entity.rotation}"); } 
                return worldGrid.GetEntityAt(position + rotation.Rotate(rotationFactor).ToInt2());
            }

            void JoinChainOf(Conveyor otherConveyor)
            {
                otherConveyor.parentChain.AddConveyor(this, otherConveyor);
                parentChain = otherConveyor.parentChain;
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
                    conveyorInfront.SetRotationConfig(TurnConfig.Straight);
                }
                else if (conveyorInfront.rotation == rotation.Rotate(-1))
                {
                    conveyorInfront.SetRotationConfig(TurnConfig.LeftTurn);
                }
                else if (conveyorInfront.rotation == rotation.Rotate(+1))
                {
                    conveyorInfront.SetRotationConfig(TurnConfig.RightTurn);
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
                    JoinChainOf(conveyorInfront);
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

                if (IsConveyorFacingMe(rearNeighbor, 0)) { SetRotationConfig(TurnConfig.Straight); }
                else if (IsConveyorFacingMe(leftNeighbor, +1)) { SetRotationConfig(TurnConfig.LeftTurn); }
                else if (IsConveyorFacingMe(rightNeighbor, -1)) { SetRotationConfig(TurnConfig.RightTurn); }
                else { return; }

                conveyorFacingMe.nextConveyor = this;
                lastConveyor = conveyorFacingMe;

                if (nextConveyor != null)
                {
                    // Call Merge on conveyorInfront.parentChain passing parentChain & conveyorInfront
                }
                else
                {
                    JoinChainOf(conveyorFacingMe);
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

        private void SetRotationConfig(TurnConfig config)
        {
            if (this.turnConfig == config) { return; }

            if (config == 0)
            {
                displayObject.SetActiveModel("Default");
            }
            if (config == TurnConfig.LeftTurn)
            {
                displayObject.SetActiveModel("Left Turn");
            }
            if (config == TurnConfig.RightTurn)
            {
                displayObject.SetActiveModel("Right Turn");
            }
        }

        bool TryAddItem(Item item)
        {
            return false;
        }

        Item GetFirstItem()
        {
            return null;
        } 
    }

    public class Item
    {
        public static int Size = 1;

        public ResourceData data;


        public int distance;

        float2 worldPosition;


        public Conveyor GetConveyor(Chain chain)
        {
            return chain.conveyors[distance / Conveyor.Length];
        } 

        public void UpdateWorldPosition(Chain chain)
        {
            Conveyor currentConveyor = GetConveyor(chain);

            int distanceOnConveyor = distance % Conveyor.Length;

            byte normalisedDistanceOnConveyor = (byte)(distanceOnConveyor / Conveyor.Length); // Resolution of 0-255 as byte, probably premature optimisation but lets go with it (pray to C this doesnt destroy me)

            Vector2 positionOnConveyor = new();

            sbyte turnFactor = (sbyte)(currentConveyor.turnConfig == Conveyor.TurnConfig.Straight
                || normalisedDistanceOnConveyor < Conveyor.TurnStartOffset ? 0 : currentConveyor.turnConfig == Conveyor.TurnConfig.LeftTurn ? -1 : +1); // Yeah dont ask, its setting turn offset based on the turn configuration of the current belt

            if (currentConveyor.turnConfig == Conveyor.TurnConfig.Straight || normalisedDistanceOnConveyor < Conveyor.TurnStartOffset || normalisedDistanceOnConveyor > byte.MaxValue - Conveyor.TurnStartOffset)
            {
                positionOnConveyor = new Vector2(0.5f, normalisedDistanceOnConveyor / byte.MaxValue).RotateAround(new Vector2(0.5f, 0.5f), currentConveyor.rotation + turnFactor * 90);
            }
            else
            {
                // See notion circle math page. Christ, I am tired. 
                // We are basically rotating an (0.5,0) vector around an axis by the normalised distance to achieve a curve.
                positionOnConveyor = new Vector2(0.5f, 0 + Conveyor.TurnStartOffset).RotateAround(new Vector2(0.5f, 0.5f), currentConveyor.rotation * 90);

                positionOnConveyor = positionOnConveyor.RotateAround(Vector2.zero.RotateAround(new Vector2(0.5f, 0.5f), currentConveyor.rotation * 90), (normalisedDistanceOnConveyor / sbyte.MaxValue) - (Conveyor.TurnStartOffset * 2 / sbyte.MaxValue));

            }

            worldPosition = positionOnConveyor.ToFloat2() + currentConveyor.position;
        }

    } 
}