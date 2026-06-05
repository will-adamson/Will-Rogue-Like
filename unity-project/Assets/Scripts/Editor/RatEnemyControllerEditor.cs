using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RatEnemyController))]
public class RatEnemyControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeEnemyData"));

        EditorGUILayout.Space();

        SerializedProperty isPackLeader = serializedObject.FindProperty("isPackLeader");
        EditorGUILayout.PropertyField(isPackLeader, new GUIContent("Is Pack Leader"));

        EditorGUILayout.Space();

        SerializedProperty canPanic = serializedObject.FindProperty("canPanic");
        EditorGUILayout.PropertyField(canPanic);

        if (canPanic.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("panicSpeedMultiplier"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}