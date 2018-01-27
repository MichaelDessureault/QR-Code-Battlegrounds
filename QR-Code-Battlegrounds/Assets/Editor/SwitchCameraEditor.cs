using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SwitchCamera))]
public class SwitchCameraEditor : Editor {

    #region SerializedProperties
    SerializedProperty cameraProp;
    SerializedProperty playerCCProp;
    SerializedProperty birdsEyeCCProp;
    SerializedProperty birdsEyeViewTrackerProp;
    SerializedProperty playersTrackObject;
    SerializedProperty playerLookAtObjectProp;
    SerializedProperty birdsEyeViewLookAtObjectProp;
    SerializedProperty mainLookAtObjectProp;
    SerializedProperty playerCameraIconProp;
    SerializedProperty birdsEyeCameraIconProp;
    #endregion

    private void OnEnable()
    {
        cameraProp = serializedObject.FindProperty("camera");
        playerCCProp = serializedObject.FindProperty("PlayersCharacterController");
        birdsEyeCCProp = serializedObject.FindProperty("BirdsEyeCharacterController");
        birdsEyeViewTrackerProp = serializedObject.FindProperty("birdsEyeViewTracker");
        playersTrackObject = serializedObject.FindProperty("playersTracker");
        playerLookAtObjectProp = serializedObject.FindProperty("playerLookAtObject");
        birdsEyeViewLookAtObjectProp = serializedObject.FindProperty("birdsEyeViewLookAtObject");
        mainLookAtObjectProp = serializedObject.FindProperty("mainLookAtObject");
        playerCameraIconProp = serializedObject.FindProperty("playerCameraIcon");
        birdsEyeCameraIconProp = serializedObject.FindProperty("birdsEyeCameraIcon");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(SwitchCamera), false);
        GUI.enabled = true;

        EditorGUILayout.PropertyField(cameraProp);
        
        LabelFieldWithMultipleIndentedProperties("Character Controller", new SerializedProperty[] { playerCCProp, birdsEyeCCProp }, new string[] { "Player", "Birds Eye" });
        LabelFieldWithMultipleIndentedProperties("Track Object", new SerializedProperty[] { playersTrackObject, birdsEyeViewTrackerProp }, new string[] { "Players", "Birds Eye" });
        LabelFieldWithMultipleIndentedProperties("Look At Object", new SerializedProperty[] { playerLookAtObjectProp, birdsEyeViewLookAtObjectProp, mainLookAtObjectProp }, new string[] { "Player", "Birds Eye", "Main" });
        LabelFieldWithMultipleIndentedProperties("Camera Icons", new SerializedProperty[] { playerCameraIconProp, birdsEyeCameraIconProp }, new string[] { "Players Icon", "Birds Eye Icon" });

        serializedObject.ApplyModifiedProperties();
    }


    // The serlizedProperties array so to the same index with the propertiy names
    void LabelFieldWithMultipleIndentedProperties(string fieldLabel, SerializedProperty[] serializedProperties, string[] propertyNames)
    {
        if (serializedProperties.Length <= 0 || propertyNames.Length <= 0 || (serializedProperties.Length != propertyNames.Length))
        {
            Debug.LogWarning("Serilized property array doesn't max up in size with propertyName array");
            return;
        }

        EditorGUILayout.LabelField(fieldLabel);
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;
        for (int i = 0; i < serializedProperties.Length; i++)
        {
            EditorGUILayout.PropertyField(serializedProperties[i], new GUIContent(propertyNames[i]));
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }
}