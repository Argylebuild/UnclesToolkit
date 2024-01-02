// In ArgyleComponentInspector.cs
using UnityEditor;
using UnityEngine;

namespace Argyle.UnclesToolkit.Editor
{
	[CustomEditor(typeof(ArgyleComponent), true)]
	public class ArgyleComponentEditor : UnityEditor.Editor
	{
		bool _showFoldout = false;
		SerializedProperty _usageNotesProp;

		void OnEnable()
		{
			_usageNotesProp = serializedObject.FindProperty("_usageNotes");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawPropertiesExcluding(serializedObject, "_usageNotes");

			_showFoldout = EditorGUILayout.Foldout(_showFoldout, "Docs");
			if (_showFoldout)
			{
				EditorGUILayout.PropertyField(_usageNotesProp, GUILayout.MinHeight(4 * EditorGUIUtility.singleLineHeight), GUILayout.MaxHeight(12 * EditorGUIUtility.singleLineHeight));
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}