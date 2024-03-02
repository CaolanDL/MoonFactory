using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MoonFactory/Singletons/WorldGenerationData")]

public class WorldGenerationData : ScriptableObject
{
    public static WorldGenerationData Instance { get; private set; }

    private void OnEnable()
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


    public static int ChunkSize = 9;

    public static int StartZoneSize = 9;

    [SerializeField] public List<FloorTileData> floorTiles;

    [SerializeField] public FloorTileData devFloorTile;
}
