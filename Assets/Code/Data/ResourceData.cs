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


    //Runtime Data
    [NonSerialized] public bool unlocked = false;

    public void Unlock()
    {
        unlocked = true;
        if (!GameManager.Instance.ScienceManager.unlocked_Resources.Contains(this))
        { 
            GameManager.Instance.ScienceManager.unlocked_Resources.Add(this);
        }
    }
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

    public static bool operator ==(ResourceQuantity a, ResourceQuantity b)
    {
        return a.resource == b.resource && a.quantity == b.quantity;
    }
    public static bool operator !=(ResourceQuantity a, ResourceQuantity b)
    {
        return !(a.resource == b.resource && a.quantity == b.quantity);
    }
}

