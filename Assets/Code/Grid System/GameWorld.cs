using DataStructs;
using ExtensionMethods;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using Random = UnityEngine.Random;

public class GameWorld
{
    public Grid floorGrid = new Grid();

    public Grid worldGrid = new Grid();

    private TerrainGenerator terrainGenerator;

    public static Action WorldInstanciated;

    public GameWorld(int seed)
    {
        terrainGenerator = new TerrainGenerator(seed);
        WorldInstanciated?.Invoke();
    }

    public void OnFixedUpdate()
    {
        AddNewVisibleTiles();
    }

    public void AddNewVisibleTiles()
    {
        int2 tileLocation;

        (var xVisibleRange, var yVisibleRange) = GameManager.Instance.cameraController.GetDiamondVisibleRange();

        for (int x = xVisibleRange.x; x < xVisibleRange.y; x++)
        {
            for (int y = yVisibleRange.x; y < yVisibleRange.y; y++)
            {
                (tileLocation.x, tileLocation.y) = (x, y);

                // Generate new floor tiles
                if (GameManager.Instance.gameWorld.floorGrid.grid.ContainsKey(tileLocation) == false)
                {
                    GameManager.Instance.gameWorld.GenerateFloorTile(tileLocation);
                }
            }
        }
    }

    public void GenerateChunk(int2 position)
    {
        int2 chunkWorldPosition = position * TerrainGenerationData.ChunkSize;

        int2 chunkEndPositon = chunkWorldPosition + TerrainGenerationData.ChunkSize;

        GenerateRegion(chunkWorldPosition, chunkEndPositon);
    }

    public int2 GetChunkAt(int2 position)
    {
        int2 unflooredChunkPosition = position / TerrainGenerationData.ChunkSize;
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
        FloorTileData newTileData = terrainGenerator.GenerateTileAt(position);

        FloorTile newFloorTile = new(newTileData);

        if (floorGrid.TryAddEntity(newFloorTile, position, (sbyte)Random.Range(0, 3)) != null)
        {
            worldGrid.AddLocation(position);
        }

        return newFloorTile;
    }

    ////
    ///   Sequencing

    public void GenerateStartZone()
    {
        int size = TerrainGenerationData.StartZoneSize;

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

    public byte id = 0;

    public Grid()
    {
        grids.Add(this);
        id = (byte)(grids.Count > 1 ? grids[grids.Count - 1].id + 1 : 0);
    }

    public static Grid GetGrid(byte _id)
    {
        return grids.Find(grid => grid.id == _id);
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
        if (grid.TryGetValue(position, out Location location))
        {
            return location;
        }
        else return null;
    }

    public Entity AddEntity(Entity entity, int2 position)
    {
        return TryAddEntity(entity, position, 0);
    }

    static byte2 singleTileSize = new byte2(1, 1);

    public Entity TryAddEntity(Entity entity, int2 position, sbyte rotation)
    {
        if (true || entity.size.Equals(singleTileSize))
        {
            Location location = AddLocation(position);

            if (IsEntityAt(position)) return null;

            location.entity = entity;
        }

/*        else
        {
            for (int x = 0; x < entity.size.x; x++)
            {
                for (int y = 0; y < entity.size.y; y++)
                {
                    var offsetPosition = position + (new int2(x, y).Rotate(rotation));

                    Location location = AddLocation(offsetPosition);

                    if (IsEntityAt(offsetPosition)) return null;

                    location.entity = entity;
                }
            }
        }*/

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

    public bool IsEntityInArea(int2 position, int2 size)
    {
        return IsEntityInArea(position, size, 0);
    }

    public bool IsEntityInArea(int2 position, int2 size, sbyte rotation)
    {
        for (int x = position.x; x < position.x + size.x; x++)
        {
            for (int y = position.y; y < position.y + size.y; y++)
            {
                if (grid.TryGetValue(new int2(x, y), out Location location))
                {
                    if (location.entity != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsEntityInRegion(int2 xRange, int2 yRange)
    {
        int xSign = (int)Mathf.Sign(xRange.x - xRange.y);
        int ySign = (int)Mathf.Sign(yRange.x - yRange.y);

        for (int x = xRange.x; x != xRange.y; x += xSign)
        {
            for (int y = yRange.x; y != yRange.y; y += ySign)
            {
                if (grid.TryGetValue(new int2(x, y), out Location location))
                {
                    if (location.entity != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}


public class Location // Size: 17 bytes
{
    public int2 position; // 8 bytes

    public byte gridId; // 1 bytes

    public Entity entity; // 8 bytes 

    //public static Location empty = new Location() { position = new int2(int.MaxValue, int.MaxValue) };

    public int2 GetChunk()
    {
        int2 unflooredChunkPosition = position.x / TerrainGenerationData.ChunkSize;
        return new int2(Mathf.FloorToInt(unflooredChunkPosition.x), Mathf.FloorToInt(unflooredChunkPosition.y));
    }

    static class Directions
    {
        public static int2 forward = new(0, 1);
        public static int2 back = new(0, -1);
        public static int2 left = new(-1, 0);
        public static int2 right = new(1, 0);
    }

    public Location[] GetNeighbors()
    {
        Location[] neighbors = new Location[4];

        Grid grid = Grid.GetGrid(gridId);

        neighbors[0] = grid.GetLocationAt(position + Directions.forward);
        neighbors[1] = grid.GetLocationAt(position + Directions.right);
        neighbors[2] = grid.GetLocationAt(position + Directions.back);
        neighbors[3] = grid.GetLocationAt(position + Directions.left);

        return neighbors;
    }

    public Entity RemoveEntity()
    {
        var entityToRemove = entity;
        Grid.GetGrid(gridId).entities.Remove(entity);
        entity = null;
        return entityToRemove;
    }
}