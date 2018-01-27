using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Backpack))]
public class BackpackEditor : Editor
{

    #region SerializezdProperties
    SerializedProperty potionAmountProp;
    SerializedProperty potionButtonProp;
    SerializedProperty etherAmountProp;
    SerializedProperty etherButtonProp;
    SerializedProperty characterGuiSetupProp;
    
    #region assassin variables
    SerializedProperty assassinBoarderProp;
    SerializedProperty assassinButtonProp;
    #endregion


    #region guardian variables
    SerializedProperty guardianBoarderProp;
    SerializedProperty guardianButtonProp;
    #endregion


    #region wizard variables
    SerializedProperty wizardBoarderProp;
    SerializedProperty wizardButtonProp;
    #endregion
    #endregion

    private bool[] charactersFoldOutArray = new bool[GameManager.numberOfCharacters];
    private bool[] abilitiesFoldOutArray = new bool[Ability.KMaxAbilities];

    private void OnEnable()
    {
        potionAmountProp = serializedObject.FindProperty("potionAmountText");
        potionButtonProp = serializedObject.FindProperty("potionButton");
        etherAmountProp = serializedObject.FindProperty("etherAmountText");
        etherButtonProp = serializedObject.FindProperty("etherButton");
        characterGuiSetupProp = serializedObject.FindProperty("characterGuiSetup");
        
        assassinBoarderProp = serializedObject.FindProperty("assassinBoarder");
        assassinButtonProp = serializedObject.FindProperty("assassinButton");

        guardianBoarderProp = serializedObject.FindProperty("guardianBoarder");
        guardianButtonProp = serializedObject.FindProperty("guardianButton");

        wizardBoarderProp = serializedObject.FindProperty("wizardBoarder");
        wizardButtonProp = serializedObject.FindProperty("wizardButton");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour) target), typeof(Backpack), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(potionAmountProp, new GUIContent("StoreData Amount"));
        EditorGUILayout.PropertyField(potionButtonProp, new GUIContent("StoreData Button"));
        EditorGUILayout.PropertyField(etherAmountProp, new GUIContent("Ether Amount"));
        EditorGUILayout.PropertyField(etherButtonProp, new GUIContent("Ether Button"));
        EditorGUILayout.PropertyField(characterGuiSetupProp);

        

        EditorGUILayout.LabelField("Characters");
        InsertVertical(0, assassinBoarderProp, assassinButtonProp);
        InsertVertical(1, guardianBoarderProp, guardianButtonProp);
        InsertVertical(2, wizardBoarderProp,   wizardButtonProp);

        serializedObject.ApplyModifiedProperties();
    }
    
    private void InsertVertical(int characterIndex, SerializedProperty boarder, SerializedProperty button)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        charactersFoldOutArray[characterIndex] = EditorGUILayout.Foldout(charactersFoldOutArray[characterIndex], GameManager.characterTypeStrings[characterIndex]);
        if (charactersFoldOutArray[characterIndex])
        {
            EditorGUILayout.PropertyField(boarder, new GUIContent("Boarder"));
            EditorGUILayout.PropertyField(button, new GUIContent("Button"));
        }
        
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}