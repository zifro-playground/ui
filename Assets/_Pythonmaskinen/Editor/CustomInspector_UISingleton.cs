using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace PM {

	[CustomEditor(typeof(UISingleton))]
	public class CustomInspector_UISingleton : Editor {

		private ReorderableList manusList;

		private void OnEnable() {
			manusList = new ReorderableList(serializedObject, serializedObject.FindProperty("manusSelectables"), true, true, true, true);

			manusList.drawHeaderCallback += (Rect rect) => {
				EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2, rect.height), manusList.serializedProperty.displayName, EditorStyles.boldLabel);
				EditorGUI.LabelField(new Rect(rect.x + rect.width / 2 + 8, rect.y, rect.width / 2 - 8, rect.height), "Name(s)", EditorStyles.boldLabel);
			};

			manusList.elementHeight = EditorGUIUtility.singleLineHeight + 12;

			manusList.elementHeightCallback += (int index) => {
				return manusList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("names").arraySize * (EditorGUIUtility.singleLineHeight + 2) + 10;
			};

			manusList.drawElementCallback += (Rect rect, int index, bool isActive, bool isFocused) => {
				var selProp = manusList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("selectable");
				var namesProp = manusList.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative("names");

				Rect selRect = new Rect(rect.x, rect.y, rect.width / 2 - 2, EditorGUIUtility.singleLineHeight);
				EditorGUI.PropertyField(selRect, selProp, GUIContent.none);

				for (int i = 0; i < namesProp.arraySize + 1; i++) {
					Rect nameRect = new Rect(selRect.xMax + 4, rect.y + (EditorGUIUtility.singleLineHeight+2) * i, rect.width/2-2, EditorGUIUtility.singleLineHeight);

					if (namesProp.arraySize > 0) {
						if (i == namesProp.arraySize - 1)
							nameRect.width = nameRect.width * 2 / 3 - 4;
						else if (i == namesProp.arraySize) {
							nameRect.xMin += nameRect.width * 2 / 3;
							nameRect.y -= EditorGUIUtility.singleLineHeight + 2;
						}
					}

					if (i == namesProp.arraySize) {
						GUIStyle style = EditorStyles.centeredGreyMiniLabel;
						style.normal.textColor = Color.black;
						EditorGUI.LabelField(nameRect, "<new>", style);

						string name = EditorGUI.TextField(nameRect, "").Trim();

						if (name.Length > 0) {
							namesProp.InsertArrayElementAtIndex(i);
							namesProp.GetArrayElementAtIndex(i).stringValue = name;
						}
					} else {
						var prop = namesProp.GetArrayElementAtIndex(i);

						EditorGUI.LabelField(Rect.zero, "<new>");
						EditorGUI.BeginProperty(nameRect, GUIContent.none, prop);

						prop.stringValue = EditorGUI.TextField(nameRect, prop.stringValue).Trim();
						if (prop.stringValue.Length == 0) {
							namesProp.DeleteArrayElementAtIndex(i);
						}

						EditorGUI.EndProperty();
					}
				}
			};
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			EditorGUILayout.Space();

			serializedObject.Update();
			manusList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();

			EditorGUILayout.Space();

		}

	}

}