using System;
using UnityEngine;
using DataStructs;
using Unity.Mathematics;

[Serializable]
public class Entity
{
    public byte gridId; // 1 bytes

    public int2 position; // 8 bytes

    private byte2 size = new(1, 1);

    private byte2 centre
    {
        get { return new byte2(size.x / 2, size.y / 2); }
    }

    private sbyte _rotation;
    public sbyte rotation
    {
        get { return _rotation; }
        set { _rotation = (sbyte)((value % 4 + 4) % 4); } // Dont even ask
    }


    //public GameObject gameObject;

    public Entity()
    {
        //centre = position + (size / 2);
    }
}

public class FloorTile : Entity // Size: 9 bytes
{
    public FloorTileData data; //8 bytes

    public FloorTile(FloorTileData tileData)
    {
        this.data = tileData;
    }

    //byte color = 0;
}