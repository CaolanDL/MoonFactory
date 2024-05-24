using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceCategory
{
    Basic,
    Refined,
    Manufactured,
    Fabricated,
    Assembled
}

[CreateAssetMenu(menuName = "MoonFactory/Resource Data")]
public class ResourceData : ScriptableObject
{
    [Header("Details")]
    [SerializeField] public string description;
    [SerializeField] public ResourceCategory resourceCategory;

    [Header("Rendering")]
    [SerializeField] public Sprite sprite;
    [SerializeField] public Mesh mesh;
    [SerializeField] public Material material;

    [Header("Variables")]
    [SerializeField] public byte Weight = 1;
    [SerializeField] public int ResearchValue = 1;
    [SerializeField] public float SecondsToResearch = 10;

    [Header("Crafting")] 
    [Tooltip("Machine crafted in")] public StructureData craftedIn;
    [Tooltip("Duration in Ticks (s*50)")] public short timeToCraft = 1;
    [Tooltip("Duration in Ticks (s*50)")] public short quantityCrafted = 1;
    [SerializeField] public List<ResourceQuantity> requiredResources;
        
    public void Unlock()
    { 
        GameManager.Instance?.ScienceManager?.TryUnlockResource(this);
    }
} 