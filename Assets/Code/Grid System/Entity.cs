﻿using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class Entity
{
    public byte gridId; // 1 bytes

    public int2 position; // 8 bytes

    /*
    private int2 size;

    private int2 centre;

    private int _rotation;
    private int rotation
    {
        get { return _rotation; }
        set { _rotation = value % 4; }
    }
    */

    //public GameObject gameObject;

    public Entity()
    {
        //centre = position + (size / 2);
    }
}


public class FloorTile : Entity // Size: 9 bytes
{
    public FloorTileData tileData; //8 bytes

    public FloorTile(FloorTileData tileData)
    {
        this.tileData = tileData;
    }

    //byte color = 0;
}