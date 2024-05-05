using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Global Data")]
public class GlobalData : ScriptableObject
{ 
    public static GlobalData Instance { get; private set; }

    [SerializeField] public PlayerInputActions inputActions;

    [Header("Registry")]
    [SerializeField] public List<StructureData> Structures;
    [NonSerialized] public List<StructureData> unlocked_Structures = new();

    [SerializeField] public List<ResourceData> Resources;
    [NonSerialized] public List<ResourceData> unlocked_Resources = new();

    [SerializeField] public List<StructureData> UnlockedOnStart = new();
    [SerializeField] public List<ResourceQuantity> StarterResources = new();

    [Space(8), Header("Global Materials")]
    public Material mat_DevUniversal;
    [Space]
    public Material mat_Ghost;
    public Material mat_GhostBlocked;
    [Space]
    public Material mat_Tile;


    [Space(8), Header("Gizmos")]
    public Mesh gizmo_Arrow;
    public Material mat_ArrowIndicatorInput;
    public Material mat_ArrowIndicatorOutput;
    [Space]
    public Mesh gizmo_TileGrid;
    public Material mat_PulsingGizmo;
    [Space]
    public Mesh gizmo_Axis;


    private void OnEnable()
    {
        MakeSingleton();
        SyncMachineCraftingReferences();
        //UpdateStructureCFAssignment();
    }

    private void OnValidate()
    {
        MakeSingleton();
        //UpdateStructureCFAssignment();
    }

    public void MakeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
     
/*    public void UpdateStructureCFAssignment()
    {
        foreach (var structure in Structures)
        {
            structure.CraftingFormulas.Clear();

            foreach(CraftingFormula formula in CraftingFormulas.Where(formula => formula.machine.Equals(structure)))
            {
                structure.CraftingFormulas.Add(formula);
            }
        }  
    }*/
     
    public void SyncMachineCraftingReferences()
    {
        foreach (var structure in Structures)
        {
            structure.CraftableResources.Clear();

            foreach (ResourceData resource in Resources.Where(resource => resource.craftedIn.Equals(structure)))
            {
                structure.CraftableResources.Add(resource);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GlobalData))]
public class GlobalDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GlobalData editorInstance = target as GlobalData;
        
        /*if(GUILayout.Button("Update Structure CF Assignment"))
        {
            editorInstance.UpdateStructureCFAssignment();
        }*/
        if (GUILayout.Button("Sync Machine Crafting References"))
        {
            editorInstance.SyncMachineCraftingReferences();
        }

        DrawDefaultInspector();

    }
}
#endif