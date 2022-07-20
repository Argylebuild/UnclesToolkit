using UnityEngine;

namespace Argyle.Utilities.Geometry
{
	public static class BoundsUtility
	{

		#region ==== Bounds Extensions ====

		/// <summary>
		/// A version of encapsulation that disregards the original bounds if it is effectively null. 
		/// </summary>
		/// <param name="b"></param>
		/// <param name="b2"></param>
		/// <returns></returns>
		public static Bounds SafeEncapsulate(this Bounds b, Bounds b2)
		{
			if (b.size == Vector3.zero && b.center == Vector3.zero)
			{
				return b2;
			}
			b.Encapsulate(b2);
			return b;
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
}