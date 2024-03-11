using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine; 

using Random = UnityEngine.Random;

public class GameWorld
{
    public Grid floorGrid = new Grid();

    public Grid worldGrid = new Grid();

    List<Entity> entities = new List<Entity>();


    private TerrainGenerator terrainGenerator;


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
        if (startPosition.x > endPosition.x) { (startPosition.x, endPosition.x) = (endPosition.x, startPosition.x); }
        if (startPosition.y > endPosition.y) { (startPosition.y, endPosition.y) = (endPosition.y, startPosition.y); } 

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
        FloorTileData newTileData = terrainGenerator.ChooseTileAt(position);

        FloorTile newFloorTile = new(newTileData);

        floorGrid.AddEntity(newFloorTile, position, (sbyte)Random.Range(0, 3)); 

        return newFloorTile;
    } 

    ////
    ///   Sequencing

    public void GenerateStartZone()
    {
        int size = WorldGenerationData.StartZoneSize;

        int halfSize = size / 2;

        for (int xChunk = -halfSize; xChunk < halfSize + 1; xChunk++)
        {
            for (int yChunk = -halfSize; yChunk < halfSize + 1; yChunk++)
            {
                GenerateChunk(new int2(xChunk, yChunk));
            }
        }
    }
}


public class Grid
{
    private static List<Grid> grids = new List<Grid>();
    public Dictionary<int2, Location> grid = new Dictionary<int2, Location>();

    public List<Entity> entities = new List<Entity>();

    public byte id;

    public Grid()
    {
        grids.Add(this);
        id = (byte)(grids[grids.Count - 1].id + 1);
    }

    public static Grid GetGrid(byte id)
    {
        return grids.Find(x => x.id == id);
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
        else return Location.empty;
    }

    public Entity AddEntity(Entity entity, int2 position)
    {
        return AddEntity(entity, position, 0);
    }

    public Entity AddEntity(Entity entity, int2 position, sbyte rotation)
    {
        Location location = AddLocation(position);

        location.entity = entity;

        entity.position = position;

        entity.rotation = rotation;

        return entity;
    }
     
    public Entity RemoveEntity(int2 position)
    {
        if (grid.ContainsKey(position) != true) { return null; }

        return grid[position].RemoveEntity();
    }

    public Entity GetEntityAt(int2 position)
    {
        if (grid.TryGetValue(position, out Location location))
        {
            return location.entity;
        }

        else return null;
    }

    public bool IsEntityAt(int2 position)
    {
        if (grid.TryGetValue(position, out Location location))
        {
            if (location.entity != null)
            {
                return true;
            }
        }

        return false;
    }
}


public class Location // Size: 13 bytes
{
    public int2 position; // 4 bytes

    public byte gridId; // 1 bytes

    public Entity entity; // 8 bytes 

    public static Location empty = new Location();

    public int2 GetChunk()
    {
        int2 unflooredChunkPosition = position.x / WorldGenerationData.ChunkSize;
        return new int2(Mathf.FloorToInt(unflooredChunkPosition.x), Mathf.FloorToInt(unflooredChunkPosition.y));
    }

    public Entity RemoveEntity()
    {
        var entityToRemove = entity;
        Grid.GetGrid(gridId).entities.Remove(entity);
        entity = null;
        return entityToRemove;
    }
}