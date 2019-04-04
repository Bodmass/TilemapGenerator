using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class GenerationWindow : EditorWindow
{

    string s_LevelName = "Untitled";
    int i_CreationModeSelected = 0;
    int i_PerspectiveSelected = 0;
    int i_LevelSelected = 0;
    int[] i_Grid = new int[] { 32, 32 };
    int i_CellSize = 1;
    int i_PixelPerUnit = 32;
    static string[] s_CreationModeOptions = new string[] { "Simple", "Advanced" };
    static string[] s_PerspectiveOptions = new string[] { "2D Tilemap", "2D Platformer", "Isometric" };
    static string[] s_LevelOptions = new string[] { "World Map", "Dungeon", "Town" };


    private bool b_CollisionLayer = false;

    GridPalette gp_Pallete;
    Object obj_Tile1;
    Object obj_Tile2;
    Object obj_Tile3;
    Object obj_Tile4;
    Vector2 v2_scrollPos = Vector2.zero;

    //public static object[] DropZone(string title, int w, int h, TileBase p)
    //{

    //    GUILayout.Box(title, GUILayout.Width(w), GUILayout.Height(h));
    //    EventType eventType = Event.current.type;
    //    bool isAccepted = false;

    //    if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
    //    {
    //        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            

    //        if (eventType == EventType.DragPerform)
    //        {
    //            DragAndDrop.AcceptDrag();
    //            isAccepted = true;
    //            //DragAndDrop.
    //            p = DragAndDrop.objectReferences.GetValue(0) as TileBase;
    //        }
    //        Event.current.Use();


    //    }

    //    return isAccepted ? DragAndDrop.objectReferences : null;
    //}

    [MenuItem("Aziz/Tilemap Generation")]
    public static void DisplayWindow()
    {
        GetWindow<GenerationWindow>("Tilemap Generator");
    }


    void OnGUI ()
    {
        float width = this.position.width;
        float height = this.position.height;

        //float viewWidth = 1024;
        //float viewHeight = 768;

        v2_scrollPos = GUILayout.BeginScrollView(v2_scrollPos, GUILayout.Width(width), GUILayout.Height(height));

        //
        bool canGenerate = true;
        GUILayout.Label("Tilemap Generator\nby Aziz Arar\nv0.27\n", EditorStyles.centeredGreyMiniLabel);

        GUILayout.Label("\nCreation Mode:", EditorStyles.boldLabel);

        i_CreationModeSelected = GUILayout.SelectionGrid(i_CreationModeSelected, s_CreationModeOptions, s_CreationModeOptions.Length, EditorStyles.radioButton);
        if(i_CreationModeSelected == 1)
        {
            GUILayout.Label("(*) - Advanced Options", EditorStyles.miniLabel);
        }
        GUILayout.Label("\n", EditorStyles.boldLabel);

        //EditorGUI.Toggle(rect, gUIContent, false);#
        s_LevelName = EditorGUILayout.TextField("Level Name:", s_LevelName);
        GUILayout.Label("\nTilemap Perspective:", EditorStyles.boldLabel);

        i_PerspectiveSelected = GUILayout.SelectionGrid(i_PerspectiveSelected, s_PerspectiveOptions, s_PerspectiveOptions.Length, EditorStyles.radioButton);

        

        if (i_PerspectiveSelected == 0)
        {
            GUILayout.Label("\nLevel Type:", EditorStyles.boldLabel);
            i_LevelSelected = GUILayout.SelectionGrid(i_LevelSelected, s_LevelOptions, s_LevelOptions.Length, EditorStyles.radioButton);
        }

        if (i_PerspectiveSelected == 0 && i_LevelSelected != 2)
        {
            GUILayout.Label(" ", EditorStyles.miniLabel);

            i_Grid[0] = EditorGUILayout.IntField("Grid X", i_Grid[0], EditorStyles.miniTextField);
            i_Grid[1] = EditorGUILayout.IntField("Grid Y", i_Grid[1], EditorStyles.miniTextField);
            //gridY = int.Parse(GUILayout.TextField(gridY));
            if(i_Grid[0] <= 0)
            {
                i_Grid[0] = 0;
            }

            if (i_Grid[1] <= 0)
            {
                i_Grid[1] = 0;
            }

            if (i_Grid[0] <= 256 && i_Grid[1] <= 256)
            {

                if(i_Grid[0] == 0 || i_Grid[1] == 0)
                {
                    GUILayout.Label("\nError: The size of the grid must be atleast 1x1", EditorStyles.miniLabel);
                    canGenerate = false;
                }
                else
                {
                    GUILayout.Label("\nThis will generate " + (i_Grid[0] * i_Grid[1]).ToString() + " cells.\n", EditorStyles.miniLabel);
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

        if (i_CreationModeSelected == 1)
        {
            i_CellSize = EditorGUILayout.IntField("(*) Cell Size", i_CellSize, EditorStyles.miniTextField);
            i_PixelPerUnit = EditorGUILayout.IntField("(*) Pixels Per Unit", i_PixelPerUnit, EditorStyles.miniTextField);
            GUILayout.Label("Do not change unless you know what you are doing. The default is the same as the demo Tile Pallete.", EditorStyles.helpBox);

        }

        if (EditorGUILayout.Toggle("Generate Collision Layer", b_CollisionLayer, EditorStyles.toggle))
        {
            if(b_CollisionLayer)
            {
                b_CollisionLayer = false;
            }
            else
            {
                b_CollisionLayer = true;
            }
        }
        //<-- toggle Collision Layout



        //DropZone("Water", 75, 50, Water);
        //DropZone("Grass", 75, 50, Grass);
        //DropZone("Shore", 75, 50, Shore);
        //DropZone("Mountain", 75, 50, Mountain);

        if (i_PerspectiveSelected == 0 && i_LevelSelected == 0)
        {
            GUILayout.Label("\nDrag n' Drop the Tiles you wish to use:", EditorStyles.boldLabel);
            obj_Tile3 = EditorGUILayout.ObjectField("Grass", obj_Tile3, typeof(TileBase), true);
            obj_Tile1 = EditorGUILayout.ObjectField("Water", obj_Tile1, typeof(TileBase), true);
            obj_Tile2 = EditorGUILayout.ObjectField("Shore", obj_Tile2, typeof(TileBase), true);
            obj_Tile4 = EditorGUILayout.ObjectField("Mountain", obj_Tile4, typeof(TileBase), true);
        }
        if (i_PerspectiveSelected == 0 && i_LevelSelected == 1)
        {
            GUILayout.Label("\nDrag n' Drop the Tiles you wish to use:", EditorStyles.boldLabel);
            obj_Tile3 = EditorGUILayout.ObjectField("Floor", obj_Tile3, typeof(TileBase), true);
            obj_Tile2 = EditorGUILayout.ObjectField("BG", obj_Tile2, typeof(TileBase), true);
            obj_Tile1 = EditorGUILayout.ObjectField("Walls", obj_Tile1, typeof(TileBase), true);
        }


            if (!canGenerate)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Generate", EditorStyles.miniButton))
        {

            if (i_PerspectiveSelected == 0 && i_LevelSelected == 0) //World Map
            {
                Debug.Log("Generate Perlin Noise World Map");

                GameObject newGrid = new GameObject("Topdown : World Map : Grid: " + s_LevelName, typeof(Grid));
                GameObject newTilemap = new GameObject("Tilemap", typeof(Tilemap));
                newTilemap.GetComponent<Transform>().SetParent(newGrid.transform);

                newTilemap.AddComponent<TopdownWorldMapGenerator>();
                newTilemap.GetComponent<TopdownWorldMapGenerator>().SetGrid(i_Grid[0], i_Grid[1]);
                newGrid.GetComponent<Grid>().cellSize = new Vector3(i_CellSize, i_CellSize, 0);
                newTilemap.AddComponent<TilemapRenderer>();

                newTilemap.GetComponent<TopdownWorldMapGenerator>().Water = obj_Tile1 as TileBase;
                newTilemap.GetComponent<TopdownWorldMapGenerator>().Shore = obj_Tile2 as TileBase;
                newTilemap.GetComponent<TopdownWorldMapGenerator>().Grass = obj_Tile3 as TileBase;
                newTilemap.GetComponent<TopdownWorldMapGenerator>().Mountain = obj_Tile4 as TileBase;
                newTilemap.GetComponent<Tilemap>().animationFrameRate = 60f;
            }
            if (i_PerspectiveSelected == 0 && i_LevelSelected == 1) //Dungeon
            {

                Debug.Log("Generated BSP Dungeon");

                GameObject newGrid = new GameObject("Topdown : Dungeon : Grid: " + s_LevelName, typeof(Grid));
                GameObject newTilemap = new GameObject("Tilemap", typeof(Tilemap));
                newTilemap.GetComponent<Transform>().SetParent(newGrid.transform);

                newTilemap.AddComponent<TopdownDungeonGenerator>();
                newTilemap.GetComponent<TopdownDungeonGenerator>().SetGrid(i_Grid[0], i_Grid[1]);
                newGrid.GetComponent<Grid>().cellSize = new Vector3(i_CellSize, i_CellSize, 0);
                newTilemap.AddComponent<TilemapRenderer>();

                newTilemap.GetComponent<TopdownDungeonGenerator>().Wall = obj_Tile1 as TileBase;
                newTilemap.GetComponent<TopdownDungeonGenerator>().BG = obj_Tile2 as TileBase;
                newTilemap.GetComponent<TopdownDungeonGenerator>().Floor = obj_Tile3 as TileBase;
                newTilemap.GetComponent<Tilemap>().animationFrameRate = 60f;
            }
        }

        GUI.enabled = true;

        //
        GUILayout.EndScrollView();
        
    }


}
