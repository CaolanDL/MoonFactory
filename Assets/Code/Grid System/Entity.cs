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

    public byte2 size = new(1, 1); // 2 bytes

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

    public (int2 xRange, int2 yRange) GetOccupyRegion()
    {
        var rSize = new int2(size.x, size.y);

        rSize = RotateGridRegionAroundOrigin(rSize, rotation);

        var xRange = new int2(position.x, position.x + rSize.x);
        var yRange = new int2(position.y, position.y + rSize.y);

        if(xRange.x > xRange.y)
        {
            var temp = xRange.x; 
            xRange.x = xRange.y;
            xRange.y = temp;
        }

        if (yRange.x > yRange.y)
        {
            var temp = yRange.x;
            yRange.x = yRange.y;
            yRange.y = temp;
        }

        //Debug.Log(xRange);
        //Debug.Log(yRange);

        return (xRange, yRange);
    }

    public int2 RotateGridRegionAroundOrigin(int2 region, sbyte _rotation)
    {
        var _region = region.Rotate(_rotation);

/*        if (rotation == 1)
        {
            _region.y += 1;
        }
        if (rotation == 2)
        {
            _region.y += 1;
            _region.x += 1;
        }
        if (rotation == 3)
        {
            _region.x += 1;
        }*/

        return _region;
    }

    public List<Location> GetOccupyingLocations()
    {
        var list = new List<Location>();    
        var (xRange, yRange) = GetOccupyRegion();

        for(var x = xRange.x; x < xRange.y; x++)
        {
            for (var y = yRange.x; y < yRange.y; y++)
            {
                //Debug.Log($"{x}, {y}");
                list.Add(GameManager.Instance.GameWorld.worldGrid.GetLocationAt(new int2(x, y)));
            }
        }

        return list;
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