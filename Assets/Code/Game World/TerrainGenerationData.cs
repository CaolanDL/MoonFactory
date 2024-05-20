using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MoonFactory/Singletons/WorldGenerationData")]

public class TerrainGenerationData : ScriptableObject
{ 
    public static TerrainGenerationData Instance { get; private set; }

    public static int ChunkSize = 9;
    public static int StartZoneSize = 9;
    public float GenerationScalar = 9;

    [SerializeField] public float chanceToSpawnMeteorite = 0.02f;
    [SerializeField] public float maxMeteoriteScale = 0.5f;
    [SerializeField] public float minMeteoriteScale = 1f;

    [SerializeField] public FloorTileData displaceTile;

    [SerializeField] public ModelData[] MeteorModels;

    [SerializeField] public List<FloorTileData> topographyTiles;
     
    [SerializeField] public List<FloorTileData> randomTiles;

    [SerializeField] public List<FloorTileData> craterTiles;

    [SerializeField] public List<FloorTileData> rubbleTiles;

    [NonSerialized] public  List<FloorTileData> tileRegistry = new(); 

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

        LoopListAndRegister(topographyTiles);
        LoopListAndRegister(randomTiles);
        LoopListAndRegister(craterTiles);
        LoopListAndRegister(rubbleTiles);

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
