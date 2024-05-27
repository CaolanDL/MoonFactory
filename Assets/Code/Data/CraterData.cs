using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Crater Data")]
public class CraterData : ScriptableObject
{
    [SerializeField] public Mesh mesh;
    [SerializeField] public int size;
}
