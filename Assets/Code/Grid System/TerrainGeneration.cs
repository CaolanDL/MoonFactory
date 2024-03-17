
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGenerator
{
    public int seed = 0;

    public TerrainGenerator(int seed)
    {
        this.seed = seed;
    }

    public FloorTileData ChooseTileAt(int2 position)
    {
        WorldGenerationData generationData = WorldGenerationData.Instance;

        // World generation system goes here
        // --> 

        float scalar = WorldGenerationData.Instance.GenerationScalar;

        float perlin = Mathf.PerlinNoise(position.x / scalar + 999999, position.y / scalar + 999999);

        int tileCount = generationData.topographyTiles.Count;

        int tileIndex = Mathf.FloorToInt(Mathf.Clamp(Mathf.Clamp01(perlin) * (tileCount), 0, tileCount-1));

        //Debug.Log($"Position: {position} | Perlin Value: {perlinValue} | Tile Index: {tileIndex}");

        return generationData.topographyTiles[tileIndex];

        // <--
        // World generation system goes here

        return (generationData.topographyTiles[Random.Range(0, generationData.topographyTiles.Count - 1)]); // Returning tile at index 0 only
    }
}

