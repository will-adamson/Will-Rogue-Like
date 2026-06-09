using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeleeEnemyData))]
public class MeleeEnemyDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MeleeEnemyData data = (MeleeEnemyData)target;

        EditorGUILayout.LabelField("Base Stats", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("sprite"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("health"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defence"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Detection", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("detectionRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isHoldPosition"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Attack", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("telegraphDuration"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("danger"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("xpReward"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("knockbackForce"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackCooldown"));
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Attack Shape", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackShape"));

        switch (data.attackShape)
        {
            case AttackShape.Arc:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("attackAngle"));
                break;

            case AttackShape.Radial:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("attackHitCount"));
                break;

            case AttackShape.Point:
                break;
        }
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Movement Pattern", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movementPattern"));

        switch (data.movementPattern)
        {
            case MovementPattern.CircleStrafe:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("circleStrafeDist"));
                break;

            case MovementPattern.Charge:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("chargeCooldown"));
                break;

            case MovementPattern.Zigzag:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("zigzagAmplitude"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("zigzagFrequency"));
                break;

            case MovementPattern.Direct:
                break;
        }
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Hit and Death", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canBeStaggered"));

        if (data.canBeStaggered)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("staggerThreshold"));

        EditorGUILayout.PropertyField(serializedObject.FindProperty("onHitEffect"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("splitOnDeathChance"));

        serializedObject.ApplyModifiedProperties();
    }
}