using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EasyButtons;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public class CollectionStatsTest : ArgyleComponent
	{
		public int Quantity = 20;
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
			Debug.Log($"Min is {sorted.Min().Key}.");
			Debug.Log($"Q1 is {sorted.Q1().Key}.");
			Debug.Log($"Med is {sorted.Med().Key}.");
			Debug.Log($"Q3 is {sorted.Q3().Key}.");
			Debug.Log($"Max is {sorted.Max().Key}.");
		}

		[Button]
		public void CalculateThings(bool makeNew = true)
		{
			if (makeNew)
			{
				foreach (var thing in things)
					Destroy(thing.gameObject);
				things = new List<Transform>();

				for (int i = 0; i < Quantity; i++)
				{
					var thing = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
					thing.parent = TForm;
					thing.localScale = Vector3.one * .1f;
					float rand = 0;
					for (int j = 0; j < 4; j++)
						rand += Random.value * 5;

					thing.position = new Vector3(rand, 1, 0);
					things.Add(thing);
				}
			}

			SortedList<float, Transform> sortedThings = new SortedList<float, Transform>();
			foreach (var thing in things)
				sortedThings.Add(thing.position.x, thing);

			List<Transform> iqrThings;
			var iqr = sortedThings.Iqr(out iqrThings);
			foreach (var thing in iqrThings)
				thing.GetComponent<MeshRenderer>().material.color = Color.green;
			
			
			foreach (var datum in sortedThings.Outliers())
			{
				datum.Value.localPosition = new Vector3(datum.Key, 1.5f, 0);
				datum.Value.GetComponent<MeshRenderer>().material.color = Color.red;
			}

		}
	}
}