using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Inspector for <see cref="RatEnemyController"/> that conditionally shows
/// panic-related properties only when panic behaviour is enabled.
/// </summary>
/// <remarks>
/// The <c>panicSpeedMultiplier</c> field is indented and shown only when
/// <c>canPanic</c> is <see langword="true"/>, preventing confusion when panic
/// behaviour is disabled on a given rat variant.
/// </remarks>
[CustomEditor(typeof(RatEnemyController))]
public class RatEnemyControllerEditor : Editor
{
    /// <summary>
    /// Draws the custom Inspector layout, always showing melee data and pack-leader
    /// settings, and conditionally revealing panic speed when panic is enabled.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("meleeData"));

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