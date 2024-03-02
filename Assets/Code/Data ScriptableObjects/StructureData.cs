using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


[CreateAssetMenu(menuName = "MoonFactory/Structure Data")]
public class StructureData : ScriptableObject
{
    public StructureType structureType = StructureType.None;

    [Space]

    public string screenname = string.Empty;

    [Space]

    public Mesh mesh;
    public Sprite sprite;

    [Space]

    public List<ArrowIndicatorData> arrowIndicators = new();

    [Serializable]
    public class ArrowIndicatorData
    {
        public Vector3 relativePosition = new();

        public Quaternion rotation = Quaternion.identity;

        public float size = 1;

        public Color color = new Color(50,255,50) ;
    }
}

