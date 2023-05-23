using System;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public static class UnitConvert
	{
		public static float MetersToFeet(this float meters, float exponent = 1)
		{
			return meters * Mathf.Pow(3.28084f,exponent);
		}

		public static float FeetToMeters(this float feet, float exponent = 1)
		{
			return feet / Mathf.Pow(3.28084f, exponent);
		}

		public static float MetersToInches(this float meters, float exponent = 1)
		{
			return meters * Mathf.Pow(39.3701f, exponent);
		}
		
		public static float InchesToMeters(this float inches, float exponent = 1)
		{
			return inches / Mathf.Pow(39.3701f, exponent);
		}
		
		
		/// <summary>
		/// Convert meters float to formatted string of feet and inches
		/// </summary>
		/// <param name="meters"></param>
		/// <param name="dimensions"></param>
		/// <param name="decimals">Sig Figs</param>
		/// <param name="useDecimalFeet"></param>
		/// <returns></returns>
		public static string MetersToFeetInchesString(this float meters, int dimensions = 1, int decimals = 2, bool useDecimalFeet = false)
		{
			float feetDecimal = meters.MetersToFeet(dimensions);
			int feetInt = Mathf.FloorToInt(feetDecimal);
			float inchesRemainder =(feetDecimal - feetInt) *  Mathf.Pow(12, dimensions);

			if (useDecimalFeet)
			{
				if(dimensions == 1)
					return $"{Math.Round(feetDecimal, decimals)}'";

				return $"{Math.Round(feetDecimal, decimals)} ft<sup>{dimensions}</sup>";
			}
			else
			{
				if(dimensions == 1)
					return $"{feetInt}'-{Math.Round(inchesRemainder, decimals)}\"";

				return $"{feetInt} ft<sup>{dimensions}</sup> - \n" +
				       $"{Math.Round(inchesRemainder, 2)} in<sup>{dimensions}</sup>)";
			}
		}
		
		/// <summary>
		/// Convert meters float to formatted string of inches
		/// </summary>
		/// <param name="meters"></param>
		/// <param name="dimensions"></param>
		/// <param name="decimals"></param>
		/// <returns></returns>
		public static string MetersToInchesString(this float meters, int dimensions = 1, int decimals = 2)
		{
			float inches = meters.MetersToInches(dimensions);

			if(dimensions == 1)
				return $"{Math.Round(inches, decimals)}\"";

			return $"{Math.Round(inches, 2)} in<sup>{dimensions}</sup>)";
		}

	}
}
