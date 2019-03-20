using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TopdownWorldMapGenerator))]
public class TopdownWorldMapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TopdownWorldMapGenerator tilemapGenerator = (TopdownWorldMapGenerator)target;
        if(GUILayout.Button("Regenerate"))
        {
            tilemapGenerator.Regenerate();
        }
    }
}
