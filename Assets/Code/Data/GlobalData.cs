using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Global Data")]
public class GlobalData : ScriptableObject
{ 
    public static GlobalData Instance { get; private set; }

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

    [Header("Universal Assets")]
    public Material mat_DevUniversal;

    public Mesh Gizmo;

    [Header("Global Materials")]
    public Material mat_Ghost;
    public Material mat_GhostBlocked;

    public Material mat_Tile;

    [Header("Gizmos")]
    public Mesh m_ArrowIndicator;
    public Material mat_ArrowIndicator;

    public Mesh m_TileGizmo;
    public Material mat_PulsingGizmo;

    [Header("Registry")]
    public List<StructureData> structures;
 
    public List<ResourceData> resources;
}
