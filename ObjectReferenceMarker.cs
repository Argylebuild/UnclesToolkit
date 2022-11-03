using UnityEngine;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// Used in combination with ObjectReference for loosely coupled, prefab-safe object references in a scene.
	/// Place an ObjectReference on the object you are looking to reference.
	/// Populate with an ObjectReferenceMarker SO.
	/// Add a reference to the marker into the script attempting to make the reference.
	/// Populate that field with the same marker. 
	/// </summary>
	[CreateAssetMenu( fileName = "New ObjectReferenceMarker", menuName = "Argyle/Scriptable Objects/Variables/ObjectReferenceMarker")]
	public class ObjectReferenceMarker : ArgyleScriptableObject
	{
		/// <summary>
		/// Find the object in the scene using referenced by this SO asset marker. 
		/// </summary>
		public ObjectReference Reference => ObjectReference.GetReference(this);
	}
}
