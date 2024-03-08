using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using ExtensionMethods; 
using System.Linq;  

public enum Attempt
{
    Success,
    Fail
}

namespace Logistics
{
    public class Item
    {
        public static int Size = Conveyor.Length / 4;

        public ResourceData data; 

        public int distance;

        public float2 worldPosition;

        public short rotation;
        public short Rotation
        {
            get { return rotation; }
            set { rotation = (short)((value % 360 + 360) % 360); }
        }

        public Item(ResourceData resourceData)
        {
            data = resourceData;
            Debug.Log("item created");
        }

        public int distanceOnConveyor
        {
            get { return (distance) % (Conveyor.Length); }
        }

        public Conveyor GetConveyor(Chain chain)
        {
            if(distance < Conveyor.Length) { return chain.conveyors[0]; }
            //return chain.conveyors[distance - distanceOnConveyor  / Conveyor.Length];
            return chain.conveyors[Mathf.CeilToInt( (distance) / (float) Conveyor.Length)-1];
        }

        public void UpdateWorldPosition(Chain chain)
        {
            Conveyor currentConveyor = GetConveyor(chain);

            float normalisedDistanceOnConveyor = (float)distanceOnConveyor / Conveyor.Length; // Resolution of 0-255 as byte, probably premature optimisation but lets go with it (pray to C this doesnt destroy me)

            Vector2 positionOnConveyor = new();

            sbyte turnFactor = (sbyte)(currentConveyor.turnConfig == Conveyor.TurnConfig.Straight
                || normalisedDistanceOnConveyor < Conveyor.TurnStartOffset ? 0 : currentConveyor.turnConfig == Conveyor.TurnConfig.LeftTurn ? -1 : +1); // Yeah dont ask, its setting turn offset based on the turn configuration of the current belt

            if (currentConveyor.turnConfig == Conveyor.TurnConfig.Straight)
            {
                positionOnConveyor = new Vector2(0, normalisedDistanceOnConveyor);
                Rotation = (short)(currentConveyor.rotation * 90);
            }
            /*else
            {
                // See notion circle math page. Christ, I am tired. 
                // We are basically rotating an (0.5,0) vector around an axis by the normalised distance to achieve a curve.
                positionOnConveyor = new Vector2(0.5f, 0 + Conveyor.TurnStartOffset).RotateAround(new Vector2(0.5f, 0.5f), currentConveyor.rotation * 90);

                positionOnConveyor = positionOnConveyor.RotateAround(Vector2.zero.RotateAround(new Vector2(0.5f, 0.5f), currentConveyor.rotation * 90), (normalisedDistanceOnConveyor / sbyte.MaxValue) - (Conveyor.TurnStartOffset * 2 / sbyte.MaxValue));

            } */
            worldPosition = (positionOnConveyor - (Vector2.up / 2)).Rotate(90 * currentConveyor.rotation) + currentConveyor.position.ToVector2();
        }
    }

    public class ChainManager
    {
        public static List<Chain> chains = new();

        public static void UpdateChains()
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

        public int chainCapacity { get { return conveyorCapacity * conveyors.Count; } }
        public int chainLength { get { return Conveyor.Length * conveyors.Count; } }
 
        static int conveyorCapacity = Conveyor.Length / Item.Size;

        static int speed = 2;

        public bool wasUpdatedThisTick;
        bool ChainAtCapacity { get { return items.Count >= chainCapacity; } }


        public Chain()
        {
            ChainManager.chains.Add(this);
            Debug.Log("New chain created");
        }

        public void Update()
        {
            TryTransferLastItem();

            IterateItems();

            wasUpdatedThisTick = true;
        }

