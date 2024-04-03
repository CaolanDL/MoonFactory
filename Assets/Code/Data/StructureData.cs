using DataStructs;
using System; 
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;  

public enum StructureCategory
{
    Logistics,
    Power,
    Processing,
    Manufacturing,
    Science
} 

[CreateAssetMenu(menuName = "MoonFactory/Structure Data")]
public class StructureData : ScriptableObject
{ 
    [Header("Structure Details")] 
    public string screenname = string.Empty;

    [Multiline]
    public string description = string.Empty;

    public StructureCategory category;
    public Sprite sprite;
    [SerializeField] public GameObject displayObject; 

    [Space]
    [SerializeField] public int2 size = new(1, 1);

    public Vector2 centre
    {
        get { return new Vector2(size.x / 2f, size.y / 2f); }
    }

    [Header("Construction")] 
    public List<ResourceQuantity> requiredResources = new();
    [Tooltip("In Ticks")] public int timeToBuild = 99;

    [Header("Crafting")]
    public List<CraftingFormula> CraftingFormulas = new();

    public List<TinyTransform> inputs;
    public List<TinyTransform> outputs;  

    [Space, Header("Ghost Data")]
    public Mesh ghostMesh; 
    public List<GhostModels> ghostModels = new(); 
     

    [Serializable]
    public class GhostModels
    {
        public Mesh mesh;
        public Material material;
    } 
}

[CreateAssetMenu(menuName = "MoonFactory/Configurable Structure Data")]
public class ConfigurableStructureData : StructureData
{
    public List<StructureData> Configurations; 
} 