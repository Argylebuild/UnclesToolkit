using System;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    [Serializable]
    public class Hotspot
    {
        public float Value { get; }
        public float Probability { get; }
        public float Average { get; }
        public float Qty { get; }
        public Vector2Int Position { get; }
        public Vector2Int Dimensions { get; }
        
        public Vector2 PositionNormalized => new Vector2((float) Position.x / Dimensions.x, (float) Position.y / Dimensions.y);
        public Vector2 ScreenPosition => new Vector2(PositionNormalized.x * Screen.width, PositionNormalized.y * Screen.height);

        public Hotspot( Vector2Int coordinates, Vector2Int dimensions, float value, float probability, float avg, int qty = 1)
        {
            Value = value;
            Probability = probability;
            Average = avg;
            Qty = qty;
            Position = coordinates;
            Dimensions = dimensions;
        }
    }
}