using System;
using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
    /// <summary>
    /// Since unity doesn't flag the Vector2 as serializable, we
    /// need to create our own version. This one will automatically convert
    /// between Vector2 and SerializableVector2
    /// </summary>
    [Serializable]
    public struct SerializableVector2
    {
        /// <summary>
        /// x component
        /// </summary>
        public float x;

        /// <summary>
        /// y component
        /// </summary>
        public float y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        public SerializableVector2(float rX, float rY)
        {
            x = rX;
            y = rY;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}]", x, y);
        }

        /// <summary>
        /// Automatic conversion from SerializableVector2 to Vector2
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector2(SerializableVector2 rValue)
        {
            return new Vector2(rValue.x, rValue.y);
        }

        /// <summary>
        /// Automatic conversion from Vector2 to SerializableVector2
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator SerializableVector2(Vector2 rValue)
        {
            return new SerializableVector2(rValue.x, rValue.y);
        }

        public static SerializableVector2 Parse(string v2string)
        {
            try
            {
                var v2strings = v2string
                    .TrimStart('(')
                    .TrimEnd(')')
                    .Split(',');

                return new SerializableVector2(
                    float.Parse(v2strings[0]),
                    float.Parse(v2strings[1]));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.LogError(string.Format(
                    "Failed to parse {0} into vector2",
                    v2string));
            }
            return new SerializableVector2();
        }
        
        public static SerializableVector2 Zero => new SerializableVector2(0, 0);
        
        public static SerializableVector2 One => new SerializableVector2(1, 1);
        
        public Vector2 ToVector2() => new Vector2(x, y);
    }
    
    public static class Vector2Extensions
    {
        public static SerializableVector2 ToSerializableVector2(this Vector2 v2) => new SerializableVector2(v2.x, v2.y);
        public static SerializableVector2 ToSerializableVector2(this Vector3 v3) => new SerializableVector2(v3.x, v3.y);
    }
}
