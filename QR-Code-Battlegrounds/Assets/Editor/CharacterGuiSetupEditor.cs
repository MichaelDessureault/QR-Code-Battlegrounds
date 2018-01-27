using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterGuiSetup))]
public class CharacterGuiSetupEditor : Editor
{

    #region SerializezdProperties
    #region assassin variables
    SerializedProperty assassinHealthBarProp;
    SerializedProperty assassinNameProp;
    SerializedProperty assassinLevelProp;
    SerializedProperty assassinAbilitiesProp;
    SerializedProperty assassinAbilitiesPPProp;
    #endregion


    #region guardian variables
    SerializedProperty guardianHealthBarProp;
    SerializedProperty guardianNameProp;
    SerializedProperty guardianLevelProp;
    SerializedProperty guardianAbilitiesProp;
    SerializedProperty guardianAbilitiesPPProp;
    #endregion


    #region wizard variables
    SerializedProperty wizardHealthBarProp;
    SerializedProperty wizardNameProp;
    SerializedProperty wizardLevelProp;
    SerializedProperty wizardAbilitiesProp;
    SerializedProperty wizardAbilitiesPPProp;
    #endregion
    #endregion

    private bool[] charactersFoldOutArray = new bool[GameManager.numberOfCharacters];
    private bool[] abilityFoldOutArray1 = new bool[Ability.KMaxAbilities];
    private bool[] abilityFoldOutArray2 = new bool[Ability.KMaxAbilities];
    private bool[] abilityFoldOutArray3 = new bool[Ability.KMaxAbilities];

    private void OnEnable()
    {
        assassinHealthBarProp = serializedObject.FindProperty("assassinHealthBar");
        assassinNameProp = serializedObject.FindProperty("assassinName");
        assassinLevelProp = serializedObject.FindProperty("assassinLevel");
        assassinAbilitiesProp = serializedObject.FindProperty("assassinAbilities");
        assassinAbilitiesPPProp = serializedObject.FindProperty("assassinAbilitiesPP");

        guardianHealthBarProp = serializedObject.FindProperty("guardianHealthBar");
        guardianNameProp = serializedObject.FindProperty("guardianName");
        guardianLevelProp = serializedObject.FindProperty("guardianLevel");
        guardianAbilitiesProp = serializedObject.FindProperty("guardianAbilities");
        guardianAbilitiesPPProp = serializedObject.FindProperty("guardianAbilitiesPP");

        wizardHealthBarProp = serializedObject.FindProperty("wizardHealthBar");
        wizardNameProp = serializedObject.FindProperty("wizardName");
        wizardLevelProp = serializedObject.FindProperty("wizardLevel");
        wizardAbilitiesProp = serializedObject.FindProperty("wizardAbilities");
        wizardAbilitiesPPProp = serializedObject.FindProperty("wizardAbilitiesPP");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(CharacterGuiSetup), false);
        GUI.enabled = true;

        EditorGUILayout.LabelField("Characters");
        InsertVertical(0, assassinHealthBarProp, assassinNameProp, assassinLevelProp, assassinAbilitiesProp, assassinAbilitiesPPProp, abilityFoldOutArray1);
        InsertVertical(1, guardianHealthBarProp, guardianNameProp, guardianLevelProp, guardianAbilitiesProp, guardianAbilitiesPPProp, abilityFoldOutArray2);
        InsertVertical(2, wizardHealthBarProp, wizardNameProp, wizardLevelProp, wizardAbilitiesProp, wizardAbilitiesPPProp, abilityFoldOutArray3);

        serializedObject.ApplyModifiedProperties();
    }

    private void InsertVertical(int characterIndex, SerializedProperty healthBar,
        SerializedProperty name, SerializedProperty level, SerializedProperty abilities, SerializedProperty abilitiesPP, bool[] abilityFoldOutArray)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        charactersFoldOutArray[characterIndex] = EditorGUILayout.Foldout(charactersFoldOutArray[characterIndex], GameManager.characterTypeStrings[characterIndex]);
        if (charactersFoldOutArray[characterIndex])
        {
            EditorGUILayout.PropertyField(healthBar, new GUIContent("Health Bar"));
            EditorGUILayout.PropertyField(name, new GUIContent("Name"));
            EditorGUILayout.PropertyField(level, new GUIContent("Level"));

            EditorGUILayout.LabelField("Abilities");

            for (int i = 0; i < Ability.KMaxAbilities; i++)
            {
                AbilityVertical(i, abilities, abilitiesPP, abilityFoldOutArray);
            }
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }


    private void AbilityVertical(int abilityIndex, SerializedProperty abilities, SerializedProperty abilitiesPP, bool[] abilityFoldOutArray)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        abilityFoldOutArray[abilityIndex] = EditorGUILayout.Foldout(abilityFoldOutArray[abilityIndex], "Ability " + abilityIndex);

        if (abilityFoldOutArray[abilityIndex])
        {
            EditorGUILayout.PropertyField(abilities.GetArrayElementAtIndex(abilityIndex), new GUIContent("Text"));
            EditorGUILayout.PropertyField(abilitiesPP.GetArrayElementAtIndex(abilityIndex), new GUIContent("PP"));
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

}