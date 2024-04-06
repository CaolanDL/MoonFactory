using System;
using System.Collections.Generic;

using ExtensionMethods;
using Logistics;
using UnityEngine;

public class Machine : Structure
{
    public Inventory[] InputInventories;
    public Inventory[] OutputInventories; 

    public override void OnInitialise()
    {
        InputInventories = new Inventory[StructureData.inputs.Count];
        OutputInventories = new Inventory[StructureData.outputs.Count];

        for (int i = 0; i < StructureData.inputs.Count; i++) { InputInventories[i] = new(); }
        for (int i = 0; i < StructureData.outputs.Count; i++) { OutputInventories[i] = new(); } 
    }

    public override void OnClicked(Vector3 mousePosition)
    {
        OpenInterface(mousePosition);
    } 

    // Input Output //
    #region Input Output

    public override void ConnectOuputs()
    {
        foreach (var output in StructureData.outputs)
        {
            var offsetPosition = output.position.Rotate(rotation) + position;
            var offsetRotation = output.rotation.Rotate(rotation);

            var entity = GameManager.Instance.gameWorld.worldGrid.GetEntityAt(offsetPosition);

            if (entity == null) continue;

            if (entity.GetType() == typeof(Conveyor))
            {
                var conveyor = (Conveyor)entity;

                if (conveyor.parentChain.conveyors[0] != conveyor) { continue; }

                if (conveyor.rotation == offsetRotation)
                {
                    conveyor.SetRotationConfig(Conveyor.TurnConfig.Straight);
                }
                else if (conveyor.rotation == offsetRotation.Rotate(1))
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
                    ItemOutput();


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
                ItemOutput();

                return true;
            }
        }

        return false;
    }

    public bool TryOutputItem(ResourceData resource, int outputIndex)
    {
        return TryOutputItem(resource, OutputInventories[outputIndex], StructureData.outputs[outputIndex]);
    }

    public bool TryOutputAnything(int outputIndex)
    {
        return TryOutputItem(OutputInventories[outputIndex].GetRandomResource(), outputIndex);
    }

    private void ItemOutput()
    {
        TryBeginCrafting();
        TryUpdateInterface();

        OnItemOutput();
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

        foreach (var input in StructureData.inputs)
        {
            if (input.position.Equals(relativeInputTransform.position))
            {
                invIndex = StructureData.inputs.IndexOf(input);
                break;
            }
        }

        if (invIndex == -1) { return false; }

        Inventory inputInventory = InputInventories[invIndex];

        if (inputInventory == null) return false;

        if (inputInventory.GetMaxAcceptable(resource) == 0) return false;

        if (inputInventory.TryAddResource(resource, 1))
        {
            ItemInput();

            return true;
        }

        return false;
    }

    void ItemInput()
    {
        TryBeginCrafting();
        TryUpdateInterface();

        OnItemInput();
    }

    public virtual void OnItemInput()
    {

    }

    #endregion

    // Inventory Management //
    #region Inventory Management

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

        if (resource == null) return false;

        if (inventoryTo.GetMaxAcceptable(resource) == 0) return false;

