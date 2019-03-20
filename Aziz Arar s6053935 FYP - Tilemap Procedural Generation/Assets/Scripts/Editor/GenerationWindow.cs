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

    GridPalette palette;
    Object obj_Water;
    Object obj_Shore;
    Object obj_Grass;
    Object obj_Mountain;
    Vector2 scrollPos = Vector2.zero;

    public static object[] DropZone(string title, int w, int h, TileBase p)
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
                //DragAndDrop.
                p = DragAndDrop.objectReferences.GetValue(0) as TileBase;
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
        float width = this.position.width;
        float height = this.position.height;

        float viewWidth = 1024;
        float viewHeight = 768;

        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(width), GUILayout.Height(height));

        //
        bool canGenerate = true;
        GUILayout.Label("Tilemap Generator\nby Aziz Arar\nv0.27\n", EditorStyles.centeredGreyMiniLabel);

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

        if (perspectiveSelected == 0 && levelSelected != 2)
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

        }



        //DropZone("Water", 75, 50, Water);
        //DropZone("Grass", 75, 50, Grass);
        //DropZone("Shore", 75, 50, Shore);
        //DropZone("Mountain", 75, 50, Mountain);

        if (perspectiveSelected == 0 && levelSelected == 0)
        {
            GUILayout.Label("\nDrag n' Drop the Tiles you wish to use:", EditorStyles.boldLabel);
            obj_Grass = EditorGUILayout.ObjectField("Grass", obj_Grass, typeof(TileBase), true);
            obj_Water = EditorGUILayout.ObjectField("Water", obj_Water, typeof(TileBase), true);
            obj_Shore = EditorGUILayout.ObjectField("Shore", obj_Shore, typeof(TileBase), true);
            obj_Mountain = EditorGUILayout.ObjectField("Mountain", obj_Mountain, typeof(TileBase), true);
        }
        if (perspectiveSelected == 0 && levelSelected == 1)
        {
            GUILayout.Label("\nDrag n' Drop the Tiles you wish to use:", EditorStyles.boldLabel);
            obj_Grass = EditorGUILayout.ObjectField("Floor", obj_Grass, typeof(TileBase), true);
            obj_Water = EditorGUILayout.ObjectField("Walls", obj_Water, typeof(TileBase), true);
        }


            if (!canGenerate)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Generate", EditorStyles.miniButton))
        {

            if (perspectiveSelected == 0 && levelSelected == 0) //World Map
            {
                Debug.Log("Generate Perlin Noise World Map");

                GameObject newGrid = new GameObject("Topdown : World Map : Grid: " + myLevel, typeof(Grid));
                GameObject newTilemap = new GameObject("Tilemap", typeof(Tilemap));
                newTilemap.GetComponent<Transform>().SetParent(newGrid.transform);

                newTilemap.AddComponent<TopdownWorldMapGenerator>();
                newTilemap.GetComponent<TopdownWorldMapGenerator>().SetGrid(grid[0], grid[1]);
                newGrid.GetComponent<Grid>().cellSize = new Vector3(CellSize, CellSize, 0);
                newTilemap.AddComponent<TilemapRenderer>();

                newTilemap.GetComponent<TopdownWorldMapGenerator>().Water = obj_Water as TileBase;
                newTilemap.GetComponent<TopdownWorldMapGenerator>().Shore = obj_Shore as TileBase;
                newTilemap.GetComponent<TopdownWorldMapGenerator>().Grass = obj_Grass as TileBase;
                newTilemap.GetComponent<TopdownWorldMapGenerator>().Mountain = obj_Mountain as TileBase;
                newTilemap.GetComponent<Tilemap>().animationFrameRate = 60f;
            }
            if (perspectiveSelected == 0 && levelSelected == 1) //Dungeon
            {

                Debug.Log("Generated BSP Dungeon");

                GameObject newGrid = new GameObject("Topdown : Dungeon : Grid: " + myLevel, typeof(Grid));
                GameObject newTilemap = new GameObject("Tilemap", typeof(Tilemap));
                newTilemap.GetComponent<Transform>().SetParent(newGrid.transform);

                newTilemap.AddComponent<TopdownDungeonGenerator>();
                newTilemap.GetComponent<TopdownDungeonGenerator>().SetGrid(grid[0], grid[1]);
                newGrid.GetComponent<Grid>().cellSize = new Vector3(CellSize, CellSize, 0);
                newTilemap.AddComponent<TilemapRenderer>();

                newTilemap.GetComponent<TopdownDungeonGenerator>().Floor = obj_Water as TileBase;
                newTilemap.GetComponent<TopdownDungeonGenerator>().Walls = obj_Shore as TileBase;
                newTilemap.GetComponent<Tilemap>().animationFrameRate = 60f;
            }
        }

        GUI.enabled = true;

        //
        GUILayout.EndScrollView();
        
    }


}
