using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleGUIManager))]
public class BattleGUIManagerEditor : Editor {
    #region SerializedProperties

    #region Text Window
    SerializedProperty textWindowProp;
    SerializedProperty textWindowTextProp;
    #endregion

    #region Selection / Ability Window
    SerializedProperty selectionWindowProp;
    SerializedProperty abilityButtonsTextProp;
    SerializedProperty abilityPPTextProp;
    SerializedProperty backButtonTextProp;
    #endregion

    #region BackPack Store window
    SerializedProperty backpackStoreWindowProp;
    SerializedProperty backpackStoreButtonText;
    #endregion

    #region Action Window 
    SerializedProperty actionWindowProp;
    SerializedProperty runButtonProp;
    SerializedProperty bossButtonProp;
    #endregion

    #region Sprites
    SerializedProperty enemyImageLocProp;
    SerializedProperty playerImageLocProp;
    #endregion

    #region Particle System
    SerializedProperty enemyParticleSystemProp;
    SerializedProperty playerParticleSystemProp;
    #endregion

    SerializedProperty yesNoWindowProp;
    SerializedProperty messageWaitTimeProp;
    SerializedProperty backgroundQuadProp;

    #endregion

    private bool[] showAbilitySlots = new bool[Ability.KMaxAbilities];

    private void OnEnable() {
        textWindowProp = serializedObject.FindProperty("textWindow");
        textWindowTextProp = serializedObject.FindProperty("textWindowText");

        selectionWindowProp = serializedObject.FindProperty("selectionWindow");
        abilityButtonsTextProp = serializedObject.FindProperty("abilityButtonsText");
        abilityPPTextProp = serializedObject.FindProperty("abilityPPText");
        backButtonTextProp = serializedObject.FindProperty("backButtonText");



        backpackStoreWindowProp = serializedObject.FindProperty("backpackStoreWindow");
        backpackStoreButtonText = serializedObject.FindProperty("backpackStoreButtonText");

        actionWindowProp = serializedObject.FindProperty("actionWindow");
        runButtonProp = serializedObject.FindProperty("runButton");
        bossButtonProp = serializedObject.FindProperty("bossButton");

        enemyImageLocProp = serializedObject.FindProperty("enemySpriteRenderer");
        playerImageLocProp = serializedObject.FindProperty("playerSpriteRenderer");

        enemyParticleSystemProp = serializedObject.FindProperty("enemyParticleSystem");
        playerParticleSystemProp = serializedObject.FindProperty("playerParticleSystem");

        yesNoWindowProp = serializedObject.FindProperty("yesNoWindow");
        messageWaitTimeProp = serializedObject.FindProperty("textMessageWaitForDelay");
        backgroundQuadProp = serializedObject.FindProperty("backgroundQuad");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(BattleGUIManager), false);
        GUI.enabled = true;

        // Text wait time
        EditorGUILayout.Slider(messageWaitTimeProp, 0.1f, 1.5f, new GUIContent("Text Wait Time"));
        EditorGUILayout.PropertyField(backgroundQuadProp, new GUIContent("Background Quad"));
        
        // Yes no window
        EditorGUILayout.PropertyField(yesNoWindowProp, new GUIContent("Yes No Window"));


        // Backpack store window
        EditorGUILayout.PropertyField(backpackStoreWindowProp, new GUIContent("Backpack Store Window"));
        EditorGUILayout.PropertyField(backpackStoreButtonText, new GUIContent("Backpack Store Button Text"));

        // Action window
        EditorGUILayout.LabelField("Action Window");
        EditorGUI.indentLevel++;
          EditorGUILayout.PropertyField(actionWindowProp, new GUIContent("Action Window"));
          EditorGUILayout.PropertyField(runButtonProp, new GUIContent("Run Button"));
          EditorGUILayout.PropertyField(bossButtonProp, new GUIContent("Boss Button"));
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();


        // Selection / Ability window
        EditorGUILayout.LabelField("Selection Window");
        EditorGUI.indentLevel++;
          EditorGUILayout.PropertyField(selectionWindowProp, new GUIContent("Selection Window"));
          EditorGUILayout.PropertyField(backButtonTextProp, new GUIContent("Back Button Text"));
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Ability Window");
        for (int i = 0; i < Ability.KMaxAbilities; i++) {
            AbilitySlot(i);
        }

        EditorGUILayout.Space();
        LabelFieldWithMultipleIndentedProperties("Text Window", new SerializedProperty[] { textWindowProp, textWindowTextProp }, new string[] { "Component", "Text" });
        LabelFieldWithMultipleIndentedProperties("Sprites", new SerializedProperty[] { enemyImageLocProp, playerImageLocProp }, new string[] { "Enemy", "Player" });
        LabelFieldWithMultipleIndentedProperties("Particle Systems", new SerializedProperty[] { enemyParticleSystemProp, playerParticleSystemProp }, new string[] { "Enemy", "Player" });


        serializedObject.ApplyModifiedProperties();
    }


    private void AbilitySlot(int index) {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        showAbilitySlots[index] = EditorGUILayout.Foldout(showAbilitySlots[index], "Ability " + (index + 1));

        if (showAbilitySlots[index]) {
            EditorGUILayout.PropertyField(abilityButtonsTextProp.GetArrayElementAtIndex(index), new GUIContent("Name"));
            EditorGUILayout.PropertyField(abilityPPTextProp.GetArrayElementAtIndex(index), new GUIContent("PP Amount"));
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    void LabelFieldWithMultipleIndentedProperties(string fieldLabel, SerializedProperty[] serializedProperties, string[] propertyNames) {
        if (serializedProperties.Length <= 0 || propertyNames.Length <= 0 || (serializedProperties.Length != propertyNames.Length)) {
            Debug.LogWarning("Serilized property array doesn't max up in size with propertyName array");
            return;
        }

        EditorGUILayout.LabelField(fieldLabel);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;
        for (int i = 0; i < serializedProperties.Length; i++) {
            EditorGUILayout.PropertyField(serializedProperties[i], new GUIContent(propertyNames[i]));
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
}