using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Global Data")]
public class GlobalData : ScriptableObject
{ 
    public static GlobalData Instance { get; private set; } 

    [Header("Registry")]
    public List<StructureData> structures;
 
    public List<ResourceData> resources;

    [ContextMenuItem("Update Structure CF Assignment", "UpdateStructureCFAssignment")] 
    public List<CraftingFormula> craftingFormulas; 


    [Header("Tool Tips")]
    public GameObject BuildButtonTooltip;

    [Header("Interfaces")]
    public GameObject GenericInterface;



    [Space(32), Header("Global Materials")]
    public Material mat_DevUniversal;
    [Space]
    public Material mat_Ghost;
    public Material mat_GhostBlocked;
    [Space]
    public Material mat_Tile;

    [Header("Gizmos")]
    public Mesh m_ArrowIndicator;
    public Material mat_ArrowIndicatorInput;
    public Material mat_ArrowIndicatorOutput;
    [Space]
    public Mesh m_TileGizmo;
    public Material mat_PulsingGizmo;
    [Space]
    public Mesh Gizmo;


    private void OnEnable()
    {
        MakeSingleton();
    }

    private void OnValidate()
    {
        MakeSingleton();
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

    [ContextMenu("Update Structure CF Assignment")]
    public void UpdateStructureCFAssignment()
    {
        foreach (var structure in structures)
        {
            structure.CraftingFormulas.Clear();

            foreach(CraftingFormula formula in craftingFormulas.Where(formula => formula.machine.Equals(structure)))
            {
                structure.CraftingFormulas.Add(formula);
            }
        }  
    }
}

[CustomEditor(typeof(GlobalData))]
public class GlobalDataInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GlobalData editorInstance = target as GlobalData;
        
        if(GUILayout.Button("Update Structure CF Assignment"))
        {
            editorInstance.UpdateStructureCFAssignment();
        }

        DrawDefaultInspector();

    }
}