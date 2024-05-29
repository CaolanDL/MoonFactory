using DataStructs;
using Terrain;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random; 

public class GameWorld
{
    public List<int2> ExploredChunks = new();
    public Grid floorGrid = new Grid();
    public Grid worldGrid = new Grid();
    public Dictionary<int2, Meteorite> meteorites = new();
    public Dictionary<int2, Crater> craters = new();

    public static Action WorldInstanciated;

    public GameWorld(int seed)
    {
        //TerrainGenerator = new TerrainGenerator(seed);
        WorldInstanciated?.Invoke();
    }

    public void OnUpdate()
    {
        foreach (var kvp in meteorites)
        {
            kvp.Value.TryUpdate();
        }
    }

    public void OnFixedUpdate()
    {
        TryGenerateNewChunks();
    }

    /*    public void GenerateStartZone()
        {
            int size = TerrainGenerationData.ChunkSize;

            int halfSize = size / 2;

            for (int x = -halfSize; x < halfSize + 1; x++)
            {
                for (int y = -halfSize; y < halfSize + 1; y++)
                {
                    GenerateFloorTile(new int2(x, y));
                }
            }
        }*/

    public void TryGenerateNewChunks()
    {
        //(var xVisibleRange, var yVisibleRange) = GameManager.Instance.CameraController.GetIsometricVisibleRange();

        var chunk = InferChunkFromPosition(GameManager.Instance.CameraController.CameraGridPosition);

        var genDist = TerrainGenerationData.ChunkGenerationDistance;
        for (int cx = chunk.x - genDist; cx < chunk.x + genDist; cx++)
            for (int cy = chunk.y - genDist; cy < chunk.y + genDist; cy++)
            {
                var cPos = new int2(cx, cy);
                if (ExploredChunks.Contains(cPos) == false)
                {
                    GenerateChunk(cPos);
                }
            }
    }

    void GenerateChunk(int2 chunk)
    {
        (int2 xRange, int2 yRange) = InferRegionFromChunk(chunk);
        GenerateRegion(xRange, yRange);
        if (ExploredChunks.Contains(chunk) == false) ExploredChunks.Add(chunk);
        //Debug.Log("Chunk Generated");
    }

    public int2 InferChunkFromPosition(int2 position)
    {
        float2 unflooredChunkPosition = ((float2)position) / TerrainGenerationData.ChunkSize;
        return new int2(Mathf.FloorToInt(unflooredChunkPosition.x), Mathf.FloorToInt(unflooredChunkPosition.y));
    }
    public (int2 xRange, int2 yRange) InferRegionFromChunk(int2 chunk)
    {
        var chunkOrigin = chunk * TerrainGenerationData.ChunkSize;
        int2 xRange = new(chunkOrigin.x, chunkOrigin.x + TerrainGenerationData.ChunkSize);
        int2 yRange = new(chunkOrigin.y, chunkOrigin.y + TerrainGenerationData.ChunkSize);
        return (xRange, yRange);
    }

    /// <summary>
    /// Generates a world region given an start position and an end position
    /// </summary>
    public void GenerateFromStartToEnd(int2 startPosition, int2 endPosition)
    {
        //Modify start and end positions to point in positive iteration direction
        if (startPosition.x > endPosition.x) { (startPosition.x, endPosition.x) = (endPosition.x, startPosition.x); }
        if (startPosition.y > endPosition.y) { (startPosition.y, endPosition.y) = (endPosition.y, startPosition.y); }

        var xRange = new int2(startPosition.x, endPosition.x);
        var yRange = new int2(startPosition.y, endPosition.y);
        GenerateRegion(xRange, yRange);
    }

    /// <summary>
    /// Generates a world region given an x range and y range.
    /// This is the main world generation method.
    /// </summary> 
    /// 
    ///Generation Sequence:
    ///1. Generate new Locations in visible region + (32 on each side)
    ///   Store list of all new Locations
    ///   Store list of new Locations outside of visible range
    ///2. Dice roll on each Location outside of visible range for spawning a crater
    ///   If crater would intersect with any existing entity do not spawn the crater
    ///3. Dice roll on each new location for spawning a meteorite
    ///   If meteorite is ontop of crater or intersects with an existing entity do not spawn the meteorite
    ///4. Fill all remaining floor locations with displacement tiles 

    static List<Location> locationsToFill = new();
    static List<int2> coordsToGenerate = new();

    //? The reason this doesn't work is that it needs to be generated in chunks, not as each new tile is seen
    //? Store a dictionary of rendered chunks
    //? Replace Generate Visible Region with TryGenerateNewChunks

