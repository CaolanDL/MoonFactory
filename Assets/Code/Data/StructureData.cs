using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

enum StructureCategory
{
    Logistics,
    Power,
    Processing,
    Manufacturing,
    Research
}


[CreateAssetMenu(menuName = "MoonFactory/Structure Data")]
public class StructureData : ScriptableObject
{
    [Header("Structure Details")]

    public string screenname = string.Empty;

    public Sprite sprite;

    [SerializeField] public GameObject displayObject;  

    public List<int2> inputLocations;
    public List<int2> outputLocations;


    [Space, Header("Ghost Data")]

    public Mesh ghostMesh; 

    public List<GhostModels> ghostModels = new(); 

    public List<ArrowIndicatorData> arrowIndicators = new();



    [Serializable]
    public class GhostModels
    {
        public Mesh mesh;
        public Material material;
    }

    [Serializable]
    public class ArrowIndicatorData
    {
        public Vector3 relativePosition = new();

        public Quaternion rotation = Quaternion.identity;

        public float size = 1;
    }
}

