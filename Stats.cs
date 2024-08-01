using Unity.Barracuda;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    public static class Stats
    {
        public static float MaxValue(float[] values)
        {
            float max = float.MinValue;
            foreach (var value in values)
            {
                if (value > max)
                    max = value;
            }

            return max;
        }
        
        
        /// <summary>
        /// Calculate the maximum value in a 2D array and return the value and the x and y indices of the maximum value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Hotspot MaxCoord2D(float[,] values, int ignoreBorder = 0)
        {
            int width = values.GetLength(1);
            int height = values.GetLength(0);

            // Step 1: Calculate the maximum value (for numerical stability) and sum of exponentials
            float maxVal = float.MinValue;
            for (int y = ignoreBorder; y < height - ignoreBorder; y++)
            {
                for (int x = ignoreBorder; x < width - ignoreBorder; x++)
                {
                    if (values[y, x] > maxVal)
                    {
                        maxVal = values[y, x];
                    }
                }
            }

            float sumExp = 0;
            float[,] softmaxValues = new float[height, width];
            for (int y = ignoreBorder; y < height - ignoreBorder; y++)
            {
                for (int x = ignoreBorder; x < width - ignoreBorder; x++)
                {
                    softmaxValues[y, x] = Mathf.Exp(values[y, x] - maxVal); // Softmax step
                    sumExp += softmaxValues[y, x];
                }
            }

            // Step 2: Normalize to get probabilities and find the maximum probability
            float maxProbability = float.MinValue;
            int maxX = 0;
            int maxY = 0;
            float sum = 0;
            int qty = 0;

            for (int y = ignoreBorder; y < height - ignoreBorder; y++)
            {
                for (int x = ignoreBorder; x < width - ignoreBorder; x++)
                {
                    softmaxValues[y, x] /= sumExp; // Normalize to get the probability
                    sum += softmaxValues[y, x];
                    qty++;
                    if (softmaxValues[y, x] > maxProbability)
                    {
                        maxProbability = softmaxValues[y, x];
                        maxX = x;
                        maxY = y;
                    }
                }
            }

            float avgProbability = sum / qty;
            return new Hotspot(maxX, maxY, values[maxY, maxX], maxProbability, avgProbability, qty);
        }


        
        public static float MinValue(float[] values)
        {
            float min = float.MaxValue;
            foreach (var value in values)
            {
                if (value < min)
                    min = value;
            }

            return min;
        }
        
        public static (float, int, int) Min2D(float[,] values)
        {
            float min = float.MaxValue;
            int minX = 0;
            int minY = 0;
            for (int y = 0; y < values.GetLength(0); y++)
            {
                for (int x = 0; x < values.GetLength(1); x++)
                {
                    if (values[y, x] < min)
                    {
                        min = values[y, x];
                        minX = x;
                        minY = y;
                    }
                }
            }

            return (min, minX, minY);
        }
        
        public static float Mean(float[] values)
        {
            float sum = 0;
            foreach (var value in values)
            {
                sum += value;
            }

            return sum / values.Length;
        }


        public static float[,] To2DArray(Tensor tensor, int channel = 0)
        {
            var shape = tensor.shape;
            float[,] values = new float[shape.height, shape.width];
    
            for (int i = 0; i < shape.height; i++)
            {
                for (int j = 0; j < shape.width; j++)
                {
                    values[i, j] = tensor[0, i, j, channel];
                }
            }

            return values;
        }
        
    }

    public class Hotspot
    {
        public float Value { get; }
        public float Probability { get; }
        public float Average { get; }
        public float Qty { get; }
        public Vector2Int Position { get; }

        public Hotspot( int x, int y, float value, float probability, float avg, int qty = 1)
        {
            Value = value;
            Probability = probability;
            Average = avg;
            Qty = qty;
            Position = new Vector2Int(x, y);
        }
    }
}