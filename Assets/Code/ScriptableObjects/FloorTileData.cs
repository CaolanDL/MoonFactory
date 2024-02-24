using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/FloorTile")]
public class FloorTileData : ScriptableObject
{
    public Mesh mesh;
    public Material material;

    public int chanceToSpawn = 1;
}