    public void GenerateRegion(int2 xRange, int2 yRange)
    {
/*        ///Expand Generation Region by 32
        int e = 64;
        xRange.x -= e; xRange.y += e;
        yRange.x -= e; yRange.y += e;*/

        ///Generate new Locations in expanded generation region and store them in a List<>
        locationsToFill.Clear();
        coordsToGenerate.Clear();
        for (int x = xRange.x; x < xRange.y; x++)
        {
            for (int y = yRange.x; y < yRange.y; y++)
            {
                var coord = new int2(x, y);
                if (floorGrid.LocationExists(coord) == false)
                {
                    locationsToFill.Add(floorGrid.GetOrAddLocation(coord));
                    coordsToGenerate.Add(coord);
                    worldGrid.GetOrAddLocation(coord);
                }
            }
        }

        /// If there is nothing that can be generated then early exit
        if (coordsToGenerate.Count == 0) { return; }

        /// Generate Craters
        float craterChance = TerrainGenerationData.Instance.chanceToSpawnCrater;
        var craterDataPool = TerrainGenerationData.Instance.Craters;
        foreach (var location in locationsToFill)
        {
            if (Mathf.Abs(location.position.x) + Mathf.Abs(location.position.y) < TerrainGenerationData.StartZoneSize) { continue; }
            if (coordsToGenerate.Contains(location.position) == false) { continue; }
            if (Random.value < craterChance) /// Dice roll on each new Location for spawning a crater
            {
                var craterData = craterDataPool[Random.Range(0, craterDataPool.Count)];
                var craterRegion = Entity.GetOccupyingRegion(location.position, new(craterData.size, craterData.size), 0);
                bool spawn = true; 

                bool @break = false;
                ForInRange(craterRegion.xRange, craterRegion.yRange, ref @break, (pos) =>
                {
                    if (worldGrid.GetEntityAt(pos) != null)
                    {
                        spawn = false;
                        @break = true;
                    }
                });
                 
                if (spawn)
                {
                    var newCrater = new Terrain.Crater(craterData);
                    floorGrid.TryAddEntity(newCrater, location.position, 0);
                    this.craters.Add(location.position, newCrater);
                    @break = false;
                    ForInRange(craterRegion.xRange, craterRegion.yRange, ref @break, (pos) =>
                    {
                        coordsToGenerate.Remove(pos);
                    });
                }
            }
        };

        /// Fill all remaining floor locations with displacement tiles  
        foreach (var coord in coordsToGenerate) GameManager.Instance.GameWorld.GenerateFloorTile(coord);

        /// Dice roll on each location for spawning a meteorite
        foreach (var coord in coordsToGenerate)
        {
            if (Mathf.Abs(coord.x) + Mathf.Abs(coord.y) < TerrainGenerationData.StartZoneSize) { continue; }
            TryGenerateMeteorite(coord);
        }

        void ForInRange(int2 xRange, int2 yRange, ref bool @break, Action<int2> action)
        {
            for (int x = xRange.x; x < xRange.y; x++)
                for (int y = yRange.x; y < yRange.y; y++)
                {
                    action(new(x, y));
                    if (@break) { break; }
                }
        }

        /*        for (int x = xRange.x; x < xRange.y; x++)
                    for (int y = yRange.x; y < yRange.y; y++)
                    {
                        var tileLocation = new int2(x, y);
                        if (GameManager.Instance.GameWorld.floorGrid.grid.ContainsKey(tileLocation) == false)
                            GameManager.Instance.GameWorld.GenerateFloorTile(tileLocation);
                    }*/
    }


    public FloorTile GenerateFloorTile(int2 position)
    {
        FloorTile newFloorTile = new(TerrainGenerationData.Instance.displaceTile);

        worldGrid.GetOrAddLocation(position);
        floorGrid.GetOrAddLocation(position);

        floorGrid.TryAddEntity(newFloorTile, position, (sbyte)Random.Range(0, 3));

        return newFloorTile;
    }

