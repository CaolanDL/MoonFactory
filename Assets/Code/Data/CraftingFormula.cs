using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Crafting Formula")]
public class CraftingFormula : ScriptableObject
{
    [Tooltip("Machine crafted in")] public StructureData machine;

    [Tooltip("Duration in Ticks (s*50)")] public short duration;

    [SerializeField] public List<ResourceQuantity> InputResources;

    [SerializeField] public List<ResourceQuantity> OutputResources;
}

[Serializable]
public struct ResourceQuantity
{
    [SerializeField] public ResourceData resource;
    [SerializeField] public int quantity;
     
    public ResourceQuantity(ResourceData resource, int quantity)
    {
        this.resource = resource;
        this.quantity = quantity;
    }
}