        if (inventoryTo.TryAddResource(resource, 1))
        {
            inventoryFrom.RemoveResource(resource, 1);
            //Debug.Log(inventoryTo.totalItems);
            return true;
        }
        return false;
    }

    #endregion

    // Crafting //
    #region Crafting

    /// <summary> Crafting Forumula index in use </summary>
    public byte activeCFIndex = 0;

    byte newCFIndex = 0;

    public short craftingCountdown = 0;

    public bool isCrafting = false;

    public bool isCrafter = false;


    public void SetNewCF(CraftingFormula craftingFormula)
    {
        newCFIndex = (byte)StructureData.CraftingFormulas.IndexOf(craftingFormula);
    }

    /// <summary> Returns the crafting formula at a specific index </summary> 
    public CraftingFormula GetCraftingFormula(byte CFIndex)
    {
        return StructureData.CraftingFormulas[CFIndex];
    }
    /// <summary> Returns the active crafting formula </summary> 
    public CraftingFormula GetCraftingFormula()
    {
        return StructureData.CraftingFormulas[activeCFIndex];
    }

    void TryUpdateCF()
    {
        if (activeCFIndex != newCFIndex)
        {
            activeCFIndex = newCFIndex;

            UpdateCFVariables(StructureData.CraftingFormulas[activeCFIndex]);
        }
    }

    void UpdateCFVariables(CraftingFormula cf)
    {
        craftingCountdown = cf.duration;
    }

    public void TickCrafting()
    {
        if (isCrafting)
        {
            craftingCountdown--;

            if (craftingCountdown < 0)
            {
                FinishCrafting();
            }
        }
    }

    public bool TryBeginCrafting()
    {
        if (isCrafter == false) { return false; }

        if (isCrafting) return false;

        TryUpdateCF();

        if (InputInventories == null) return false;
        if (OutputInventories == null) return false;

        // Check if input inventories are empty
        bool inventoriesEmpty = true;
        foreach (var inventory in InputInventories) { if (inventory.totalItems > 0) { inventoriesEmpty = false; break; } }
        if (inventoriesEmpty) return false;

        // Get the current crafting formula
        CraftingFormula cf = StructureData.CraftingFormulas[activeCFIndex];

        if (cf == null) return false;
        if (cf.OutputResources.Count > OutputInventories.Length) { throw new Exception("Crafting formula outputs exceed outputs available on structure"); }

        // Check if any output inventories are full
        for (int i = 0; i < cf.OutputResources.Count; i++)
        {
            if (OutputInventories[i].GetMaxAcceptable(cf.OutputResources[i].resource) < cf.OutputResources[i].quantity)
            {
                return false;
            }
        }

        // Create a dictionary of input fulfilment requirements
        Dictionary<ResourceQuantity, bool> fullfilmentRegister = new Dictionary<ResourceQuantity, bool>();

        // Loop through inventories, adding & toggling entries in fullfilment register if resource quantity is in an input inventory
        foreach (ResourceQuantity resourceQuantity in cf.InputResources)
        {
            fullfilmentRegister.Add(resourceQuantity, false);

            bool hasResourceQuanity = false;
            foreach (Inventory inventory in InputInventories)
            {
                if (inventory.GetQuantityOf(resourceQuantity.resource) >= resourceQuantity.quantity)
                {
                    hasResourceQuanity = true;
                    break;
                }
            }
            if (hasResourceQuanity) { fullfilmentRegister[resourceQuantity] = true; }
        }

        // Exit crafting if resource requirements not met
        foreach (var keyValuePair in fullfilmentRegister)
        {
            if (keyValuePair.Value == false) return false;
        }

        /*        foreach (ResourceQuantity resourceQuantity in cf.InputResources)
                {
                    fullfilmentRegister.Add(resourceQuantity, false);

                    bool hasResourceQuanity = false;
                    foreach (Inventory inventory in InputInventories)
                    {
                        if (inventory.GetQuantityOf(resourceQuantity.resource) >= resourceQuantity.quantity)
                        {
                            hasResourceQuanity = true;
                            break;
                        }
                    }
                    if (hasResourceQuanity) { fullfilmentRegister[resourceQuantity] = true; } 
                }*/

        // Remove resources from input inventories
        foreach (Inventory inventory in InputInventories)
        {
            if (inventory.totalItems == 0) { continue; }

            List<ResourceQuantity> resourcesToRemoveFromRegister = new();

            foreach (var keyValuePair in fullfilmentRegister)
            {
                if (inventory.GetQuantityOf(keyValuePair.Key.resource) >= keyValuePair.Key.quantity)
                {
                    inventory.RemoveResource(keyValuePair.Key.resource, keyValuePair.Key.quantity);

                    resourcesToRemoveFromRegister.Add(keyValuePair.Key);
                }
            }

            foreach (var key in resourcesToRemoveFromRegister)
            {
                fullfilmentRegister.Remove(key);
            }
        }

        craftingCountdown = cf.duration;
        isCrafting = true;

        OnBeginCrafting();

        return true;
    }

    public virtual void OnBeginCrafting()
    {

    }

    public void FinishCrafting()
    {
        // Get the current crafting formula
        CraftingFormula cf = StructureData.CraftingFormulas[activeCFIndex];

        // Add output resources to output inventories
        for (int i = 0; i < cf.OutputResources.Count; i++)
        {
            OutputInventories[i].TryAddResource(cf.OutputResources[i].resource, cf.OutputResources[i].quantity);
            TryOutputAnything(i);
        }

        isCrafting = false;

        OnCraft();

        if (TryBeginCrafting()) return;

        OnStopCrafting();
    }

    public virtual void OnCraft()
    {

    }

    public virtual void OnStopCrafting()
    {

    }

    #endregion


    #region Interface Handling

    bool isInterfaceOpen = false;
    static ModularInterface activeInterface;

    //TODO Should really modifiy this to a different creational design pattern. Maybe pass the call off to the structure subclass in question with a virtual method
    //TODO and let the subclass handle the HUD call. Would prefer not to have a large switch statement in the HUD class to dependently spawn interfaces.
    // Defaults to opening a crafting machine interface.
    public virtual void OpenInterface(Vector3 mousePosition) 
    {
        OpenInterfaceOnHUD(MenuData.Instance.CraftingMachineInterface, mousePosition);
    }

    public void OpenInterfaceOnHUD(GameObject interfacePrefab, Vector3 mousePosition)
    {
        var success = GameManager.Instance.HUDController.OpenInterface(interfacePrefab, this, mousePosition);

        if (success)
        {
            isInterfaceOpen = true;
            activeInterface = GameManager.Instance.HUDController.openInterface;
        }

        TryUpdateInterface();
    }

    public void OnInterfaceClosed()
    {
        isInterfaceOpen = false;
    }

    public virtual void TryUpdateInterface()
    {
        if (isInterfaceOpen)
        {
            activeInterface.UpdateUI();
        }
    }

    #endregion
}
