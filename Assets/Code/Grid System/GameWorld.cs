using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameWorld
{
    private TerrainGenerator terrainGenerator;


    public Grid floorGrid = new Grid();

    public Grid worldGrid = new Grid();

    List<Entity> entities = new List<Entity>();


    public GameWorld(int seed)
    {
        terrainGenerator = new TerrainGenerator(seed);
    }


    public void GenerateChunk(int2 position)
    { 
        int2 chunkWorldPosition = position * WorldGenerationData.ChunkSize;

        int2 chunkEndPositon = chunkWorldPosition + WorldGenerationData.ChunkSize;

        GenerateRegion(chunkWorldPosition, chunkEndPositon);
    } 

    public int2 GetChunkAt(int2 position)
    {
        int2 unflooredChunkPosition = position / WorldGenerationData.ChunkSize;
        return new int2(Mathf.FloorToInt(unflooredChunkPosition.x), Mathf.FloorToInt(unflooredChunkPosition.y));
    }

    public void GenerateRegion(int2 startPosition, int2 endPosition)
    { 
        //Modify start and end positions to point in positive iteration direction
        if (startPosition.x > endPosition.x)
        {
            (startPosition.x, endPosition.x) = (endPosition.x, startPosition.x); 
        }
        if (startPosition.y > endPosition.y)
        {
            (startPosition.y, endPosition.y) = (endPosition.y, startPosition.y); 
        }
/*
        // Find the direction to iterate towards
        int2 size = startPostion - endPosition;

        sbyte xIterationDirection = (sbyte)-MathF.Sign(size.x);
        sbyte yIterationDirection = (sbyte)-MathF.Sign(size.y);*/

        for (int x = startPosition.x; x < endPosition.x; x++)
        {
            for (int y = startPosition.y; y < endPosition.y; y++)
            {
                GenerateFloorTile(new int2(x, y));
            }
        }
    }

    public FloorTile GenerateFloorTile(int2 position)
    {
        (FloorTileData newTileData, int darkness) = terrainGenerator.ChooseTileAt(position);

        FloorTile newFloorTile = new(newTileData);

        floorGrid.AddEntity(newFloorTile, position);

        //Debug.Log($"Generated floor tile at: {position}");

        return newFloorTile;
    }

    public void DebugLogLocations()
    { 
        UnityEngine.Debug.Log($"{floorGrid.grid.Count} Grid locations spawned"); 
    }
}


public class TerrainGenerator
{
    public int seed = 0;

    public TerrainGenerator(int seed)
    {
        this.seed = seed;
    }

    public (FloorTileData, int) ChooseTileAt(int2 position)
    {
        ref WorldGenerationData generationData = ref GameManager.Instance.worldGenerationData;
        // World generation system goes here
        // 

        byte color = (byte) UnityEngine.Random.Range(0, 255);

        return (generationData.devFloorTile, color); // Returning tile at index 0 only
    }
}


public class Grid
{
    static List<Grid> grids = new List<Grid>();
    public Dictionary<int2, Location> grid = new Dictionary<int2, Location>();

    public List<Entity> entities = new List<Entity>(); 

    public byte id;

    public Grid()
    {
        grids.Add(this);
        id = (byte)(grids[grids.Count - 1].id + 1);
    }

    public Location AddLocation(int2 position)
    {
        if (LocationExists(position)) { return grid[position]; }

        Location newLocation = new Location()
        {
            gridId = id,
            position = position,
        };

        grid.Add(position, newLocation);

        return newLocation;
    }

    public bool LocationExists(int2 position)
    {
        if (grid.ContainsKey(position))
        {
            return true;
        }
        else return false;
    }

    public Location GetLocationAt(int2 position)
    {
        if (grid.ContainsKey(position))
        {
            return grid[position];
        }
        else return null;
    }

    public Entity AddEntity(Entity entity, int2 position)
    {
        Location location = AddLocation(position);

        location.entity = entity;

        return entity;
    }

    public Entity GetEntityAt(int2 position)
    {
        if (!LocationExists(position)) { return null; }

        if (grid.TryGetValue(position, out Location location))
        {
            return location.entity;
        }

        else return null;
    }
}


public class Location
{
    public int2 position;

    public byte gridId;

    public Entity entity; // 8 bytes

    public int2 GetChunk()
    {
        int2 unflooredChunkPosition = position.x / WorldGenerationData.ChunkSize;
        return new int2(Mathf.FloorToInt(unflooredChunkPosition.x), Mathf.FloorToInt(unflooredChunkPosition.y));
    }
}