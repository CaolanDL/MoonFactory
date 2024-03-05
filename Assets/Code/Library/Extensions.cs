using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
/*using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
*/


namespace ExtensionMethods
{
    public static class Int2ToVectorExtensions
    {
        public static Vector3 ToVector3(this int2 i)
        {
            return new Vector3(i.x, 0, i.y);
        }

        public static Vector2 ToVector2(this int2 i)
        {
            return new Vector2(i.x, i.y);
        }

        public static int2 ToInt2(this Vector2 i)
        {
            return new int2((int)i.x, (int)i.y);
        }

        public static int2 ToInt2(this Vector3 i)
        {
            return new int2((int)i.x, (int)i.y);
        }
    }

    public static class SbyteRotationConversions
    {
        public static int2 ToInt2(this sbyte i)
        {
            switch (i)
            {
                case 0:
                    return new int2(0, 1);
                case 1:
                    return new int2(1, 0);
                case 2:
                    return new int2(0, -1);
                case 3:
                    return new int2(-1, 0);
                default:
                    return new int2(0, 0);
            }
        }

        public static Quaternion ToQuaternion(this sbyte i)
        {
            return Quaternion.Euler(0, 90 * i, 0);
        }

        public static sbyte Rotate(this sbyte i, sbyte rotation)
        {
            return (sbyte)(((i + rotation) % 4 + 4) % 4);
        }

        public static sbyte ToSbyte(this Quaternion i)
        {
            switch (Mathf.Round(i.eulerAngles.y / 90) * 90)
            {
                case 0:
                    return 0;
                case 90:
                    return 1;
                case 180:
                    return 2;
                case 270:
                    return 3;
                default:
                    return 0;
            }
        }
    }

    public static class MatrixConstruction
    {
        public static Matrix4x4 CreateTransformMatrix(int2 location, sbyte rotation)
        {
            return Matrix4x4.TRS
            (
                new Vector3(location.x, 0, location.y),
                Quaternion.Euler(0, 90 * rotation, 0),
                Vector3.one
            );
        }
    }
}