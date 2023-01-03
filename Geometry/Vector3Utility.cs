using System;
using System.Collections.Generic;
using Argyle.Utilities;
using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
	public static class Vector3Utility
	{
		#region Extensions

		/// <summary>
		/// A label for vector components.
		/// </summary>
		public enum Component { x, y, z }

		/// <summary>
		/// Gets the angle between a normal vector and one axis (component) as projected on a 2d plane containing the the component dimension and one reference dimension. 
		/// WARNING: MIGHT NOT WORK. ¯\_(ツ)_/¯
		/// </summary>
		/// <param name="v3">The vector3 source.</param>
		/// <param name="comp">Dimension for which you want component angle. </param>
		/// <param name="refe">Reference dimension to build 2d plane</param>
		/// <returns></returns>
		public static float GetAngleComponent(this Vector3 v3, Component comp, Component refe)
		{
			//interpret extension method to call static method. 
			float compValue;
			float refeValue;
			switch (comp)
			{
				case Component.x:
					compValue = v3.x;
					break;
				case Component.y:
					compValue = v3.y;
					break;
				case Component.z:
					compValue = v3.z;
					break;
				default:
					compValue = 0f;
					break;
			}

			switch (refe)
			{
				case Component.x:
					refeValue = v3.x;
					break;
				case Component.y:
					refeValue = v3.x;
					break;
				case Component.z:
					refeValue = v3.z;
					break;
				default:
					refeValue = 0f;
					break;
			}

			return GetAngleComponent(compValue, refeValue);
		}

		/// <summary>
		/// Gets the angle between a normal vector and one axis (component) as projected on a 2d plane containing the the component dimension and one reference dimension. 
		/// WARNING: MIGHT NOT WORK. ¯\_(ツ)_/¯
		/// Note: Huh?
		/// </summary>
		/// <param name="comp"></param>
		/// <param name="refe"></param>
		/// <returns></returns>
		public static float GetAngleComponent(float comp, float refe)
		{
			//get normalized 2d projection of 3d vector
			Vector2 v2 = new Vector2(comp, refe).normalized;
			//get the angle of that 2d vector 
			return (float) Math.Asin(v2.x).RadToDeg();
		}

		/// <summary>
		/// Convert vector3 to one with only 2 dimensions present. 
		/// </summary>
		/// <param name="flatDimention">The dimension componenet to remove from the Vector3</param>
		public static Vector3 Flatten(this Vector3 v3, Component flatDimention)
		{
			switch (flatDimention)
			{
				case Component.x:
					return v3.RemoveX();
				case Component.y:
					return v3.RemoveY();
				case Component.z:
					return v3.RemoveZ();
				default:
					return v3;
			}
		}

		/// <summary>
		/// Convert vector3 to a vector3 with one dimenstion cut out. 
		/// </summary>
		/// <param name="flatDimention">The dimension componenet to remove from the Vector3</param>
		public static Vector2 FlattenToVector2(this Vector3 v3, Component flatDimention)
		{
			switch (flatDimention)
			{
				case Component.x:
					var flat = v3.RemoveX();
					return new Vector2(flat.y, flat.z);
				case Component.y:
					flat = v3.RemoveY();
					return new Vector2(flat.x, flat.z);
				case Component.z:
					flat = v3.RemoveZ();
					return new Vector2(flat.x, flat.y);
				default:
					return v3;
			}
		}

		/// <summary>
		/// Convert to only y,z components ofthe vector3
		/// </summary>
		public static Vector3 RemoveX(this Vector3 v3) => new Vector3(0, v3.y, v3.z);

		/// <summary>
		/// Convert to only x,z components ofthe vector3
		/// </summary>
		public static Vector3 RemoveY(this Vector3 v3) => new Vector3(v3.x, 0, v3.z);

		/// <summary>
		/// Convert to only x,y components ofthe vector3
		/// </summary>
		public static Vector3 RemoveZ(this Vector3 v3) => new Vector3(v3.x, v3.y, 0);

		/// <summary>
		/// Weighted distance measurement. Exaggerates the vertical to favor horizontal closeness. 
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static float DistanceYWeighted(Vector3 v1, Vector3 v2)
		{
			float deltaY = v2.y - v1.y;
			Vector3 v2e = new Vector3(v2.x, v1.y + Mathf.Pow(deltaY, 3), v2.z);

			return Vector3.Distance(v1, v2);
		}
		

		/// <summary>
		/// Get the point directly between this v3 and another.
		/// </summary>
		/// <param name="point1"></param>
		/// <param name="point2"></param>
		/// <returns></returns>
		public static Vector3 HalfWayTo(this Vector3 point1, Vector3 point2)
		{
			return new Vector3(
				(point1.x + point2.x) / 2,
				(point1.y + point2.y) / 2,
				(point1.z + point2.z) / 2
			);
		}

		/// <summary>
		/// Convert a vector3 from OpenGL's righthanded coordinate system into Unity's lefthanded system.
		/// Also available in 4x4 transform matrix.
		/// </summary>
		/// <param name="rightHand"></param>
		/// <returns></returns>
		public static Vector3 RH2LH(this Vector3 rightHand)
		{
			return new Vector3(rightHand.x, rightHand.y, -rightHand.z);
		}

		/// <summary>
		/// Temporary method to correct the bounds properties coming through. 
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		public static Vector3 FixBounds(this Vector3 original)
		{
			const float scale = 0.3048f;

			return new Vector3(original.x, original.z, original.y) * scale;
		}


		public static float WeightedDistance(Vector3 vectorA, Vector3 vectorB, 
			float xWeight = 1, float yWeight = 1, float zWeight = 1)
		{
			Vector3 vecPrime = vectorB - vectorA;

			return Vector3.Magnitude(new Vector3(
				vecPrime.x * xWeight,
				vecPrime.y * yWeight,
				vecPrime.z * zWeight));
		}
		
		
		#endregion /Vector3


		#region Should probably be extensions

		/// <summary>
		/// To avoid unexpected rotation behavior when rotation passes 360, 
		/// method compares angle with reference and returns the closest in 
		/// number including negative. 
		/// </summary>
		/// <param name="angle">Angle being modified</param>
		/// <param name="reference">Angle to attempt to match</param>
		/// <returns>Modified notation of angle.</returns>
		public static float ClosestAngle(float angle, float reference)
		{
			
			var sorted = new SortedList<float, float>();
			if (!sorted.ContainsKey(angle - reference))
			{sorted.Add(Mathf.Abs(angle - reference), angle);}
			if (!sorted.ContainsKey(Mathf.Abs((angle - 360) - reference)))
			{sorted.Add(Mathf.Abs((angle - 360) - reference), angle - 360);}
			if (!sorted.ContainsKey(Mathf.Abs((angle + 360) - reference)))
			{sorted.Add(Mathf.Abs((angle + 360) - reference), angle + 360);}
			

			return sorted.Values[0];
		}

		/// <summary>
		/// Shortcut to remove the y component of a vector3
		/// </summary>
		/// <param name="vec3">Vector3 to modify</param>
		/// <returns></returns>
		public static Vector3 NoHeight(Vector3 vec3)
		{
			return NoHeight(vec3, 0f);
		}
		/// <summary>
		/// shortcut to replace the y component of a vector3 with a consistent float.
		/// </summary>
		/// <param name="vec3">Vector3 to modify</param>
		/// <param name="height">New y component</param>
		/// <returns></returns>
		public static Vector3 NoHeight(Vector3 vec3, float height)
		{
			return new Vector3(vec3.x, height, vec3.z);
		}
		

		#endregion /Should probably be extensions


		#region ==== New vector3 ====-----------------

		/// <summary>
		/// Creates a randomized vector3 where each component is randomized then the whole is normalized.
		/// </summary>
		/// <returns></returns>
		public static Vector3 Random() =>
			Vector3.Normalize(new Vector3(
				new System.Random().Next(),
				new System.Random().Next(),
				new System.Random().Next()));

		#endregion ------------------/New vector3 ====
		
		/// <summary>
		/// Copypasta from MRTK. Checks whether a vector3 has NaN or infinite values. 
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static bool IsValidVector(this Vector3 vector)
		{
			return !float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) &&
			       !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y) && !float.IsInfinity(vector.z);
		}

		
	}
}