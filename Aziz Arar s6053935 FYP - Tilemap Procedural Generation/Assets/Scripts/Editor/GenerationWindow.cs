using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class GenerationWindow : EditorWindow
{

    string myLevel = "Untitled";
    int creationModeSelected = 0;
    int perspectiveSelected = 0;
    int levelSelected = 0;
    int[] grid = new int[] { 32, 32 };
    int CellSize = 1;
    int PixelsPerUnit = 32;
    static string[] creationModeOptions = new string[] { "Simple", "Advanced" };
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

        GUILayout.Label("\nCreation Mode:", EditorStyles.boldLabel);

        creationModeSelected = GUILayout.SelectionGrid(creationModeSelected, creationModeOptions, creationModeOptions.Length, EditorStyles.radioButton);
        if(creationModeSelected == 1)
        {
            GUILayout.Label("(*) - Advanced Options", EditorStyles.miniLabel);
        }
        GUILayout.Label("\n", EditorStyles.boldLabel);

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

            grid[0] = EditorGUILayout.IntField("Grid X", grid[0], EditorStyles.miniTextField);
            grid[1] = EditorGUILayout.IntField("Grid Y", grid[1], EditorStyles.miniTextField);
            //gridY = int.Parse(GUILayout.TextField(gridY));
            if(grid[0] <= 0)
            {
                grid[0] = 0;
            }

            if (grid[1] <= 0)
            {
                grid[1] = 0;
            }

            if (grid[0] <= 256 && grid[1] <= 256)
            {

                if(grid[0] == 0 || grid[1] == 0)
                {
                    GUILayout.Label("\nError: The size of the grid must be atleast 1x1", EditorStyles.miniLabel);
                    canGenerate = false;
                }
                else
                {
                    GUILayout.Label("\nThis will generate " + (grid[0] * grid[1]).ToString() + " cells.\n", EditorStyles.miniLabel);
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

        if (creationModeSelected == 1)
        {
            CellSize = EditorGUILayout.IntField("(*) Cell Size", CellSize, EditorStyles.miniTextField);
            PixelsPerUnit = EditorGUILayout.IntField("(*) Pixels Per Unit", PixelsPerUnit, EditorStyles.miniTextField);
            GUILayout.Label("Do not change unless you know what you are doing. The default is the same as the demo Tile Pallete.", EditorStyles.helpBox);
            GUILayout.Label("\nDrag n' Drop a Tile Pallete you wish to use:", EditorStyles.boldLabel);
        }

        DropZone("Tileset", 50, 50);

        if (!canGenerate)
        {
            GUI.enabled = false;
        }
        if (GUILayout.Button("Generate", EditorStyles.miniButton))
        {
            Debug.Log("Generate");

            GameObject newGrid = new GameObject("Grid: "+myLevel, typeof(Grid));
            GameObject newTilemap = new GameObject("Tilemap", typeof(Tilemap));
            newTilemap.GetComponent<Transform>().SetParent(newGrid.transform);

            newGrid.AddComponent<TilemapGenerator>();
            newGrid.GetComponent<Grid>().cellSize = new Vector3(CellSize, CellSize, 0);
        }
        GUI.enabled = true;


        
    }


}
