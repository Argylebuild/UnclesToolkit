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

    
    
		public static string MetersToFeetInchesString(this float meters, int dimensions = 1)
		{
			float feetDecimal = meters.MetersToFeet(dimensions);
			int feetInt = Mathf.FloorToInt(feetDecimal);
			float inchesRemainder =(feetDecimal - feetInt) *  Mathf.Pow(12, dimensions);

			if(dimensions == 1)
				return $"{feetInt}'-{decimal.Round((decimal) inchesRemainder, 2)}\"";

			return $"{feetInt} ft<sup>{dimensions}</sup> - \n" +
			       $"{decimal.Round((decimal) inchesRemainder, 2)} in<sup>{dimensions}</sup>)";
		}
	}
}
