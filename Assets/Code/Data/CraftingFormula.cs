using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Crafting Formula")]
public class CraftingFormula : ScriptableObject
{
    // Editor Variables // 
    [Tooltip("Machine crafted in")] public StructureData machine;

    [Tooltip("Duration in Ticks (s*50)")] public short duration;

    public Sprite sprite;

    [SerializeField] public List<ResourceQuantity> InputResources;

    [SerializeField] public List<ResourceQuantity> OutputResources;

    // Runtime Data //
    public bool unlocked = false;

    public void Unlock()
    {
        unlocked = true;
        GlobalData.Instance.unlocked_CraftingFormulas.Add(this);
    }
} 