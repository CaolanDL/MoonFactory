using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class Entity
{
    public byte gridId;

    public string debugName;

    private int2 position;
    private int2 size;

    private int2 centre;

    private int _rotation;
    private int rotation
    {
        get { return _rotation; }
        set { _rotation = value % 4; }
    }

    public GameObject gameObject;

    public Entity()
    {
        centre = position + (size / 2);
    }
}