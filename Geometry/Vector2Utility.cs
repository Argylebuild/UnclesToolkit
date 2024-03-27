using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
	public static class Vector2Utility
	{
		public static Vector2Int ToVector2Int(this Vector2 v2)
		{
			return new Vector2Int((int)v2.x, (int)v2.y);
		}
	}
}