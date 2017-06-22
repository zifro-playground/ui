using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PM {

	[CustomEditor(typeof(MatchViewportToGameRect))]
	public class CustomInspector_MatchViewportToGameRect : Editor {

		private MatchViewportToGameRect script;

		private void OnEnable() {
			script = target as MatchViewportToGameRect;
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (script.runInEditor && !script.isValid) {
				EditorGUILayout.HelpBox("ERROR!\nUnable to find the game rect. Are you missing the IDE?", MessageType.Error);

				serializedObject.Update();

				EditorGUILayout.PropertyField(serializedObject.FindProperty("theRect"), new GUIContent("Manually find it:"));

				serializedObject.ApplyModifiedProperties();

			}
		}

	}

}