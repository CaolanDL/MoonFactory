using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/*using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
*/

namespace ExtensionMethods
{
    public static class Int2Extensions
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

        public static int2 Rotate(this int2 i, sbyte rotation)
        {
            float radian = -rotation * 90 * Mathf.Deg2Rad;
            int _x = Mathf.RoundToInt(i.x * Mathf.Cos(radian) - i.y * Mathf.Sin(radian));
            int _y = Mathf.RoundToInt(i.x * Mathf.Sin(radian) + i.y * Mathf.Cos(radian));
            return new int2(_x, _y);
        }
    }

    public static class VectorExtensions
    {
        public static float2 ToFloat2(this Vector3 i)
        {
            return new float2(i.x, i.z);
        }

        public static float2 ToFloat2(this Vector2 i)
        {
            return new float2(i.x, i.y);
        }

        public static Vector3 ToWorldspaceV3(this Vector2 i)
        {
            return new Vector3(i.x, 0, i.y);
        }


        public static Vector3 RotateAround(this Vector3 i, Vector3 Pivot, Quaternion quaternion)
        {
            return quaternion * (i - Pivot) + Pivot;
        }

        // https://sushanta1991.blogspot.com/2016/08/how-to-rotate-2d-vector-in-unity.html
        public static Vector2 Rotate(this Vector2 i, float angle)
        {
            float radian = -angle * Mathf.Deg2Rad;
            float _x = i.x * Mathf.Cos(radian) - i.y * Mathf.Sin(radian);
            float _y = i.x * Mathf.Sin(radian) + i.y * Mathf.Cos(radian);
            return new Vector2(_x, _y);
        }

        public static Vector2 FastRotate(this Vector2 i, float radians)
        {   
            return new Vector2(xRotate(), yRotate());

            float xRotate()
            {
                return  i.x * Mathf.Cos(radians) - i.y * Mathf.Sin(radians);
            }

            float yRotate()
            {
                return i.x * Mathf.Sin(radians) + i.y * Mathf.Cos(radians);
            }

            float _x = i.x * Mathf.Cos(radians) - i.y * Mathf.Sin(radians);
            float _y = i.x * Mathf.Sin(radians) + i.y * Mathf.Cos(radians);
        }

        public static Vector2 RotateAround(this Vector2 i, Vector2 Pivot, float angle)
        {
            return (i - Pivot).Rotate(angle) + Pivot;
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

        static Quaternion byteQuaternionZero = Quaternion.Euler(0, 0, 0);
        static Quaternion byteQuaternionOne = Quaternion.Euler(0, 90, 0);
        static Quaternion byteQuaternionTwo = Quaternion.Euler(0, 180, 0);
        static Quaternion byteQuaternionThree = Quaternion.Euler(0, 270, 0); 

        public static Quaternion ToQuaternion(this sbyte i)
        {
            return i switch
            {
                0 => byteQuaternionZero,
                1 => byteQuaternionOne,
                2 => byteQuaternionTwo,
                3 => byteQuaternionThree,
                _ => throw new NotImplementedException(),
            };
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

    public static class WithinRangeExtensions
    {
        public static bool WithinRange(this int i, int2 range)
        {
            return (i > range.x && i < range.y);
        }

        public static bool WithinRange(this float i, int2 range)
        {
            return (i > range.x && i < range.y);
        }
    }

    public static class QuaternionExtensions
    {
        public static Quaternion MinusFortyFive = Quaternion.Euler(0, -45, 0);
    }

    public static class Float2Extensions
    {
        public static float DistanceBetween(float2 origin, float2 target)
        {
            return Mathf.Sqrt((origin.x * origin.x) + (target.y * target.y));
        }
    }
}