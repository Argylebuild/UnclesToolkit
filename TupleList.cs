using System;
using System.Collections.Generic;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// For sorting where sort values may contain duplicates.
	/// Partly copied from https://stackoverflow.com/questions/5716423/c-sharp-sortable-collection-which-allows-duplicate-keys
	/// by user https://stackoverflow.com/users/3469332/user450
	/// </summary>
	/// <typeparam name="T1"></typeparam>
	/// <typeparam name="T2"></typeparam>
	public class TupleList<T1, T2> : List<Tuple<T1, T2>> where T1 : IComparable
	{
		public void Add(T1 item, T2 item2)
		{
			Add(new Tuple<T1, T2>(item, item2));
		}

		public new void Sort()
		{
			Comparison<Tuple<T1, T2>> c = (a, b) => a.Item1.CompareTo(b.Item1);
			base.Sort(c);
		}

		public List<T2> Values
		{
			get
			{
				List<T2> values = new List<T2>();
				foreach (var tuple in this)
				{
					values.Add(tuple.Item2);
				}

				return values;
			}
		}
	}
}