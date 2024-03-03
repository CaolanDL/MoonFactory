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
    [Space]

    public string screenname = string.Empty;

    public Sprite sprite;

    [Space]

    public Mesh mesh; 

    public List<AdditiveModelData> additiveModels = new();

    [Space]

    public List<int2> inputLocations;
    public List<int2> outputLocations;

    [Space]

    public List<ArrowIndicatorData> arrowIndicators = new();

    [Serializable]
    public class AdditiveModelData
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

