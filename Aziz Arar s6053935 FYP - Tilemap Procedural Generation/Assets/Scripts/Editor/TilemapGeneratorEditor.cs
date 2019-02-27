using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapGenerator))]
public class TilemapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TilemapGenerator tilemapGenerator = (TilemapGenerator)target;
        if(GUILayout.Button("Regenerate"))
        {
            tilemapGenerator.Regenerate();
        }
    }
}