        public void IterateItems()
        {
            // TryTransferLastItem()

            if (items.Count == 0)  { return; }

            int lastItemIndex = items.Count - 1;

            if (items[lastItemIndex].distance < chainLength)
            {
                items[lastItemIndex].distance += speed;

                if (items[lastItemIndex].distance >= chainLength)
                {
                    items[lastItemIndex].distance = chainLength-1;
                    TryTransferLastItem();
                }
            }

            for (int i = items.Count - 2; i >= 0; i--)
            {
                int nextItemStart = items[i + 1].distance - Item.Size;

                if (items[i].distance >= nextItemStart) continue;

                items[i].distance += speed; 

                if (items[i].distance > nextItemStart)
                {
                    items[i].distance = nextItemStart;
                } 
            }

            UpdateItemPositions();
        }

        void UpdateItemPositions()
        {
            foreach (Item item in items)
            {
                item.UpdateWorldPosition(this);
            }
        }

        public bool TryTransferLastItem()
        {
            return false;
        }

        public void TransferLastItem()
        {

        }

        public bool TryAddFirstItem(ResourceData resourceData)
        {
            if (ChainAtCapacity) return false;

            if(items.Count != 0 && items[0].distance <= Item.Size) { return false; }

            var newItem = new Item(resourceData);

            items.Insert(0, newItem); // Prepend Item

            newItem.UpdateWorldPosition(this); // Brute force update its position. You should figure out why the execution order allows items to default to 0,0 on first frame. 

            return true;
        }

        public void PrependConveyor(Conveyor newConveyor)
        {
            conveyors.Insert(0, newConveyor); 
        }

        public void AppendConveyor(Conveyor newConveyor)
        { 
            conveyors.Add(newConveyor); 
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
        public static int Length = 60 * 20;

        public static int TurnStartOffset = 0;//byte.MaxValue / 8;

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
                parentChain.conveyors.Add(this);
            }

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
                    conveyorInfront.parentChain.PrependConveyor(this);
                    parentChain = conveyorInfront.parentChain; 
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
                    conveyorFacingMe.parentChain.AppendConveyor(this);
                    parentChain = conveyorFacingMe.parentChain; 
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

        // BROKEN
        public bool TryAddItem(Item item)
        {
            return false;
/*            bool success = false;

            //firstItemOnConveyor = first Item in parentChain.items where item.parentConveyor == this
            var itemsOnConveyor = GetItemsOnConveyor();

            var firstItemOnConveyor = itemsOnConveyor.Count > 0 ? itemsOnConveyor[0] : null;

            int conveyorDistanceOnChain = GetDistanceOnChain();

            if (firstItemOnConveyor == null)
            {
                for (var i = parentChain.items.Count-1; i >= -1; i--)
                {
                    if (parentChain.items.Count == 0 || i == -1) { }
                    else if (parentChain.items[i].distance < conveyorDistanceOnChain)
                    {
                        parentChain.items.Insert(i+1, item);
                        item.distance = conveyorDistanceOnChain;
                        success = true;
                        break;
                    }
                    if (i == -1)
                    {
                        parentChain.items.Add(item);
                        item.distance = conveyorDistanceOnChain;
                        success = true;
                        break;
                    }
                } 
            }
            else if (firstItemOnConveyor.distanceOnConveyor > Item.Size)
            {
                parentChain.items.Insert(parentChain.items.IndexOf(firstItemOnConveyor) - 1, item);
                item.distance = conveyorDistanceOnChain;
                success = true;
            }
            else
            {
                return false;
            }

            Debug.Log(success);

            return success;*/
        }

        int GetDistanceOnChain()
        {
            return parentChain.conveyors.IndexOf(this) * Length; 
        }

        List<Item> GetItemsOnConveyor()
        {
            List<Item> itemsOnConveyor = new List<Item>();

            int distanceOnChain = GetDistanceOnChain();

            foreach (var item in parentChain.items)
            {
                if(item.distance < distanceOnChain - 1)
                {
                    continue;
                }
                if(item.GetConveyor(parentChain) == this)
                {
                    itemsOnConveyor.Add(item);
                }
                if (item.distance > distanceOnChain + Length++)
                {
                    break;
                }
            }

            return itemsOnConveyor;
        } 
    }
}