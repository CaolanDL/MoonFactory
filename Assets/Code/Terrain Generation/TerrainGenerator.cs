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

    public FloorTileData GenerateTileAt(int2 position)
    {
        FloorTileData tile;

        tile = TryGenerateRubbleRegion();
        if (tile != null) return tile;

        tile = TryGenerateCraterCone();
        if (tile != null) return tile;

        tile = TryGenerateRandomTile();
        if (tile != null) return tile; 

        return GenerateTopographyTile(position); 
    }

    public FloorTileData TryGenerateCraterCone()
    {  
        return null;
    }

    public FloorTileData TryGenerateRubbleRegion()
    {
        return null;
    }

    public FloorTileData TryGenerateRandomTile()
    { 
        if(Random.Range(0, 50) == 1)
        {
            return TerrainGenerationData.Instance.randomTiles[Random.Range(0, TerrainGenerationData.Instance.randomTiles.Count-1)];
        }

        return null;
    }

    public FloorTileData GenerateTopographyTile(int2 position)
    {
        float scalar = TerrainGenerationData.Instance.GenerationScalar;

        float perlin = Mathf.PerlinNoise(position.x / scalar + 999999, position.y / scalar + 999999);

        int tileCount = TerrainGenerationData.Instance.topographyTiles.Count;

        int tileIndex = Mathf.FloorToInt(Mathf.Clamp(Mathf.Clamp01(perlin) * (tileCount), 0, tileCount - 1));

        //Debug.Log($"Position: {position} | Perlin Value: {perlinValue} | Tile Index: {tileIndex}");

        return TerrainGenerationData.Instance.topographyTiles[tileIndex];
    }
}