    public void TryGenerateMeteorite(int2 position)
    {
        var terrainData = TerrainGenerationData.Instance;

        if (Mathf.Abs(position.x) + Mathf.Abs(position.y) < TerrainGenerationData.StartZoneSize) { return; }

        if (Random.value < terrainData.chanceToSpawnMeteorite)
        {
            if (floorGrid.GetEntityAt(position) is not FloorTile) { return; }

            var randomModel = terrainData.MeteorModels[Random.Range(0, terrainData.MeteorModels.Length)];
            var meteorite = new Meteorite(randomModel, Random.Range(terrainData.minMeteoriteScale, terrainData.maxMeteoriteScale));
            meteorite.position = position;
            worldGrid.TryAddEntity(meteorite, position, (sbyte)Random.Range(0, 3));
            meteorites.Add(position, meteorite);
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

    public Location GetOrAddLocation(int2 position)
    {
        if (LocationExists(position)) return grid[position];

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
        if (grid.ContainsKey(position)) return true;
        else return false;
    }

    public Location GetLocationAt(int2 position)
    {
        if (grid.TryGetValue(position, out Location location)) return location;
        else return null;
    }

    /// <summary>
    /// ! Avoid calling this. Use TryAddEntity() instead.
    /// </summary> 
    public Entity AddEntity(Entity entity, int2 position) => TryAddEntity(entity, position, 0);


    static byte2 singleTileSize = new byte2(1, 1);

    public Entity TryAddEntity(Entity entity, int2 position, sbyte rotation)
    {
        entity.position = position;
        entity.rotation = rotation;
        entity.gridId = id;

        if (entity.size.Equals(singleTileSize))
        {
            Location location = GetOrAddLocation(position);
            if (IsEntityAt(position)) return null;
            location.entity = entity;
        }
        else
        {
            var occupyLocations = Entity.GetOccupyingLocations(entity);

            foreach (var location in occupyLocations)
                if (location.entity != null) return null;

            GetLocationAt(position).entity = entity;
            foreach (var location in occupyLocations)
                location.entity = entity;
        }

        return entity;
    }

    public Entity RemoveEntity(int2 position)
    {
        if (grid.ContainsKey(position) != true) return null;

        return grid[position].RemoveEntity();
    }

    public Entity GetEntityAt(int2 position)
    {
        if (grid.TryGetValue(position, out Location location)) return location.entity;

        else return null;
    }

    public bool IsEntityAt(int2 position)
    {
        if (grid.TryGetValue(position, out Location location))
            if (location.entity != null) return true;

        return false;
    }

    public bool IsEntityInArea(int2 position, int2 size)
    {
        return IsEntityInArea(position, size, 0);
    }

    public bool IsEntityInArea(int2 position, int2 size, sbyte rotation)
    {
        for (int x = position.x; x < position.x + size.x; x++)
            for (int y = position.y; y < position.y + size.y; y++)
                if (grid.TryGetValue(new int2(x, y), out Location location))
                    if (location.entity != null)
                        return true;
        return false;
    }

    public bool IsEntityInRegion(int2 xRange, int2 yRange)
    {
        int xSign = (int)Mathf.Sign(xRange.x - xRange.y);
        int ySign = (int)Mathf.Sign(yRange.x - yRange.y);

        for (int x = xRange.x; x != xRange.y; x += xSign)
            for (int y = yRange.x; y != yRange.y; y += ySign)
                if (grid.TryGetValue(new int2(x, y), out Location location))
                    if (location.entity != null)
                        return true;
        return false;
    }

    public List<Location> GetSquareRadius(int2 origin, int radius) { return GetSquareRadius(origin, radius, radius); }
    public List<Location> GetSquareRadius(int2 origin, int xRadius, int yRadius)
    {
        int2 xRange = new int2(origin.x - xRadius, origin.x + xRadius);
        int2 yRange = new int2(origin.y - yRadius, origin.y + yRadius);

        List<Location> region = new();

        int2 pos = new();

        for (pos.x = xRange.x; pos.x <= xRange.y; pos.x += 1)
            for (pos.y = yRange.x; pos.y <= yRange.y; pos.y += 1)
                if (grid.TryGetValue(pos, out Location location))
                    region.Add(location);
        return region;
    }



    //Todo GetRegion is just broken and I dont really know why. Try to figure this out please.
    /*    public List<Location> GetRegion(int2 origin, int xSize, int ySize)
        {
            int2 xRange = new int2( origin.x, origin.x + xSize );
            int2 yRange = new int2(origin.y, origin.y + ySize);

            return GetRegion(xRange, yRange);
        }

        public List<Location> GetRegion(int2 xRange, int2 yRange)
        {
            List<Location> region = new();

            int xSign = (int)Mathf.Sign(xRange.x - xRange.y);
            int ySign = (int)Mathf.Sign(yRange.x - yRange.y);

            for (int x = xRange.x; x != xRange.y; x += xSign)
                for (int y = yRange.x; y != yRange.y; y += ySign)
                    if (grid.TryGetValue(new int2(x, y), out Location location))
                        region.Add(location);
            return region;
        }*/
}

/// <summary>
/// A Location in the World Grid
/// </summary>
public class Location // Size: 17 bytes
{
    public int2 position; // 8 bytes

    public byte gridId; // 1 bytes

    public Entity entity; // 8 bytes  

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
        if (entity == null) { return null; }
        var entityToRemove = entity;
        Grid.GetGrid(gridId).entities.Remove(entity);
        entity = null;

        foreach (var location in Entity.GetOccupyingLocations(entityToRemove))
        {
            location.entity = null;
        }

        return entityToRemove;
    }
}