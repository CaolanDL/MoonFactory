using System;
using System.Collections.Generic;

using ExtensionMethods;
using Logistics;
using UnityEngine;

public class Machine : Structure
{
    public List<Inventory> InputInventories = new();
    public List<Inventory> OutputInventories = new();

    public override void ConnectOuputs()
    {
        foreach (var output in structureData.outputs)
        {
            var offsetPosition = output.position.Rotate(rotation) + position;
            var offsetRotation = output.rotation.Rotate(rotation);

            var entity = GameManager.Instance.gameWorld.worldGrid.GetEntityAt(offsetPosition);

            if (entity == null) continue;

            if(entity.GetType() == typeof(Conveyor))
            {
                var conveyor = (Conveyor)entity;

                if (conveyor.parentChain.conveyors[0] != conveyor) { continue; }

                if (conveyor.rotation == offsetRotation)
                {
                    conveyor.SetRotationConfig(Conveyor.TurnConfig.Straight);
                }
                else if(conveyor.rotation == offsetRotation.Rotate(1))
                {
                    conveyor.SetRotationConfig(Conveyor.TurnConfig.RightTurn);
                }
                else if (conveyor.rotation == offsetRotation.Rotate(-1))
                {
                    conveyor.SetRotationConfig(Conveyor.TurnConfig.LeftTurn);
                }
            }
        }
    } 

    public override void OnInitialise()
    {
        for (int i = 0; i < structureData.inputs.Count; i++) { InputInventories.Add(new()); }
        for (int i = 0; i < structureData.outputs.Count; i++) { OutputInventories.Add(new()); }
    }  

    public bool TryOutputItem(ResourceData resource, Inventory inventory, TinyTransform outputTransform)
    {
        var worldGrid = GameManager.Instance.gameWorld.worldGrid;

        var entityAtLocation = worldGrid.GetEntityAt(position + (outputTransform.position.ToVector2().Rotate(rotation * 90).ToInt2()));

        if (entityAtLocation == null) return false;

        if (inventory.GetQuantityOf(resource) == 0) return false;

        // Output to first conveyor in chain.
        if (entityAtLocation.GetType() == typeof(Conveyor))
        {
            var conveyor = (Conveyor)entityAtLocation;

            if (conveyor.parentChain.conveyors.IndexOf(conveyor) == 0)//entityAtLocation.rotation == outputTransform.rotation.Rotate(rotation))
            {
                if (conveyor.inputPosition.Equals(position) == false) return false;

                if (conveyor.parentChain.TryAddFirstItem(resource))
                {
                    inventory.RemoveResource(resource, 1);
                    OnItemOutput();
                    return true;
                }
            }
        }
        else if (entityAtLocation.GetType().IsSubclassOf(typeof(Machine)))
        {
            Machine otherMachine = (Machine)entityAtLocation;
            TinyTransform offsetOutputTransform = new TinyTransform(outputTransform.position - outputTransform.rotation.ToInt2() + position, outputTransform.rotation.Rotate(rotation));

            if (otherMachine.TryInputItem(resource, offsetOutputTransform))
            {
                inventory.RemoveResource(resource, 1);
                OnItemOutput();
                return true;
            }
        }

        return false;
    }

    public bool TryOutputAnything(Inventory inventory, TinyTransform outputTransform)
    {
        return TryOutputItem(inventory.GetRandomResource(), inventory, outputTransform);
    }

    public virtual void OnItemOutput()
    {

    }

    public bool TryInputItem(ResourceData resource, TinyTransform inputTransform)
    {
        int invIndex = -1;

        TinyTransform relativeInputTransform = (inputTransform - transform);

        relativeInputTransform.rotation += rotation;

        relativeInputTransform.position = relativeInputTransform.position.Rotate((sbyte)-rotation); 

        foreach (var input in structureData.inputs)
        {
            if (input.position.Equals(relativeInputTransform.position))
            {
                invIndex = structureData.inputs.IndexOf(input);
                break;
            }
        }

        if (invIndex == -1) { return false; }

        Inventory inputInventory = InputInventories[invIndex];

        if (inputInventory == null) return false;

        if (inputInventory.GetMaxAcceptable(resource) == 0) return false;

        if (inputInventory.TryAddResource(resource, 1))
        {
            OnItemInput();
            return true;
        }

        return false;
    }

    public virtual void OnItemInput()
    {

    }

    public bool TryTransferAllOrNothing(Inventory inventoryFrom, Inventory inventoryTo, ResourceData resource, int quantity)
    {
        if (inventoryFrom.GetQuantityOf(resource) < quantity) return false;
        if (inventoryTo.GetMaxAcceptable(resource) < quantity) return false;

        if (inventoryTo.TryAddResource(resource, quantity))
        {
            inventoryFrom.RemoveResource(resource, quantity);
            return true;
        }

        return false;
    }

    public int TransferWithRemainder(Inventory inventoryFrom, Inventory inventoryTo, ResourceData resource, int quantity)
    {
        int remainder = quantity;

        for (int i = 0; i < quantity; i++)
        {
            if (inventoryTo.TryAddResource(resource, 1))
            {
                remainder--;
                inventoryFrom.RemoveResource(resource, 1);
            }
            else
            {
                break;
            }
        }

        return remainder;
    }

    public bool TransferSingle(Inventory inventoryFrom, Inventory inventoryTo, ResourceData resource)
    {
        if (inventoryTo.TryAddResource(resource, 1))
        {
            inventoryFrom.RemoveResource(resource, 1);
            return true;
        }
        return false;
    }

    public bool TransferAnythingRandom(Inventory inventoryFrom, Inventory inventoryTo)
    {
        var resource = inventoryFrom.GetRandomResource();

        if(resource == null) return false;

        if(inventoryTo.GetMaxAcceptable(resource) == 0) return false;

        if (inventoryTo.TryAddResource(resource, 1))
        {
            inventoryFrom.RemoveResource(resource, 1); 
            //Debug.Log(inventoryTo.totalItems);
            return true;
        }
        return false;
    }
}
