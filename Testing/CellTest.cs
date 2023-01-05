using System.Collections.Generic;
using Argyle.UnclesToolkit.Geometry;
using EasyButtons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Argyle.UnclesToolkit.Testing
{
	public class CellTest : ArgyleComponent
	{
		public Cell<GameObject> Cell;
		public List<GameObject> _things = new List<GameObject>();

		[Button]
		public void HighLight()
		{
			foreach (var thing in _things)
				thing.GetComponent<LatticeThingTest>().Highlight();
		}

		public void Scale(float scale) => TForm.localScale = Vector3.one * Cell.CellSize * scale;
	}
}