using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Inspector for <see cref="AcidPuddle"/> that conditionally hides
/// lifetime and fade fields when the puddle is marked as permanent.
/// </summary>
/// <remarks>
/// Lifetime-related properties (<c>minLifetime</c>, <c>maxLifetime</c>, <c>fadeDuration</c>)
/// are only shown when <c>isPermanent</c> is <see langword="false"/>, keeping the
/// Inspector uncluttered for permanent environmental hazards.
/// </remarks>
[CustomEditor(typeof(AcidPuddle))]
public class AcidPuddleEditor : Editor
{
    /// <summary>
    /// Draws the custom Inspector layout, rendering damage properties unconditionally
    /// and lifetime/fade properties only when the puddle is not permanent.
    /// </summary>
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