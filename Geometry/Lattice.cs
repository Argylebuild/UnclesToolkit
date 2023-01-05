using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
	/// <summary>
	/// A spatial collection strategy where all elements in collection are referenced by one or more cells,
	/// laid out in a square grid. Only cells with data are referenced in memory.
	/// Collection items may be referenced by more than one cell, but are only listed once per cell.
	/// (Example use: large objects elements spanning several cells, can be found geometrically at O(1) without repeating geometric calculations.)
	///
	/// Note: This assumed to be used in tandem with a local coordinate system.
	/// Can be helpful in replacing Unity's assumption that bounds are always relative to "global" coordinates.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Lattice<T> where T : class
	{
		
		/// <summary>
		/// All cells stored within this Lattice.
		/// Note empty cells are not actually stored in memory, but only implied by the grid structure. 
		/// </summary>
		public Dictionary<Vector3Int, Cell<T>> Cells { get; } = new Dictionary<Vector3Int, Cell<T>>();

		/// <summary>
		/// A coordinate representing the minimum value in all dimensions of the lattice grid.
		/// Used to express the extents of the Lattice. Like bounds. 
		/// </summary>
		public Vector3Int Min { get; private set; }
		/// <summary>
		/// A coordinate representing the maximum value in all dimensions of the lattice grid.
		/// Used to express the extents of the Lattice. Like bounds. 
		/// </summary>
		public Vector3Int Max { get; private set; }
		
		#region ==== Settings ====------------------

		/// <summary>
		/// Size in meters of each cell. Must be set in constructor and cannot be changed. 
		/// </summary>
		public float CellSize { get; private set; }


		#endregion -----------------/Settings ====


		#region ==== CTOR ====------------------
		
		/// <summary>
		/// Creates a new lattice with no content, but all settings established.
		/// Ready to start adding content. 
		/// </summary>
		/// <param name="cellSize"></param>
		public Lattice(float cellSize = 1)
		{
			CellSize = cellSize;
		}

		#endregion -----------------/CTOR ====



		#region ==== CRUD ====------------------

		#region == Add ==----

		/// <summary>
		/// Add a single object to a given cell. Used for a single cell when the cellposition is already known. 
		/// </summary>
		/// <param name="cellPosition"></param>
		/// <param name="thing"></param>
		public void Add(Vector3Int cellPosition, T thing)
		{
			if (!Cells.ContainsKey(cellPosition))
					NewCell(cellPosition);

			Cells[cellPosition].Add(thing);
		}

		/// <summary>
		/// Add a single object so a single cell, based on a relative point in space.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="thing"></param>
		public void Add(Vector3 point, T thing) =>
			Add(PointToCellPosition(point), thing);
		

		/// <summary>
		/// Add a single object to all relevant cells based on the object's relative extents.
		/// Each cell within extents box has a duplicate reference to the object in question. 
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <param name="thing"></param>
		public void AddByMinMax(Vector3 min, Vector3 max, T thing)
		{
			Vector3Int minCell = PointToCellPosition(min);
			Vector3Int maxCell = PointToCellPosition(max);
			Vector3Int range = maxCell - minCell;

			for (int i = 0; i <= range.x; i++)
				for (int j = 0; j <= range.y; j++)
					for (int k = 0; k <= range.z; k++)
							Add(new Vector3Int(minCell.x + i,minCell.y + j,minCell.z + k), thing);
		}

		/// <summary>
		/// When a cell is needed but not yet initialized.
		/// </summary>
		/// <param name="cellPosition"></param>
		private void NewCell(Vector3Int cellPosition)
		{
			Cells.Add(cellPosition, new Cell<T>(this, cellPosition));
			UpdateMinMax(cellPosition);
		}


		#endregion ----/Add ==


		#region == Get ==----

		public void GetAtPoint(Vector3 point) => GetAtCellPosition(PointToCellPosition(point));
		

		public List<T> GetAtCellPosition(Vector3Int cellPosition) => 
			Cells.ContainsKey(cellPosition) ? Cells[cellPosition].Things.ToList() : new List<T>();

		#endregion ----/Get ==

		#endregion -----------------/CRUD ====


		#region ==== Geometry ====------------------

		/// <summary>
		/// Everytime a new cell is added 
		/// </summary>
		/// <param name="cellPosition"></param>
		private void UpdateMinMax(Vector3Int cellPosition)
		{
			if (Cells.Count == 1)
			{
				Min = cellPosition;
				Max = cellPosition;
				return;
			}

			Min = new Vector3Int(
				IntUtility.MinBetween(cellPosition.x, Min.x),
				IntUtility.MinBetween(cellPosition.y, Min.y),
				IntUtility.MinBetween(cellPosition.z, Min.z)
			);
			Max = new Vector3Int(
				IntUtility.MaxBetween(cellPosition.x, Max.x),
				IntUtility.MaxBetween(cellPosition.y, Max.y),
				IntUtility.MaxBetween(cellPosition.z, Max.z)
			);

		}

		
		/// <summary>
		/// Find the closest cell contains content.
		/// Replaces checking distance to every object in set and returns best answer, rather than threshold based.
		/// For equidistant results, returns first found.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public Cell<T> ClosestCell(Vector3 point)
		{
			var closest = ClosestCellPosition(point);
			if (Cells.ContainsKey(closest))
				return Cells[closest];
			else
				return null;


		}

		/// <summary>
		/// Find the closest cell position that contains content.
		/// Replaces checking distance to every object in set and returns best answer, rather than threshold based.
		/// For equidistant results, returns first found.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private Vector3Int ClosestCellPosition(Vector3 point)
		{
			//quit early if empty
			if (Cells.Count <= 0)
				return Vector3Int.zero;
			
			//cheaply make sure it's within the extents of the set.
			Vector3Int coord = PointToCellPosition(point);
			coord.Clamp(Min, Max);
			
			//if in cell ditch out early
			if (Cells.ContainsKey(coord))
				return coord;

			//try progressively more distant cells to find content.
			for (int i = 1; i < (Max - Min).magnitude; i++)
			{
				var cells = Surrounding(coord, i);
				if (cells.Count > 0)
					return cells[0].CellPosition;
			}

			//Unable to find. Default value
			return Vector3Int.zero;
		}

		/// <summary>
		/// Translate a float based point in space to its encapsulating cell based on the given cell size. 
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private Vector3Int PointToCellPosition(Vector3 point) => new Vector3Int(
				Mathf.RoundToInt(point.x / CellSize),
				Mathf.RoundToInt(point.y / CellSize),
				Mathf.RoundToInt(point.z / CellSize));

		/// <summary>
		/// Reversing the translation to get the precise location of the center of a given cell prosition. 
		/// </summary>
		/// <param name="cellPosition"></param>
		/// <returns></returns>
		public Vector3 CellPositionToPoint(Vector3Int cellPosition) => new Vector3(
			cellPosition.x * CellSize,
			cellPosition.y * CellSize,
			cellPosition.z * CellSize
		);

		/// <summary>
		/// Collects the list of all cells surrounding a given cellposition that contain content.
		/// Includes the center and all cells between center and edges. 
		/// </summary>
		/// <param name="center"></param>
		/// <param name="distance">How many cells away from the center to include.</param>
		/// <returns></returns>
		public List<Cell<T>> Surrounding(Vector3Int center, int distance)
		{
			List<Cell<T>> surrounding = new List<Cell<T>>();
			foreach (var pos in SurroundingPositions(center, distance))
				if(Cells.ContainsKey(pos))
					surrounding.Add(Cells[pos]);

			return surrounding;
		}

		/// <summary>
		/// A list of all cell positions surrounding the center to a given distance.
		/// These are just the coordinates, inclusive. There may not be a cell in any or all of the resulting positions.
		/// Includes the center and all cells between center and edges. 
		/// </summary>
		/// <param name="center"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		private List<Vector3Int> SurroundingPositions(Vector3Int center, int distance)
		{
			List<Vector3Int> surrounding = new List<Vector3Int>();

			for (int i = -distance; i < distance; i++)
				for (int j = -distance; j < distance; j++)
					for (int k = -distance; k < distance; k++)
						surrounding.Add(new Vector3Int(center.x + i, center.y + j, center.z + k));

			return surrounding;
		}

		#endregion -----------------/Geometry ====

	}
}