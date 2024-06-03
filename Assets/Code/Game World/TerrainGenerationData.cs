using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "MoonFactory/Singletons/WorldGenerationData")]

public class TerrainGenerationData : ScriptableObject
{ 
    public static TerrainGenerationData Instance { get; private set; }

    public static int ChunkGenerationDistance = 5;
    public static int ChunkSize = 28;
    public static int StartZoneSize = 24;
    //public float GenerationScalar = 9;
    [Space]
    [SerializeField] public float chanceToSpawnMeteorite = 0.02f;
    [SerializeField] public float maxMeteoriteScale = 0.5f;
    [SerializeField] public float minMeteoriteScale = 1f;
    [Space]
    [SerializeField] public float chanceToSpawnCrater = 0.02f;
    [SerializeField] public float chanceToSpawnRubble = 0.1f;
    [Space]
    [SerializeField] public FloorTileData displaceTile;
    [SerializeField] public FloorTileData[] Tiles;
    [SerializeField] public ModelData[] MeteorModels; 
    [SerializeField] public List<CraterData> Craters;
    [SerializeField] public ModelData[] Rubble;

    //[SerializeField] public List<FloorTileData> topographyTiles; 
    //[SerializeField] public List<FloorTileData> randomTiles; 
    //[SerializeField] public List<FloorTileData> rubbleTiles;

    [NonSerialized] public List<FloorTileData> tileRegistry = new(); 

    private void OnValidate()
    {
        UpdateTileRegistry();
    }

    private void OnEnable()
    {
        MakeSingleton();
        UpdateTileRegistry();
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


    public void UpdateTileRegistry()
    {
        tileRegistry.Clear();

        LoopListAndRegister(Tiles.ToList()); 

        tileRegistry.Add(displaceTile);

        void LoopListAndRegister(List<FloorTileData> list)
        {
            foreach (FloorTileData tile in list)
            {
                if (tileRegistry.Contains(tile)) { continue; }
                tileRegistry.Add(tile);
            }
        }
    }
}
