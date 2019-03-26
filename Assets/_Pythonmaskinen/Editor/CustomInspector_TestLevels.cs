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
                    TestLevels.instance.isTesting = true;
                    Debug.Log("Testing Levels\n");
                    TestLevels.instance.TestLevel();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("\nEnter playmode to start testing", MessageType.Info, true);
            }
        }
    }
}