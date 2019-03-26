using UnityEditor;
using UnityEngine;

namespace PM
{
    [CustomEditor(typeof(GameToMigration)), CanEditMultipleObjects]
    public class CustomInspector_GameToMigration : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying)
            {
	            EditorGUILayout.HelpBox("\nCurrent running game has ID " + Main.instance.gameDefinition.gameId + ". Make sure that is the correct game.", MessageType.Info, true);

				if (GUILayout.Button("Create migration from json"))
                {
                    Debug.Log("Creating migration for game with id " + Main.instance.gameDefinition.gameId + "...\n");
                    GameToMigration.instance.CreateMigrationFromJson();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("\nEnter playmode to enable migration of game and levels from json", MessageType.Info, true);
            }
        }
    }
}