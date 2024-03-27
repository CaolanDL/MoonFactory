using System; 
using Unity.Mathematics;
using UnityEngine; 

/// <summary>
/// An int2 position and a sbyte rotation representing 90 degree turns as 0-3. 9 bytes.
/// </summary>
[Serializable]
public struct TinyTransform
{
    [SerializeField] public int2 position; // 8 bytes 

    [SerializeField] private sbyte _rotation; // 1 byte
    public sbyte rotation
    {
        get { return _rotation; }
        set { _rotation = (sbyte)((value % 4 + 4) % 4); }
    }

    public static TinyTransform empty = new TinyTransform();

    public TinyTransform(int2 position)
    {
        this.position = position; 
        _rotation = 0; 
    }

    public TinyTransform(int2 position, sbyte rotation)  
    {
        this.position = position;

        _rotation = rotation;
        this.rotation = _rotation;
    }  

    /// <summary> Only adds the position, takes the rotation from lhs </summary> 
    public static TinyTransform operator + (TinyTransform lhs, TinyTransform rhs) => new TinyTransform(new int2(lhs.position.x + rhs.position.x, lhs.position.y + rhs.position.y), lhs.rotation);
    /// <summary> Only subtracts the position, takes the rotation from lhs </summary> 
    public static TinyTransform operator - (TinyTransform lhs, TinyTransform rhs) => new TinyTransform(new int2(lhs.position.x - rhs.position.x, lhs.position.y - rhs.position.y), lhs.rotation);

    public bool Equals(TinyTransform other)
    { 
        return position.Equals(other.position) && rotation == other.rotation;
    }

    public Matrix4x4 ToMatrix()
    {
        return Matrix4x4.TRS
        (
            new Vector3(position.x, 0, position.y),
            Quaternion.Euler(0, 90 * rotation, 0),
            Vector3.one
        );
    }
}

/// <summary>
/// Stores a float2 for position, and a float for rotation between 0-359
/// </summary>
public struct SmallTransform
{
    [SerializeField] public float2 position; // 8 bytes 

    [SerializeField] private float _rotation; // 4 bytes
    public float rotation
    {
        get { return _rotation; }
        set { _rotation = ((value+180) % 360 + 360) % 360 - 180; }
    }

    public static SmallTransform empty = new SmallTransform();

    public SmallTransform(float2 position)
    {
        this.position = position;
        _rotation = 0;
    }

    public SmallTransform(float2 position, short rotation)
    {
        this.position = position;

        _rotation = rotation;
        this.rotation = _rotation;
    }

    public static explicit operator int2(SmallTransform i)
    {
        return new int2(Mathf.RoundToInt(i.position.x), Mathf.RoundToInt(i.position.y));
    }
}

namespace DataStructs
{
    [Serializable]
    public struct byte2 // 2 bytes
    {
        public byte x;
        public byte y;

        public byte2(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public byte2(int x, int y) : this()
        {
            this.x = (byte)x;
            this.y = (byte)y;
        }

        public bool Equals(byte2 other)
        {
            return (x == other.x && y == other.y);
        }

        public static byte2 operator +(byte2 a, byte2 b) => new byte2(a.x + b.x, a.y + b.y);

        public static byte2 operator -(byte2 a, byte2 b) => new byte2(a.x - b.x, a.y - b.y);
         
    }
}

