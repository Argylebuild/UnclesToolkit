using System.Collections.Generic;
using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
	public class Cell<T> where T : class
	{
		public HashSet<T> Things { get; } = new HashSet<T>();

		public Lattice<T> LatticeParent { get; private set; }
		
		public Vector3Int CellPosition { get; private set; }
		
		#region ==== Settings ====------------------

		public float CellSize => LatticeParent.CellSize;

		#endregion -----------------/Settings ====

	
	
	
		#region ==== CTOR ====------------------

		public Cell(Lattice<T> latticeParent, Vector3Int cellPosition)
		{
			LatticeParent = latticeParent;
			CellPosition = cellPosition;
		}

		#endregion -----------------/CTOR ====



		#region ==== CRUD ====------------------

		public void Add(T thing)
		{
			Things.Add(thing);
		}

		public void Clear()
		{
			Things.Clear();
		}

		#endregion -----------------/CRUD ====
		
	}
}