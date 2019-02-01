using UnityEngine;
using UnityEditor;

public class GenerationWindow : EditorWindow
{

    string myLevel = "My Level";
    int perspectiveSelected = 2;
    int levelSelected = 2;
    int gridX = 32;
    int gridY = 32;
    static string[] perspectiveOptions = new string[] { "2D Tilemap", "2D Platformer", "Isometric" };
    static string[] levelOptions = new string[] { "World Map", "Dungeon", "Town" };

    public static object[] DropZone(string title, int w, int h)
    {
        GUILayout.Box(title, GUILayout.Width(w), GUILayout.Height(h));

        EventType eventType = Event.current.type;
        bool isAccepted = false;

        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                isAccepted = true;
            }
            Event.current.Use();
        }

        return isAccepted ? DragAndDrop.objectReferences : null;
    }

    [MenuItem("Aziz/Tilemap Generation")]
    public static void DisplayWindow()
    {
        GetWindow<GenerationWindow>("Tilemap Generator");
    }


    void OnGUI ()
    {
        bool canGenerate = true;
        GUILayout.Label("Tilemap Generator\nby Aziz Arar\nv0.01\n", EditorStyles.centeredGreyMiniLabel);


        //EditorGUI.Toggle(rect, gUIContent, false);#
        myLevel = EditorGUILayout.TextField("Level Name:", myLevel);
        GUILayout.Label("\nTilemap Perspective:", EditorStyles.boldLabel);

        perspectiveSelected = GUILayout.SelectionGrid(perspectiveSelected, perspectiveOptions, perspectiveOptions.Length, EditorStyles.radioButton);


        if (perspectiveSelected == 0)
        {
            GUILayout.Label("\nLevel Type:", EditorStyles.boldLabel);
            levelSelected = GUILayout.SelectionGrid(levelSelected, levelOptions, levelOptions.Length, EditorStyles.radioButton);
        }

        if (perspectiveSelected == 0 && levelSelected == 0)
        {
            GUILayout.Label(" ", EditorStyles.miniLabel);
            gridX = EditorGUILayout.IntField("Grid X", gridX, EditorStyles.miniTextField);
            gridY = EditorGUILayout.IntField("Grid Y", gridY, EditorStyles.miniTextField);
            //gridY = int.Parse(GUILayout.TextField(gridY));
            if(gridX <= 0)
            {
                gridX = 0;
            }

            if (gridY <= 0)
            {
                gridY = 0;
            }

            if (gridX <= 256 && gridY <= 256)
            {

                if(gridX == 0 || gridY == 0)
                {
                    GUILayout.Label("\nError: The size of the grid must be atleast 1x1", EditorStyles.miniLabel);
                    canGenerate = false;
                }
                else
                {
                    GUILayout.Label("\nThis will generate " + (gridX * gridY).ToString() + " cells", EditorStyles.miniLabel);
                }


            }
            else
            {
                GUILayout.Label("\nError: The size limit in each axis is 256!", EditorStyles.miniLabel);
                canGenerate = false;
            }
        }
        else
        {
            canGenerate = false;
            GUILayout.Label("\nThis is not yet implemented, sorry.", EditorStyles.miniLabel);
        }

        GUILayout.Label("\nDrag n' Drop a Tile Pallete you wish to use:", EditorStyles.boldLabel);
        DropZone("Tileset", 50, 50);

        if (!canGenerate)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Generate", EditorStyles.miniButton))
        {
            Debug.Log("Generate");
        }
        GUI.enabled = true;


        
    }


}
