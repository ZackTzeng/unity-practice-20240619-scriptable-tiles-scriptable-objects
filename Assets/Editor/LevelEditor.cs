using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Level level = (Level)target;

        GUI.enabled = Application.isPlaying;
        
        if (GUILayout.Button("Spawn Unit"))
        {
            level.SpawnUnit();
        }
    }
}
