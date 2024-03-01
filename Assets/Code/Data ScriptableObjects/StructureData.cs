using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Structure Data")]
public class StructureData : ScriptableObject
{
    public StructureType structureType = StructureType.None;

    public Mesh mesh;
    public Material material; 
}