using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(GameManager))]
class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Delete Files"))
        {
            GameManager gm = (GameManager)target;
            gm.ResetGameData();
        }
    }
}
