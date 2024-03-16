using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using ExtensionMethods;
using System.Linq;  

namespace Logistics
{
    public class Item
    {
        public static int Size = Conveyor.Length / 5;

        public ResourceData data;

        public int distance = 1;

        public int distanceOnConveyor
        {
            get { return (distance - 1) % Conveyor.Length + 1; }
        }

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
            //Debug.Log("item created");
        }

        public Conveyor GetConveyor(Chain chain)
        {
            if (distance < Conveyor.Length) { return chain.conveyors[0]; }
            int index = Mathf.CeilToInt((float)distance / Conveyor.Length) - 1;
            return chain.conveyors[index];
        }



        static Conveyor currentConveyor;            
        static float normalisedDistanceOnConveyor;  
        static Vector2 positionOnConveyor;          
        static float turnFactor;                    
        static float remapNormalDistance;
        static float normalRotation;

        public void UpdateWorldPosition(Chain chain)
        {
            currentConveyor = GetConveyor(chain);

            normalisedDistanceOnConveyor = (float)distanceOnConveyor / Conveyor.Length;

            Vector2 linearPositionCalc()
            {
                return new Vector2(0, normalisedDistanceOnConveyor - 0.5f);
            }

            Rotation = 0;//(short)(currentConveyor.rotation * 90);

            // Item path of straight conveyors
            if (currentConveyor.turnConfig == Conveyor.TurnConfig.Straight)
            {
                positionOnConveyor = linearPositionCalc();
                Rotation = (short)(currentConveyor.rotation * 90);
            }

            // Item path of curved conveyors. This could probably do with some refactoring and optimisations but heyho it works.
            else
            {
                turnFactor = currentConveyor.turnConfig == Conveyor.TurnConfig.LeftTurn ? +90f : -90f; // Setting turn offset based on the turn configuration of the current belt 
                 

                if (normalisedDistanceOnConveyor <= Conveyor.TurnStartOffset)
                {
                    positionOnConveyor = linearPositionCalc().Rotate(turnFactor);
                    Rotation = (short)(currentConveyor.rotation * 90f + turnFactor);
                }
                else if (normalisedDistanceOnConveyor >= 1 - Conveyor.TurnStartOffset)
                {
                    positionOnConveyor = linearPositionCalc();
                    Rotation = (short)(currentConveyor.rotation * 90f);
                }
                else
                {
                    Vector2 turnOrigin;

                    if (currentConveyor.turnConfig == Conveyor.TurnConfig.LeftTurn)
                    {
                        turnOrigin = new Vector2(-0.5f + Conveyor.TurnStartOffset, Conveyor.TurnStartOffset);
                    }
                    else
                    {
                        turnOrigin = new Vector2(0.5f - Conveyor.TurnStartOffset, Conveyor.TurnStartOffset);
                    }

                    remapNormalDistance = math.remap(0f + Conveyor.TurnStartOffset, 1f - Conveyor.TurnStartOffset, 0f, 1f, normalisedDistanceOnConveyor);

                    normalRotation = remapNormalDistance * -turnFactor; 

                    positionOnConveyor = new Vector2(0, Conveyor.TurnStartOffset).RotateAround(turnOrigin, normalRotation);

                    positionOnConveyor = positionOnConveyor - (Vector2.up / 2);

                    positionOnConveyor = positionOnConveyor.Rotate(turnFactor);

                    Rotation = (short)(currentConveyor.rotation * 90 + turnFactor + normalRotation); 
                }
            }

            worldPosition = (positionOnConveyor).Rotate(90 * currentConveyor.rotation) + currentConveyor.position.ToVector2();
        }
    }

    public class ChainManager
    {
        public static List<Chain> chains = new();

        public static void UpdateChains()
        {
            foreach (Chain chain in chains)
            {
                chain.wasUpdatedThisTick = false;
            }

            foreach (Chain chain in chains)
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

        static int speed = 10;

        public bool wasUpdatedThisTick;
        bool ChainAtCapacity { get { return items.Count >= chainCapacity; } }

        public Chain()
        {
            ChainManager.chains.Add(this);
            //Debug.Log("New chain created");
        }

        public void Update()
        {
            IterateItems();

            wasUpdatedThisTick = true;
        }

        public void IterateItems()
        {
            TryTransferLastItem();

            if (items.Count == 0) { return; }

            int lastItemIndex = items.Count - 1;

            if (items[lastItemIndex].distance <= chainLength - 1)
            {
                items[lastItemIndex].distance += speed;

                if (items[lastItemIndex].distance >= chainLength)
                {
                    items[lastItemIndex].distance = chainLength - 1;
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
            if (items.Count == 0) return false;

            Conveyor lastConveyor = conveyors[conveyors.Count - 1];

            Item lastItem = items[items.Count - 1];

            if (lastItem.distance < chainLength - 1) return false;

            var entityToTransferTo = lastConveyor.GetNeighbor(0);

            if (entityToTransferTo == null) { return false; }

            if (entityToTransferTo.GetType().IsSubclassOf(typeof(Machine)))
            {
                Machine otherMachine = (Machine)entityToTransferTo;

                if (otherMachine.TryInputItem(lastItem.data, lastConveyor.transform))
                {
                    items.Remove(lastItem);
                    return true;
                }
            }
            return false;
        }

        public bool TryAddFirstItem(ResourceData resourceData)
        {
            if (ChainAtCapacity) return false;

            if (items.Count != 0 && items[0].distance <= Item.Size) { return false; }

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

            if (conveyors.IndexOf(conveyor) == 0 || conveyors.IndexOf(conveyor) == conveyors.Count - 1)
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

        public void MergeWith(Chain otherChain, bool isChainInfront)
        {
            if (isChainInfront)
            {
                conveyors.AddRange(otherChain.conveyors);
                items.AddRange(otherChain.items);
            }
            else
            {
                conveyors.InsertRange(0, otherChain.conveyors);
                items.InsertRange(0, otherChain.items);
            }
            foreach (var conveyor in otherChain.conveyors)
            {
                conveyor.parentChain = this;
            }

            ChainManager.chains.Remove(otherChain);
        }
    }

    public class Conveyor : Machine
    {
        public static int Length = 60;

        public static float TurnStartOffset = 0.20f;

        public Chain parentChain;

        public Conveyor nextConveyor;
        public Conveyor lastConveyor;

        public int2 inputPosition;

        public TurnConfig turnConfig = new();

        public enum TurnConfig
        {
            Straight,
            LeftTurn,
            RightTurn
        }

        public override void OnConstructed()
        { 
            inputPosition = new int2(0, -1).Rotate(rotation) + position;

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

                if (parentChain != null)
                {
                    // Call Merge on conveyorInfront.parentChain passing parentChain & conveyorInfront
                    conveyorInfront.parentChain.MergeWith(parentChain, false);
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

                else if (IsMachineOuputFacingMe(rearNeighbor)) { SetRotationConfig(TurnConfig.Straight); }
                else if (IsMachineOuputFacingMe(leftNeighbor)) { SetRotationConfig(TurnConfig.LeftTurn); }
                else if (IsMachineOuputFacingMe(rightNeighbor)) { SetRotationConfig(TurnConfig.RightTurn); }

                else { return; }

                if(conveyorFacingMe != null)
                { 
                    conveyorFacingMe.nextConveyor = this;
                    lastConveyor = conveyorFacingMe; 

                    if (nextConveyor != null)
                    {
                        // Call Merge on conveyorInfront.parentChain passing parentChain & conveyorInfront
                        conveyorFacingMe.parentChain.MergeWith(parentChain, true);
                    }
                    else
                    {
                        conveyorFacingMe.parentChain.AppendConveyor(this);
                        parentChain = conveyorFacingMe.parentChain;
                    }
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

                bool IsMachineOuputFacingMe(Entity entity)
                {
                    if (entity == null) { return false; }

                    if (entity.GetType().IsSubclassOf(typeof(Machine)))
                    {
                        var machine = (Machine)entity;
                        if (machine.structureData.outputs.Count == 0) return false;

                        foreach( var output in machine.structureData.outputs)
                        {
                            if (((output).position.Rotate(machine.rotation) + machine.position).Equals( position ))
                            {
                                return true;
                            }
                        } 
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

        public void SetRotationConfig(TurnConfig config)
        {
            if (this.turnConfig == config) { return; }

            this.turnConfig = config;

            if (config == 0)
            {
                displayObject.SetActiveModel("Default");
                inputPosition = new int2(0, -1).Rotate(rotation) + position;
            }
            if (config == TurnConfig.LeftTurn)
            {
                displayObject.SetActiveModel("Left Turn");
                inputPosition = new int2(0, -1).Rotate((sbyte)(rotation + 1)) + position;
            }
            if (config == TurnConfig.RightTurn)
            {
                displayObject.SetActiveModel("Right Turn");
                inputPosition = new int2(0, -1).Rotate((sbyte)(rotation - 1)) + position    ;
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
                if (item.distance < distanceOnChain - 1)
                {
                    continue;
                }
                if (item.GetConveyor(parentChain) == this)
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