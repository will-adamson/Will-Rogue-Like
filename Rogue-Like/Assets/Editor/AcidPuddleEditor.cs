using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AcidPuddle))]
public class AcidPuddleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("damagePerTick"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("tickInterval"));

        EditorGUILayout.Space();
        SerializedProperty isPermanent = serializedObject.FindProperty("isPermanent");
        EditorGUILayout.PropertyField(isPermanent);

        if (!isPermanent.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minLifetime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLifetime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fadeDuration"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}