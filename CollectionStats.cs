using System;
using System.Collections.Generic;

namespace Argyle.UnclesToolkit
{
	public static class CollectionStats
	{
		/// <summary>
		/// Lowest datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="thing">The object associated with the given key</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static float Min<T>(this SortedList<float, T> data, out T thing)
		{
			if (data.Count > 0)
			{
				float key = data.Keys[0];
				thing = data[key];
				return key;
			}	
			else
				throw new IndexOutOfRangeException();
		}
		/// <summary>
		/// Lowest datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">Prebuilt sorted list with the sorting parameter as floats. </param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Min<T>(this SortedList<float, T> data)
		{
			T temp;
			return data.Min<T>(out temp);
		}

		/// <summary>
		/// Datapoint dividing the 1st quartile from the 2nd in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="thing">The object associated with the given key</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static float Q1<T>(this SortedList<float, T> data, out T thing)
		{
			if (data.Count > 5)
			{
				float key = data.Keys[(data.Count - 1) / 4];
				thing = data[key];
				return key;
			}	
			else if (data.Count > 0)
			{
				float key = data.Keys[0];
				thing = data[key];
				return key;
			}
			else
				throw new IndexOutOfRangeException();
		}
		
		/// <summary>
		/// Datapoint dividing the 1st quartile from the 2nd in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">Prebuilt sorted list with the sorting parameter as floats. </param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Q1<T>(this SortedList<float, T> data)
		{
			T temp;
			return data.Q1<T>(out temp);
		}

		/// <summary>
		/// Middle datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="thing">The object associated with the given key</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static float Med<T>(this SortedList<float, T> data, out T thing)
		{
			if (data.Count > 3)
			{
				float key = data.Keys[(data.Count - 1) / 2];
				thing = data[key];
				return key;
			}	
			else if (data.Count > 0)
			{
				float key = data.Keys[0];
				thing = data[key];
				return key;
			}
			else
				throw new IndexOutOfRangeException();
		}
		/// <summary>
		/// Middle datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">Prebuilt sorted list with the sorting parameter as floats. </param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Med<T>(this SortedList<float, T> data)
		{
			T temp;
			return data.Med<T>(out temp);
		}

		/// <summary>
		/// Datapoint dividing the 3rd quartile from the 4th in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="thing">The object associated with the given key</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static float Q3<T>(this SortedList<float, T> data, out T thing)
		{
			if (data.Count > 5)
			{
				float key = data.Keys[(data.Count - 1) * 3 / 4];
				thing = data[key];
				return key;
			}	
			else if (data.Count > 0)
			{
				float key = data.Keys[0];
				thing = data[key];
				return key;
			}
			else
				throw new IndexOutOfRangeException();
		}
		/// <summary>
		/// Datapoint dividing the 3rd quartile from the 4th in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">Prebuilt sorted list with the sorting parameter as floats. </param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Q3<T>(this SortedList<float, T> data)
		{
			T temp;
			return data.Q3<T>(out temp);
		}

		/// <summary>
		/// Highest datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="thing">The object associated with the given key</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static float Max<T>(this SortedList<float, T> data, out T thing)
		{
			if (data.Count > 0)
			{
				float key = data.Keys[data.Count - 1];
				thing = data[key];
				return key;
			}	
			else
				throw new IndexOutOfRangeException();
		}
		/// <summary>
		/// Highest datum in the dataset according to the sorted float keys. 
		/// </summary>
		/// <param name="data">Prebuilt sorted list with the sorting parameter as floats. </param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Max<T>(this SortedList<float, T> data)
		{
			T temp;
			return data.Max<T>(out temp);
		}


		/// <summary>
		/// Inner Quartile Range. The difference between the float keys of the 1st and 3rd quartiles.
		/// Used to calculate outliers etc. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="things">The objects associated with the given keys</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static float Iqr<T>(this SortedList<float, T> data, out List<T> things)
		{
			if (data.Count > 5)
			{
				float q1 = data.Q1();
				float q3 = data.Q3();
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
				throw new IndexOutOfRangeException();

		}
		/// <summary>
		/// Inner Quartile Range. The difference between the float keys of the 1st and 3rd quartiles.
		/// Used to calculate outliers etc. 
		/// </summary>
		/// <param name="data">Prebuilt sorted list with the sorting parameter as floats. </param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static float Iqr<T>(this SortedList<float, T> data)
		{
			if (data.Count > 5)
				return data.Q3() - data.Q1();
			else
				throw new IndexOutOfRangeException();

		}

		/// <summary>
		/// Calculates and returns a list of outliers from a give dataset, as organized by the sorted set keys. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="outlierThings">The objects associated with the given outlier key</param>
		/// <param name="threshold"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="IndexOutOfRangeException"></exception>
		public static List<float> Outliers<T>(this SortedList<float, T> data, out List<T> outlierThings, float threshold = 1.5f)
		{
			if (data.Count > 5)
			{
				float iqr = data.Q3() - data.Q1();
				float med = data.Med();


				List<float> outlierKeys = new List<float>();
				outlierThings = new List<T>();
				foreach (var datum in data)
				{
					if (datum.Key > med + iqr * threshold)
					{
						outlierKeys.Add(datum.Key);
						outlierThings.Add(datum.Value);
					}
				}

				return outlierKeys;
			}
			else
				throw new IndexOutOfRangeException();
		}

		public static List<float> Outliers<T>(this SortedList<float, T> data, float threshold = 1.5f)
		{
			List<T> temp = new List<T>();
			return data.Outliers<T>(out temp);

		}

		
	}
}