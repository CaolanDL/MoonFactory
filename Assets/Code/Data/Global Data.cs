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
    public List<StructureData> structures;
 
    public List<ResourceData> resources;

    [ContextMenuItem("Update Structure CF Assignment", "UpdateStructureCFAssignment")] 
    public List<CraftingFormula> craftingFormulas; 


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
        UpdateStructureCFAssignment();
    }

    private void OnValidate()
    {
        MakeSingleton();
        UpdateStructureCFAssignment();
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

#if UNITY_EDITOR
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
#endif