using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(CharacterSelectionManager))]
class CharacterSelectionManagerEditor : Editor
{
    #region SerializedProperties
    SerializedProperty deathColorProp;
    SerializedProperty enemyBlurProp;
    SerializedProperty characterBlurProp;
    SerializedProperty characterSelectionContainerProp;
    SerializedProperty charactersRawImageArrayProp;
    SerializedProperty charactersHealthBarArrayProp;
    #endregion

    private bool[] showCharacterSlots = new bool[GameManager.numberOfCharacters];

    private void OnEnable()
    {
        deathColorProp = serializedObject.FindProperty("deathColor");
        enemyBlurProp = serializedObject.FindProperty("enemyBlur");
        characterBlurProp = serializedObject.FindProperty("characterBlur");
        characterSelectionContainerProp = serializedObject.FindProperty("characterSelectionContainer");
        charactersRawImageArrayProp = serializedObject.FindProperty("charactersRawImageArray");
        charactersHealthBarArrayProp = serializedObject.FindProperty("charactersHealthBarArray");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(CharacterSelectionManager), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(deathColorProp, new GUIContent("Death Color"));
        EditorGUILayout.PropertyField(enemyBlurProp, new GUIContent("Enemy Blur Image"));
        EditorGUILayout.PropertyField(characterBlurProp, new GUIContent("Character Blur Image"));
        EditorGUILayout.PropertyField(characterSelectionContainerProp, new GUIContent("Character Selection Container"));

        EditorGUILayout.LabelField("Character Raw Images And Healthbars");
        
        for (int i = 0; i < GameManager.numberOfCharacters; i++)
        {
            InsertVertical(i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void InsertVertical (int index)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        showCharacterSlots[index] = EditorGUILayout.Foldout(showCharacterSlots[index], GameManager.characterTypeStrings[index]);

        if (showCharacterSlots[index])
        {
        EditorGUILayout.PropertyField(charactersRawImageArrayProp.GetArrayElementAtIndex(index),
            new GUIContent(GameManager.characterTypeStrings[index]));
        EditorGUILayout.PropertyField(charactersHealthBarArrayProp.GetArrayElementAtIndex(index),
            new GUIContent("Health Bar"));
        }


        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
