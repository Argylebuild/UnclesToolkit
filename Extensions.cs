using System;
using API.Utility;
using Unity.Mathematics;
using UnityEngine;

namespace Argyle.Utilities
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
				Debug.Log("Sample is too small to calculate an average.");
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

		
		#endregion



		
	}

}

