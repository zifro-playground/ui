using UnityEditor;
using UnityEngine;

namespace PM
{
    [CustomEditor(typeof(TestLevels)), CanEditMultipleObjects]
    public class CustomInspector_TestLevels : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Test Levels"))
                {
                    TestLevels.Instance.IsTesting = true;
                    Debug.Log("Testing Levels\n");
                    TestLevels.Instance.TestLevel();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("\nEnter playmode to start testing", MessageType.Info, true);
            }
        }
    }
}