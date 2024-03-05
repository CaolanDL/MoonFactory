using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MoonFactory/Singletons/WorldGenerationData")]

public class WorldGenerationData : ScriptableObject
{
    #region Make Singleton
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
    #endregion 

    [SerializeField] public List<FloorTileData> floorTiles;

    public static int ChunkSize = 9;

    public static int StartZoneSize = 9;

    public float GenerationScalar = 9; 
}
