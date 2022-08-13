using UnityEngine;

namespace Argyle.Utilities.Geometry
{
	public static class BoundsUtility
	{

		#region ==== Bounds Extensions ====

		/// <summary>
		/// A version of encapsulation that disregards the original bounds if it is effectively null.
		/// Also disregards the added bounds if it is default. 
		/// </summary>
		/// <param name="b1"></param>
		/// <param name="b2"></param>
		/// <returns></returns>
		public static Bounds SafeEncapsulate(this Bounds b1, Bounds b2)
		{
			if (b1.size == Vector3.zero && b1.center == Vector3.zero)
				return b2;

			if (b2.size == Vector3.zero && b2.center == Vector3.zero)
				return b1;
			
			b1.Encapsulate(b2);
			return b1;
		}

		public static Bounds SafeEncapsulate(this Bounds b1, BoundsNullable b2)
		{
			if (b2 != null)
				b1.SafeEncapsulate(b2.b);
			return b1;
		}

		public static BoundsNullable SafeEncapsulate(this BoundsNullable b1, Bounds b2)
		{
			if(b1 != null)
			{
				b1.b.SafeEncapsulate(b2);
				return b1;
			}
				
			return new BoundsNullable(b2);
		}

		public static BoundsNullable SafeEncapsulate(this BoundsNullable b1, BoundsNullable b2)
		{
			if (b2 != null)
				b1.SafeEncapsulate(b2);
			return b1;
		}


		/// <summary>
		/// A version of encapsulation that disregards the original bounds if it is effectively null.
		/// Note: Does not modify the original. Bounds struct must be set equal to return value.
		/// </summary>
		/// <param name="b"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static Bounds SafeEncapsulate(this Bounds b, Vector3 point)
		{
			if (b.size == Vector3.zero && b.center == Vector3.zero)
			{
				b.center = point;
				return b;
			}
			b.Encapsulate(point);
			return b;
		}

		public static Bounds AverageEncapsulate(this Bounds b1, Bounds b2, int sampleSize)
		{
			Bounds original = b1;
			Bounds total = b1.SafeEncapsulate(b2);

			b1.center = new Vector3(
				original.center.x.AddToAverage(total.center.x, sampleSize),
				original.center.y.AddToAverage(total.center.y, sampleSize),
				original.center.z.AddToAverage(total.center.z, sampleSize)
			);

			b1.size = new Vector3(
				original.size.x.AddToAverage(total.size.x, sampleSize),
				original.size.y.AddToAverage(total.size.y, sampleSize),
				original.size.z.AddToAverage(total.size.z, sampleSize)
			);

			return b1;
		}
		

		public static BoundsNullable SafeEncapsulate(this BoundsNullable b, Vector3 point)
		{
			b.b.SafeEncapsulate(point);
			return b;
		}

		/// <summary>
		/// create invisible object to help find relative positions
		/// </summary>
		/// <param name="bounds">The pre-calculated bounds to visualize</param>
		/// <param name="parent">Parent object in hierarchy.
		/// This should be the object whose bounds are being visualized.
		/// Will be used for rotation visualization.</param>
		/// <param name="name">What to call the object in the scene</param>
		/// <returns>Game object representing bounds </returns>
		public static GameObject ToPrimitive(this Bounds bounds, Transform parent, string name = "bounds")
		{
			var	boundsGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
					
			boundsGo.name = name;
			boundsGo.transform.localScale = bounds.size;
			boundsGo.transform.parent = parent;
			boundsGo.transform.position = bounds.center;
			boundsGo.GetComponent<MeshRenderer>().enabled = false;
			boundsGo.GetComponent<Collider>().enabled = false;
			
			return boundsGo;
		}

		/// <summary>
		/// Easy way to test if bounds have been calculated already to prevent redundant work. 
		/// </summary>
		/// <param name="bounds"></param>
		/// <returns>Returns true if paramters match a black bounds</returns>
		public static bool IsDefault(this Bounds bounds)
		{
			return bounds.center == Vector3.zero && 
			       bounds.size == Vector3.zero;
		}


		public static BoundsNullable ToNullable(this Bounds b) => new BoundsNullable(b);
		
		#endregion /Bounds Extensions ====

		
		

		#region ==== Bounds Debugging ====
		
		/// <summary>
		/// Displays the bounds as a box primitive in its size location and orientation.
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="parent"></param>
		public static Transform ShowBounds(Bounds bounds, Transform parent)
		{
			Transform boundsObjectTForm = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
			boundsObjectTForm.parent = parent;
			boundsObjectTForm.position = bounds.center;
			boundsObjectTForm.localRotation = Quaternion.identity;
			boundsObjectTForm.localScale = bounds.size;

			return boundsObjectTForm;
		}

		

		#endregion /Bounds Debugging ====
		
		
	}

	/// <summary>
	/// Safer for situations where bounds may not exist and should not be included in calculations.
	/// WARNING: boxing structs can cause performance problems at scale.
	/// TODO: fill out documentation for properties and methods.
	/// </summary>
	public class BoundsNullable
	{
		public Bounds b;

		#region ==== Properties ====-------------------------------------

		public Vector3 center => b.center;
		public Vector3 extents => b.extents;
		public Vector3 max => b.max;
		public Vector3 min => b.min;
		public Vector3 size => b.size;
		

		#endregion -------------------------------/Properties ====


		#region ==== CTOR ====---------------------------------

		public BoundsNullable(Vector3 center, Vector3 size)
		{
			b = new Bounds(center, size);
		}

		public BoundsNullable(Bounds bounds)
		{
			b = bounds;
		}

		#endregion ----------------------/CTOR ====


		#region ==== Methods ====--------------------------------

		public Vector3 ClosestPoint(Vector3 point) => b.ClosestPoint(point);

		public bool Contains(Vector3 point) => b.Contains(point);

		public void Encapsulate(Vector3 point) => b.Encapsulate(point);

		public void Expand(float amount) => b.Expand(amount);

		public bool IntersectRay(Ray ray) => b.IntersectRay(ray);

		public bool Intersects(Bounds bounds) => b.Intersects(bounds);

		public bool Intersects(BoundsNullable boundsNullable) => b.Intersects(boundsNullable.b);

		public void SetMinMax(Vector3 min, Vector3 max) => b.SetMinMax(min, max);

		public float SqrDistance(Vector3 point) => b.SqrDistance(point);

		public string ToString() => b.ToString();


		#endregion -------------------/Methods ====


		#region ==== Extension Methods ====-----------------------


		#endregion --------------------------/Extension Methods ====


	}
	
}