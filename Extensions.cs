using System;
using System.Collections.Generic;
using System.Linq;
using API.Utility;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace Argyle.UnclesToolkit
{
	public static class Extensions
	{

		#region Object
		/// <summary>
		/// Extension method to convert any object with its values to CSV. 
		/// NOTE: Only serializes properties, not fields. 
		/// </summary>
		/// <param name="thing">The object to be serialized.</param>
		/// <param name="header">If true, returns a header line for the csv, with the names of the properties. </param>
		/// <returns></returns>
		public static String ToTSV(this object thing, bool header = false)
		{
			string csv = "";

			//header
			if (header)
			{
				csv += thing.ToTsvHeader();
			}
			
			//Values
			csv += thing.ToTsvLine();
			csv += Environment.NewLine;

			return csv;

		}

		/// <summary>
		/// Reads an object's class definition to create headers for a TSV file...
		/// listing objects of that class.
		/// Use in conjunction with ToTSVLine() for full readable data summaries. 
		/// </summary>
		/// <param name="thing"></param>
		/// <returns></returns>
		public static String ToTsvHeader(this object thing)
		{
			string tsv = "";

			foreach (var prop in thing.GetType().GetProperties())
			{
				if (Attribute.IsDefined(prop, typeof(Export)))
					tsv += prop.Name.ToString() + "\t" ; 
			}
			foreach (var field in thing.GetType().GetFields())
			{
				if (Attribute.IsDefined(field, typeof(Export)))
					tsv += field.Name.ToString() + "\t";
			}
			tsv += Environment.NewLine;
			return tsv;
		}

		/// <summary>
		/// Reads an object and serializes it into a single line of a TSV file.
		/// Use in conjunction with ToTSVHeader() for full readable data summaries. 
		/// </summary>
		/// <param name="thing"></param>
		/// <returns></returns>
		public static String ToTsvLine(this object thing)
		{
			string tsv = "";

			//Values
			foreach (var prop in thing.GetType().GetProperties())
			{
				if (Attribute.IsDefined(prop, typeof(Export)))
				{
					tsv += prop.GetValue(thing) == null ?
						"\t" :
						"\"" + prop.GetValue(thing).ToString() + "\"\t";
				}
			}
			foreach (var field in thing.GetType().GetFields())
			{
				if (Attribute.IsDefined(field, typeof(Export)))
				{
					tsv += field.GetValue(thing) == null ? 
						"\t" :
						"\"" +  field.GetValue(thing).ToString() + "\"\t";
				}
			}

			tsv += Environment.NewLine;

			return tsv;
		}

		#endregion


		//float, int
		#region Numbers

		/// <summary>
		/// As the sample size grows, this method adds the new value and properly appends the data
		/// Adjusting the average using the proper scale without needing the entire data set. 
		/// </summary>
		/// <param name="average">The existing average value</param>
		/// <param name="newDataPoint">Add this number to the running averaage</param>
		/// <param name="newSampleSize">How many total datapoints, for balancing input weight</param>
		/// <returns></returns>
		public static float AddToAverage(this float average, float newDataPoint, int newSampleSize)
		{
			//control for unexpected values
			if (newSampleSize < 1)
			{
				//Debug.Log("Sample is too small to calculate an average.");
				return 0;
			}
			else if (newSampleSize == 1)
			{
				return newDataPoint;
			}

			//claculate
			float newSum = average * (newSampleSize - 1) + newDataPoint;
			return newSum / newSampleSize;
		}

		/// <summary>
		/// The float-friendly add to average. E.G. for time-series without perfectly even samples. 
		/// As the sample size grows, this method adds the new value and properly appends the data
		/// Adjusting the average using the proper scale without needing the entire data set.
		/// </summary>
		/// <param name="average">The existing average value</param>
		/// <param name="newDataPoint">Add this number to the running averaage</param>
		/// <param name="oldSampleSize">The sample size before this new addition</param>
		/// <param name="deltaSampleSize">The total sample size after this new addition.</param>
		/// <returns></returns>
		public static float AddToAverage(this float average, 
			float newDataPoint, float oldSampleSize, float deltaSampleSize) => 
			(average * oldSampleSize + newDataPoint * deltaSampleSize) / (oldSampleSize + deltaSampleSize);

		/// <summary>
		/// As the sample size grows, this method adds the new value and properly appends the data
		/// Adjusting the average using the proper scale without needing the entire data set. 
		/// </summary>
		/// <param name="average">The existing average value</param>
		/// <param name="newDataPoint">Add this number to the running averaage</param>
		/// <param name="newSampleSize">How many total datapoints, for balancing input weight</param>
		/// <returns></returns>
		public static int AddToAverage(this int average, int newDataPoint, int newSampleSize)
		{
			float averageFloat = (float) average;
			return (int) Math.Round(averageFloat.AddToAverage((float)newDataPoint, newSampleSize));
		}
		
		/// <summary>
		/// Convert Radian angle to degrees.
		/// </summary>
		/// <param name="rad">Angle in radians</param>
		/// <returns>Angle in degrees</returns>
		public static double RadToDeg(this double rad)
		{
			return rad / Math.PI * 180;
		}
		
		/// <summary>
		/// Convert Radian angle to degrees.
		/// </summary>
		/// <param name="rad">Angle in radians</param>
		/// <returns>Angle in degrees</returns>
		public static float RadToDeg(this float rad) => (float) RadToDeg((double) rad);
		
		
		/// <summary>
		/// Convert degree angle to radians
		/// </summary>
		/// <param name="deg">Angle in degrees</param>
		/// <returns>angle in Radians</returns>
		public static double DegToRad(this double deg)
		{
			return deg / 180 * Math.PI;
		}

		/// <summary>
		/// Convert degree angle to radians
		/// </summary>
		/// <param name="deg">Angle in degrees</param>
		/// <returns>angle in Radians</returns>
		public static float DegToRad(this float deg) => (float) DegToRad((double) deg);
		
		
		/// <summary>
		/// Checks whether a number is within a given percent of another number. Useful replacement for error-prone float equal comparisons.
		/// Target and candidate are effectively interchangeable.  
		/// </summary>
		/// <param name="candidate">The extended float</param>
		/// <param name="target">The float to compare with. </param>
		/// <param name="percent">Closeness tolerance. Default 1 is usually good for replacement == comparison. </param>
		/// <returns></returns>
		public static bool Within(this float candidate, float target, float percent = 1) =>
			math.abs(candidate - target) < percent / 100 * percent;



		public static float Sigmoid(this float input)
		{
			return 1 / (1 + Mathf.Exp(-input));
		}

		/// <summary>
		/// Takes a number within a range and returns its position along an s curve from 0 to 1.
		/// The curve is scaled to match the dataset. 
		/// </summary>
		/// <param name="input">The member being analyzed.</param>
		/// <param name="min">The lowest member of the data set to establish range.</param>
		/// <param name="max">The lowest member of the data set to establish range.</param>
		/// <param name="curve">Multiplier controlling the sharpness of the curve. 1 is standard.</param>
		/// <param name="offset">Horizontal offset of the central curve. Positive numbers move the curve to the right,
		/// reducing each output. </param>
		/// <returns></returns>
		public static float Squash(this float input, float min, float max, float curve = 1, float offset = 0)
		{	//           center on zero...           scale to standard curve... sharpen by curve
			float regularized = (input + offset - (min + max) / 2) * 12 / (max - min) * curve;

			//apply sigmoid
			return regularized.Sigmoid();
		}


		/// <summary>
		/// Takes a collection of numbers and positions along an s curve from 0 to 1.
		/// The curve is scaled to match the range of the dataset. 
		/// </summary>
		/// <param name="inputs">The members being analyzed.</param>
		/// <param name="curve">Multiplier controlling the sharpness of the curve. 1 is standard.</param>
		/// <param name="offset">Horizontal offset of the central curve. Positive numbers move the curve to the right,
		/// reducing each output. </param>
		/// <returns></returns>
		public static List<float> Squash(this ICollection<float> inputs, float curve = 1, float offset = 0)
		{
			if(inputs.Count < 5)
			{
				//Debug.LogWarning($"Trying to squash inputs with less than 5 elements. Returning improperly squashed");
				return inputs.ToList();
			}		
			
			List<float> outputs = new List<float>();
			float min = inputs.Min();
			float max = inputs.Max();
			
			foreach (var input in inputs)
			{
				outputs.Add(input.Squash(min, max, curve, offset));
			}

			return outputs;
		}


		
		#endregion


		#region ==== GameObject ====-----------------

		public static bool IsPrefab(this GameObject go) => go.scene.name == null;

		#endregion ------------------/GameObject ====


		#region ==== Collection ====------------------

		/// <summary>
		/// Safely adds or updates the value of a potential dictionary key.
		/// </summary>
		/// <param name="dic"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns>If a new addition, return true.</returns>
		public static bool AddOrUpdate<TKey, TValue>(this Dictionary<TKey,TValue> dic, TKey key, TValue value)
		{
			if (!dic.ContainsKey(key))
			{
				dic.Add(key, value);
				return true;

			}			
			//if key already present
			dic[key] = value;
			return false;
		}


		#endregion -----------------/Collection ====



		#region ==== Color ====------------------

		public static Vector3 ToVector3(this Color color) => new Vector3(color.r, color.g, color.b);

		public static Color ToColor(this Vector3 vector) => new Color(vector.x, vector.y, vector.z);

		public static Vector4 ToVector4(this Color color) => new Color(color.r, color.g, color.b, color.a);
		
		public static Color ToColor(this Vector4 vector) => new Color(vector.x, vector.y, vector.z, vector.w);
		

		/// <summary>
		/// Link colors are generated by a random color generator seeded by a given string.
		/// Use consistent string input (E.G. name or id) to get consistent color output.
		/// </summary>
		/// <param name="stringSeed"></param>
		/// <returns></returns>
		public static Vector3 RandomColorVector(string stringSeed = null)
		{
			if (stringSeed == null)
				stringSeed = DateTime.Now.ToString();
			
			var r = StringToRandomFloat(stringSeed + "r");
			var g = StringToRandomFloat(stringSeed + "g");
			var b = 1 - (r + g);
			Vector3 c = new Vector3(r, g, b);

			return c;
			
			//---------------local functions----------------
			float StringToRandomFloat(string input)
			{
				var seed = input.GetHashCode();
				Random rand = new Random(input.GetHashCode());
				return (float) rand.Next(0,256) / 256f;
			}
		}

		/// <summary>
		/// Modify a color to have a V (HSV: Hue Saturation Value) of 1. 
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color FullBright(this Color color)
		{
			Color.RGBToHSV(color, out float h, out float s, out float v);
			return Color.HSVToRGB(h, s, 1);
		}
		
		

		public static Color RandomColor(string stringSeed = null) => 
			RandomColorVector(stringSeed).ToColor();

		
		#endregion -----------------/Color ====
		
		
		
		
		
	}

}

