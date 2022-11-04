using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EasyButtons;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public class CollectionStatsTest : ArgyleComponent
	{
		public int Quantity;
		public List<float> numbers;
		public List<float> sortedNumbers;
		
		public List<Transform> things;

		[Button]
		public void CalculateNumbers()
		{
			numbers = new List<float>();
			for (int i = 0; i < Quantity; i++)
				numbers.Add(Random.value * 100);

			SortedList<float, string> sorted = new SortedList<float, string>();
			foreach (var number in numbers)
				sorted.Add(number, number.ToString());

			sortedNumbers = sorted.Keys.ToList();
			string description;
			Debug.Log($"Min is {sorted.Min(out description)} described as {description}");
			Debug.Log($"Q1 is {sorted.Q1(out description)} described as {description}");
			Debug.Log($"Med is {sorted.Med(out description)} described as {description}");
			Debug.Log($"Q3 is {sorted.Q3(out description)} described as {description}");
			Debug.Log($"Max is {sorted.Max(out description)} described as {description}");
		}

		[Button]
		public void CalculateThings(bool makeNew)
		{
			foreach (var thing in things)
				Destroy(thing.gameObject);

			for (int i = 0; i < Quantity; i++)
			{
				var thing = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
				thing.parent = TForm;
				thing.localScale = Vector3.one * .1f;
				thing.position = new Vector3(Random.value * 20, 1, 0);
				things.Add(thing);
			}

			SortedList<float, Transform> sortedThings = new SortedList<float, Transform>();
			foreach (var thing in things)
				sortedThings.Add(thing.position.x, thing);

			foreach (var thing in sortedThings.Outliers())
			{
				//thing.position = new Vector3()
			}

		}
	}
}