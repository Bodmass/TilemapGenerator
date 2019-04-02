using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TopdownDungeonGenerator))]
public class TopdownDungeonGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TopdownDungeonGenerator tilemapGenerator = (TopdownDungeonGenerator)target;
        if (GUILayout.Button("Regenerate"))
        {
            tilemapGenerator.Regenerate();
        }
    }
}
