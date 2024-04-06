using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceCategory
{
    Basic,
    Refined,
    Manufactured
}

[CreateAssetMenu(menuName = "MoonFactory/Resource Data")]
public class ResourceData : ScriptableObject
{
    // Editor Variables
    [SerializeField] public string description;
    [SerializeField] public ResourceCategory resourceCategory;
    [SerializeField] public byte weight = 1;

    [Header("Rendering")] 
    [SerializeField] public Mesh mesh;
    [SerializeField] public Material material;

    [SerializeField] public Sprite sprite;

    [Header("Details")] 
    [Tooltip("Machine crafted in")] public StructureData craftedIn; 
    [Tooltip("Duration in Ticks (s*50)")] public short timeToCraft; 
    [SerializeField] public List<ResourceQuantity> requiredResources;


    //Runtime Data
    [NonSerialized] public bool unlocked = false;

    public void Unlock()
    {
        unlocked = true;
        GlobalData.Instance.unlocked_Resources.Add(this);
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
}

