using System;
using System.Collections.Generic; 
using DataStructs;
using ExtensionMethods;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class Entity // 12 bytes
{
    public TinyTransform transform; // 9 bytes 
    public byte gridId; // 1 byte
    public byte2 size = new(1, 1); // 2 bytes
    public virtual bool highlightable { get => true; }

    public int2 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public sbyte rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }

    public byte2 centre
    {
        get { return new byte2(size.x / 2, size.y / 2); }
    }

    public void RemoveEntity()
    {
        Grid.GetGrid(gridId).RemoveEntity(position);
    }

    public Entity GetNeighbor(sbyte rotationFactor)
    {
        return GameManager.Instance.GameWorld.worldGrid.GetEntityAt(position + rotation.Rotate(rotationFactor).ToInt2());
    }

    static byte2 byte2_one = new(1, 1);
    public Location[] GetNeighbors()
    {
        var grid = Grid.GetGrid(gridId);

        if (size.Equals(byte2_one)) { return grid.GetLocationAt(position).GetNeighbors(); } 

        (int2 xRange, int2 yRange) = GetOccupyingRegion(this);
        List<Location> neighbors = new();

        for(int y = yRange.x; y <= yRange.y; y++)
        {
            neighbors.Add(grid.GetLocationAt(new(xRange.x - 1, y)));
            neighbors.Add(grid.GetLocationAt(new(xRange.y + 1, y)));
        }
        for (int x = xRange.x; x <= xRange.y; x++)
        {
            neighbors.Add(grid.GetLocationAt(new(yRange.x - 1, x)));
            neighbors.Add(grid.GetLocationAt(new(yRange.y + 1, x)));
        }

        return neighbors.ToArray();
    }


    public static (int2 xRange, int2 yRange) GetOccupyingRegion(Entity entity)
    {
        return GetOccupyingRegion(entity.position, entity.size.ToInt2(), entity.rotation);
    }
    public static (int2 xRange, int2 yRange) GetOccupyingRegion(int2 position, int2 size, sbyte rotation)
    {
        var trueSize = new int2(size.x - 1, size.y - 1).Rotate(rotation);

        var xRange = new int2(position.x, position.x + trueSize.x);
        var yRange = new int2(position.y, position.y + trueSize.y);

        if (xRange.x > xRange.y) (xRange.x, xRange.y) = (xRange.y, xRange.x);
        if (yRange.x > yRange.y) (yRange.x, yRange.y) = (yRange.y, yRange.x);

        return (xRange, yRange);
    }
     

    public static List<Location> GetOccupyingLocations(Entity entity)
    {
        return GetOccupyingLocations(entity.position, entity.size.ToInt2(), entity.rotation, Grid.GetGrid(entity.gridId));
    }
    public static List<Location> GetOccupyingLocations(int2 position, int2 size, sbyte rotation, Grid grid)
    {
        var list = new List<Location>();
        var (xRange, yRange) = GetOccupyingRegion(position, size, rotation);

        for (var x = xRange.x; x <= xRange.y; x++) 
            for (var y = yRange.x; y <= yRange.y; y++) 
                list.Add(grid.GetOrAddLocation(new int2(x, y))); 

        return list;
    }


    public virtual void RenderSelectionOutline()
    {
        RenderSelectionOutline(RenderData.Instance.SelectionGizmoMaterial);
    }

    public virtual void RenderSelectionOutline(Material material)
    {
        if (!highlightable) return;
        float2 floatCentre = new((float)size.x / 2f -0.5f, size.y / 2f - 0.5f);
        var trueFloatCentre = position + floatCentre.Rotate(rotation);
        var trueVecCentre = new Vector3(trueFloatCentre.x, 0, trueFloatCentre.y);
        float sineTime = (Mathf.Sin(Time.time * 2f) + 1) / 2; // Sine 0 - 1 
        var p = trueVecCentre + (Vector3.up * (sineTime / 8 + 0.01f));
        var r = Quaternion.Euler(0, Time.time * 120f, 0);
        float scaler = size.x > size.y ? size.x : size.y;
        scaler = size.x == size.y ? ((size.x + size.y) / 2f) : scaler;
        var s = Vector3.one * scaler;
        var matrix = Matrix4x4.TRS(p, r, s);
        Graphics.DrawMesh(RenderData.Instance.SelectionGizmo, matrix, material, 0);

        matrix = Matrix4x4.TRS(position.ToVector3(), Quaternion.identity, Vector3.one);
        Graphics.DrawMesh(RenderData.Instance.TilesGizmo, matrix, RenderData.Instance.TransparentBlueGizmoMaterial, 0);

        //Debug.Log("Rendered Selection Outline");
    }
}

public class FloorTile : Entity // Size: 9 bytes
{
    public FloorTileData data; //8 bytes

    public FloorTile(FloorTileData tileData)
    {
        this.data = tileData;
    }
}