using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PlatformerGenerator))]
public class PlatformerGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlatformerGenerator tilemapGenerator = (PlatformerGenerator)target;
        if (GUILayout.Button("Regenerate"))
        {
            tilemapGenerator.Regenerate();
        }
    }
}
