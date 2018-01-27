using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Sprites))]
public class SpritesEditor : Editor {
    #region SerializedProperties
    SerializedProperty enemySpritesProp;
    SerializedProperty characterFrontSpritesProp;
    SerializedProperty characterBackSprites;
    SerializedProperty battleSceneBackgroundMaterialsProp;
    #endregion
    
    private bool[] showCharacterSlots = new bool[GameManager.numberOfCharacters];
    private void OnEnable()
    {
        enemySpritesProp = serializedObject.FindProperty("enemySprites");
        characterFrontSpritesProp = serializedObject.FindProperty("characterFrontSprites");
        characterBackSprites = serializedObject.FindProperty("characterBackSprites");
        battleSceneBackgroundMaterialsProp = serializedObject.FindProperty("battleSceneBackgroundMaterials");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(SwitchCamera), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(enemySpritesProp, true);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(battleSceneBackgroundMaterialsProp, true);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Character Sprites");
        for (int i = 0; i < 3; i++)
        {
            CharacterSlot(i);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CharacterSlot (int index)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        showCharacterSlots[index] = EditorGUILayout.Foldout(showCharacterSlots[index], GameManager.characterTypeStrings[index]);

        if (showCharacterSlots[index])
        {
            EditorGUILayout.PropertyField(characterFrontSpritesProp.GetArrayElementAtIndex(index), new GUIContent("Front Image"));
            EditorGUILayout.PropertyField(characterBackSprites.GetArrayElementAtIndex(index), new GUIContent("Back Image"));
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
