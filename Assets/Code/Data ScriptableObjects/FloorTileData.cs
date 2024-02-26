using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Floor Tile Data")]
public class FloorTileData : ScriptableObject
{
    public Mesh mesh;
    public Material material;

    public int chanceToSpawn = 1;
}
