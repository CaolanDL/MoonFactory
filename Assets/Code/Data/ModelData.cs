using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class ModelData : ScriptableObject
{
    [SerializeField] public Mesh Mesh;
    [SerializeField] public Material Material;
} 