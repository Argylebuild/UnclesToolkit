using UnityEditor;
using UnityEngine;

namespace Argyle.UnclesToolkit.Editor
{
	[CustomEditor(typeof(ArgyleComponent))]
	public class ArgyleComponentEditor : UnityEditor.Editor {
		bool _showFoldout = true;
		SerializedProperty _usageNotesProp;

		void OnEnable() {
			// Link the serialized property
			_usageNotesProp = serializedObject.FindProperty("_usageNotes");
		}

		public override void OnInspectorGUI() {
			// Start handling serialized properties
			serializedObject.Update();

			// Manually draw all properties except _usageNotes
			DrawPropertiesExcluding(serializedObject, "_usageNotes");

			// Custom foldout for _usageNotes
			_showFoldout = EditorGUILayout.Foldout(_showFoldout, "Docs");
			if (_showFoldout) {
				EditorGUILayout.PropertyField(_usageNotesProp, GUILayout.MinHeight(4 * EditorGUIUtility.singleLineHeight), GUILayout.MaxHeight(12 * EditorGUIUtility.singleLineHeight));
			}

			// Apply changes to the serialized properties
			serializedObject.ApplyModifiedProperties();
		}
	}
}