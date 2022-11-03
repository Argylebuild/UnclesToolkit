using System.Collections.Generic;

namespace Argyle.UnclesToolkit
{
	public class SortedSet<T> where T : class
	{
		private List<T> Values = new List<T>();
		private List<float> sortValues = new List<float>();

		public void Add(T value, float sortValue)
		{
			if (Values.Contains(value))
			{
				sortValues.RemoveAt(Values.IndexOf(value));
				Values.Remove(value);
			}
			SafeAdd(value, sortValue);
		}

		private void SafeAdd(T value, float sortValue)
		{
			for (int i = sortValues.Count; i >= 0; --i)
			{
				//if(sortValue < sortValues[i])
					
			}
		}
	}
}