using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;

namespace Argyle.UnclesToolkit
{
	public static class CollectionStats
	{
		private const int minQty = 5;
		private const string outOfRangeMessage = "The collection must have at least objects to be properly analyzed.";
		private const string noDataMessage = "Collection must contain data to be analyzed.";
		
		/// <summary>
		/// Lowest datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static KeyValuePair<float, T> Min<T>(this SortedList<float, T> data)
		{
			if (data.Count > 0)
			{
				float key = data.Keys[0];
				return new KeyValuePair<float, T>(key, data[key]);
			}	
			else
				throw new IndexOutOfRangeException(noDataMessage);
		}

		/// <summary>
		/// Datapoint dividing the 1st quartile from the 2nd in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static KeyValuePair<float, T> Q1<T>(this SortedList<float, T> data)
		{
			if (data.Count >= minQty)
			{
				float key = data.Keys[(data.Count - 1) / 4];
				return new KeyValuePair<float, T>(key, data[key]);
			}	
			else
				throw new IndexOutOfRangeException(outOfRangeMessage);
		}
		
		/// <summary>
		/// Middle datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <returns></returns>
		public static KeyValuePair<float, T> Med<T>(this SortedList<float, T> data)
		{
			float key;
			if (data.Count >= (minQty + 1) / 2)
				key = data.Keys[(data.Count - 1) / 2];
			else if (data.Count > 0)
				key = data.Keys[0];
			else
				throw new IndexOutOfRangeException();
			
			return new KeyValuePair<float, T>(key, data[key]);

		}

		/// <summary>
		/// Datapoint dividing the 3rd quartile from the 4th in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <returns></returns>
		public static KeyValuePair<float, T> Q3<T>(this SortedList<float, T> data)
		{
			float key;
			if (data.Count >= minQty)
				key = data.Keys[(data.Count - 1) * 3 / 4];
			else
				throw new IndexOutOfRangeException(outOfRangeMessage);
			return new KeyValuePair<float, T>(key, data[key]);
		}

		/// <summary>
		/// Highest datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <returns></returns>
		public static KeyValuePair<float, T> Max<T>(this SortedList<float, T> data)
		{
			if (data.Count > 0)
			{
				float key = data.Keys[data.Count - 1];
				return new KeyValuePair<float, T>(key, data[key]);
			}	
			else
				throw new IndexOutOfRangeException(noDataMessage);
		}


		/// <summary>
		/// Inner Quartile Range. The difference between the float keys of the 1st and 3rd quartiles.
		/// Used to calculate outliers etc. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <param name="things">The objects associated with the given keys</param>
		/// <returns></returns>
		public static float Iqr<T>(this SortedList<float, T> data, out List<T> things)
		{
			if (data.Count >= minQty)
			{
				float q1 = data.Q1().Key;
				float q3 = data.Q3().Key;
				float iqr = q3 - q1;

				things = new List<T>();
				foreach (var datum in data)
				{
					if(datum.Key >= q1 && datum.Key <= q3)
						things.Add(datum.Value);
				}
				
				return iqr;
			}
			else
				throw new IndexOutOfRangeException(outOfRangeMessage);

		}
		/// <summary>
		/// Inner Quartile Range. The difference between the float keys of the 1st and 3rd quartiles.
		/// Used to calculate outliers etc. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <returns></returns>
		public static float Iqr<T>(this SortedList<float, T> data)
		{
			if (data.Count >= minQty)
				return data.Q3().Key - data.Q1().Key;
			else
				throw new IndexOutOfRangeException(outOfRangeMessage);

		}

		/// <summary>
		/// Calculates and returns a list of outliers from a give dataset, as organized by the sorted set keys. 
		/// </summary>
		/// <param name="data">A collection of objects, sorted by some measurement. To be analyzed according to that measurement. </param>
		/// <param name="multiplier">A multiplier for the average range, beyond which a datum counts as an outlier. </param>
		/// <returns></returns>
		public static SortedList<float, T> Outliers<T>(this SortedList<float, T> data, float multiplier = 1.5f)
		{
			if (data.Count >= minQty)
			{
				float iqr = data.Q3().Key - data.Q1().Key;
				float med = data.Med().Key;

				SortedList<float, T> outliers = new SortedList<float, T>();
				foreach (var datum in data)
					if (datum.Key > med + iqr * multiplier || datum.Key < med - iqr * multiplier)
						outliers.Add(datum.Key, datum.Value);
				
				return outliers;
			}
			else
				throw new IndexOutOfRangeException(outOfRangeMessage);
		}
	}
}