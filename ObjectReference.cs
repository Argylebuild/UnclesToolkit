using System.Collections.Generic;
using Argyle.Utilities;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	/// Used in combination with ObjectReferenceMarker for loosely coupled, prefab-safe object references in a scene.
	/// Place an ObjectReference on the object you are looking to reference.
	/// Populate with an ObjectReferenceMarker SO.
	/// Add a reference to the marker into the script attempting to make the reference.
	/// Populate that field with the same marker. 
	public class ObjectReference : ArgyleComponent
	{
		public ObjectReferenceMarker marker;

		static Dictionary<ObjectReferenceMarker, ObjectReference> referenceDic;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="marker"></param>
		/// <param name="refreshList"></param>
		/// <returns></returns>
		public static ObjectReference GetReference(ObjectReferenceMarker marker, bool refreshList = false)
		{
			if (referenceDic == null || refreshList)
				PopulateDic();
			else if(!referenceDic.ContainsKey(marker))
				PopulateDic();
			else if(referenceDic[marker] == null)
				PopulateDic();

			if (referenceDic.ContainsKey(marker))
				return referenceDic[marker];
			
			Debug.LogError($"No reference found on the {marker.name} reference marker.");
			return null;
		}

		public static void PopulateDic()//honestly might be overoptimized. If bugs, ditch static dic. 
		{
			referenceDic = new Dictionary<ObjectReferenceMarker, ObjectReference>();
			foreach (var objectReference in FindObjectsOfType<ObjectReference>(true))
			{
				if(referenceDic.ContainsKey(objectReference.marker))
				{
					var original = referenceDic[objectReference.marker];//for debug
					var duplicate = objectReference;
					Debug.LogWarning($"Duplicate markers found for {objectReference.marker.name}. \n" +
					                 $"Original on object: {GetHierarchy(original)}. \n" +
					                 $"Duplicate on object: {GetHierarchy(duplicate)}");
					
					//Destroy(objectReference); //for that weird camera copy thing. 
				}				
				else
					referenceDic.Add(objectReference.marker, objectReference);
			}
		}

		private static string GetHierarchy(ObjectReference reference)
		{
			string hierarchy = reference.GO.name;
			var parent = reference.GO.transform.parent;
			while (parent != null)
			{
				hierarchy = parent.name + "/" + hierarchy;
				parent = parent.parent;
			}

			return hierarchy;
		}
	}
}