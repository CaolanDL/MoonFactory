using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MoonFactory/Singletons/WorldGenerationData")]

public class WorldGenerationData : ScriptableObject
{
    private void OnEnable()
    {
        MakeSingleton();
    }

    #region Singleton Instanciation Method
    public static WorldGenerationData Instance { get; private set; }

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
    #endregion


    public static int ChunkSize = 32;
    public static int StartZoneSize = 256;

    [SerializeField] public List<FloorTileData> floorTiles;

    [SerializeField] public FloorTileData devFloorTile;
}
