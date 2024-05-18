using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/RenderData")]
public class RenderData : ScriptableObject
{
    public static RenderData Instance { get; private set; }

    [Header("Animation Assets")]
    public GameObject LanderSequence;

    [Header("Development")]
    public Material devgizmo_material;

    [Header("Universal")]
    public Material UniversalMaterial;

    [Header("Effects")]
    public GameObject ConstructedEffect;

    [Header("Ghosts")]
    public Material StructureGhost_Mouse;
    public Material StructureGhost_Blocked;
    public Material StructureGhost_World;

    [Header("Port Indication")]
    public Mesh Arrow;
    public Mesh TwoWayArrow;
    public Material ArrowOutputMaterial;
    public Material ArrowInputMaterial;

    [Header("Icons")]
    public Texture DemolishIcon;

    [Header("Gizmos")]
    public Mesh TileHighlightGizmo;
    public Mesh TilesGizmo;
    public Material BlueGizmoMaterial;
    public Material TransparentBlueGizmoMaterial;
    [Space]
    public Mesh TilePowerGizmo;
    public Material TilePowerGizmoMaterial; 
    [Space]
    public Mesh SelectionGizmo;
    public Material SelectionGizmoMaterial;
    public Material SelectedInterfaceGizmoMaterial;

    [Header("Terrain")]
    public List<Mesh> Rubble;
    public Material RubbleMaterial;

    [Header("Wire")]
    public Mesh wire_mesh;
    public Material wire_material;

    [Header("Bridges")] 
    public Mesh BridgeSegment;
    public Mesh BridgePlatform;





    // Singleton Instanciation //

    private void OnEnable() { MakeSingleton(); }

    private void OnValidate() { MakeSingleton(); }

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
}