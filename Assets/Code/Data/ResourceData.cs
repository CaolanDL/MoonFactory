using System;
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

    [Header("Rendering")] 
    [SerializeField] public Mesh mesh;
    [SerializeField] public Material material;

    [SerializeField] public Sprite sprite;

    [Header("Details")]
    [SerializeField] public byte weight = 1;

    //Runtime Data
    public bool unlocked = false;

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

