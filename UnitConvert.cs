using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public static class UnitConvert
	{
		public static float MetersToFeet(this float meters)
		{
			return meters * 3.28084f;
		}

		public static float FeetToMeters(this float feet)
		{
			return feet / 3.28084f;
		}

    
    
		public static string MetersToFeetInchesString(this float meters)
		{
			float feetDecimal = meters.MetersToFeet();
			int feetInt = Mathf.FloorToInt(feetDecimal);
			decimal inchesRemainder =(decimal) (feetDecimal - feetInt) * 12;

			return $"{feetInt}'-{decimal.Round(inchesRemainder, 2)}\"";
		}
	}
}
