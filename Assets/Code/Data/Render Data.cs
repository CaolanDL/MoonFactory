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

    [Header("Development")]
    public Material devgizmo_material;

    [Header("Wire")]
    public Mesh wire_mesh;
    public Material wire_material;  

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