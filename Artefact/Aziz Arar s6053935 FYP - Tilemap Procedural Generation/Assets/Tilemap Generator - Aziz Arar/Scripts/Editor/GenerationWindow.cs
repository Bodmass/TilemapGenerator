using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class GenerationWindow : EditorWindow
{

    /*
    This script contains all the information to generate a level. Inherited from Editor Window, From Labels to Input Fields, it allows an easy set up of an Editor Window. 
     
     */
    private int i_CreationModeSelected = 0;
    private int i_PerspectiveSelected = 0;
    private int i_LevelSelected = 0;
    private int[] i_Grid = new int[] { 32, 32 };
    private int i_CellSize = 1;
    private int i_PixelPerUnit = 32;

    private float f_TilemapFrameRate = 60f;
    private float f_FoliageDensity = 0.2f;

    private string s_LevelName = "Untitled";
    private static string[] s_CreationModeOptions = new string[] { "Simple", "Advanced" };
    private static string[] s_PerspectiveOptions = new string[] { "2D Topdown", "2D Sidescroller"};
    private static string[] s_TopdownLevelOptions = new string[] { "World Map", "Dungeon"};

    private bool b_CollisionLayer = true;
    private bool b_FoliageLayer = true;
    private bool b_GenerateWalls = true;
    private bool b_DisableGridCap = false;

    //The TileBase selection objects
    private Object obj_Tile1;
    private Object obj_Tile2;
    private Object obj_Tile3;
    private Object obj_Tile4;
    private Object obj_Tile5;

    private Vector2 v2_ScrollPos = Vector2.zero;

    [MenuItem("Aziz/Tilemap Generation")]
    public static void DisplayWindow()
    {
        GetWindow<GenerationWindow>("Tilemap Generator");
    }


    void OnGUI()
    {
        float width = this.position.width;
        float height = this.position.height;

        //Adding a scroll view so the window doesn't need to be stretched to view all the information.
        v2_ScrollPos = GUILayout.BeginScrollView(v2_ScrollPos, GUILayout.Width(width), GUILayout.Height(height));

        bool canGenerate = true;
        GUILayout.Label("Tilemap Generator\nby Aziz Arar\nv0.7\n", EditorStyles.centeredGreyMiniLabel);

        GUILayout.Label("\nCreation Mode:", EditorStyles.boldLabel);


        //Deciding the Creation Mode between Simple and Advanced

        i_CreationModeSelected = GUILayout.SelectionGrid(i_CreationModeSelected, s_CreationModeOptions, s_CreationModeOptions.Length, EditorStyles.radioButton);
        if (i_CreationModeSelected == 1)
        {
            GUILayout.Label("(*) - Advanced Options", EditorStyles.miniLabel);
            GUILayout.Label("\nWarning: You have enabled Advanced Options, Use at your own risk.", EditorStyles.miniLabel);
        }
        GUILayout.Label("\n", EditorStyles.boldLabel);

        s_LevelName = EditorGUILayout.TextField("Level Name:", s_LevelName);
        GUILayout.Label("\nTilemap Perspective:", EditorStyles.boldLabel);

        //Deciding the Perspective

        i_PerspectiveSelected = GUILayout.SelectionGrid(i_PerspectiveSelected, s_PerspectiveOptions, s_PerspectiveOptions.Length, EditorStyles.radioButton);


        //Deciding the Level Type if in Topdown

        if (i_PerspectiveSelected == 0)
        {
            GUILayout.Label("\nLevel Type:", EditorStyles.boldLabel);
            i_LevelSelected = GUILayout.SelectionGrid(i_LevelSelected, s_TopdownLevelOptions, s_TopdownLevelOptions.Length, EditorStyles.radioButton);
        }

        //The grid options for Topdown, this ensures both the gridX and gridY sizes stay the same, while keeping it between 0 and 256. 
        ///It also checks if the Grid Cap is disabled, allowing to exceed 256.

        if (i_PerspectiveSelected == 0 && i_LevelSelected != 2)
        {
            GUILayout.Label(" ", EditorStyles.miniLabel);
            if (i_CreationModeSelected == 1)
            {
                b_DisableGridCap = EditorGUILayout.Toggle("(*) Disable Grid Cap", b_DisableGridCap);
                if (b_DisableGridCap)
                {
                    GUILayout.Label("\nWarning: You have disabled the Grid Cap, this is not recommended. Use at your own risk.", EditorStyles.miniLabel);
                }
                GUILayout.Label(" ", EditorStyles.miniLabel);
            }


                i_Grid[0] = EditorGUILayout.IntField("Grid Size", i_Grid[0], EditorStyles.miniTextField);
                i_Grid[1] = i_Grid[0];
            

            if (i_Grid[0] <= 0)
            {
                i_Grid[0] = 0;
            }

            if (i_Grid[1] <= 0)
            {
                i_Grid[1] = 0;
            }

            if (i_Grid[0] <= 256 && i_Grid[1] <= 256)
            {

                if (i_Grid[0] == 0 || i_Grid[1] == 0)
                {
                    GUILayout.Label("\nError: The size of the grid must be atleast 1x1", EditorStyles.miniLabel);
                    canGenerate = false;
                }
                else
                {
                    GUILayout.Label("\nThis will generate a " + i_Grid[0] + "x" + i_Grid[1] + " grid, containing " + (i_Grid[0] * i_Grid[1]).ToString() + " cells.\n", EditorStyles.miniLabel);
                    if ((i_Grid[0] * i_Grid[1]) >= (128 * 128))
                    {
                        GUILayout.Label("\nWarning: You are exceeding " + (128 * 128) + " tiles\n this may cause lag when using animated tiles depending on your system\n", EditorStyles.miniLabel);
                    }

                }


            }
            else
            {
                if (!b_DisableGridCap)
                {
                    if (i_Grid[0] >= 256)
                    {
                        i_Grid[0] = 256;
                    }

                    if (i_Grid[1] >= 256)
                    {
                        i_Grid[1] = 256;
                    }
                }
                else
                {
                    GUILayout.Label("\nThis will generate a " + i_Grid[0] + "x" + i_Grid[1] + " grid, containing " + (i_Grid[0] * i_Grid[1]).ToString() + " cells.\n", EditorStyles.miniLabel);
                    if ((i_Grid[0] * i_Grid[1]) >= (128 * 128))
                    {
                        GUILayout.Label("\nWarning: You are exceeding " + (128 * 128) + " tiles\n this may cause lag when using animated tiles depending on your system\n", EditorStyles.miniLabel);
                    }
                }

                //GUILayout.Label("\nError: The size limit in each axis is 256!", EditorStyles.miniLabel);

            }
        }

        //The Grid Options for Sidescrolling, the gridX and gridY are seperate in this scenario. There is no maximum cap for the X axis, however in the Y 256 is the max.

        if (i_PerspectiveSelected == 1)
        {
            GUILayout.Label(" ", EditorStyles.miniLabel);
            if (i_CreationModeSelected == 1)
            {
                b_DisableGridCap = EditorGUILayout.Toggle("(*) Disable Grid Cap", b_DisableGridCap);
                if (b_DisableGridCap)
                {
                    GUILayout.Label("\nWarning: You have disabled the Grid Cap, this is not recommended. Use at your own risk.", EditorStyles.miniLabel);
                }
                GUILayout.Label(" ", EditorStyles.miniLabel);
            }


            i_Grid[0] = EditorGUILayout.IntField("Grid Size X", i_Grid[0], EditorStyles.miniTextField);
            i_Grid[1] = EditorGUILayout.IntField("Grid Size Y", i_Grid[1], EditorStyles.miniTextField);


            //i_Grid[1] = EditorGUILayout.IntField("Grid Y", i_Grid[1], EditorStyles.miniTextField);

            if (i_Grid[0] <= 0)
            {
                i_Grid[0] = 0;
            }

            if (i_Grid[1] <= 0)
            {
                i_Grid[1] = 0;
            }

            if (i_Grid[1] <= 256)
            {

                if (i_Grid[0] == 0 || i_Grid[1] == 0)
                {
                    GUILayout.Label("\nError: The size of the grid must be atleast 1x1", EditorStyles.miniLabel);
                    canGenerate = false;
                }
                else
                {
                    GUILayout.Label("\nThis will generate a " + i_Grid[0] + "x" + i_Grid[1] + " grid, containing " + (i_Grid[0] * i_Grid[1]).ToString() + " cells.\n", EditorStyles.miniLabel);
                    if (i_Grid[1] >= 65)
                    {
                        GUILayout.Label("\nWarning: You are exceeding 64 tiles in the Y\n this will cause very hilly environments. The recommended is 32.\n", EditorStyles.miniLabel);
                    }

                }


            }
            else
            {
                if (!b_DisableGridCap)
                {

                    if (i_Grid[1] >= 256)
                    {
                        i_Grid[1] = 256;
                    }
                }

            }
        }
        
        //If another option was picked. 
        //At this time Isometeric and Town has been removed as options, so you can't reach this state.

        if(i_PerspectiveSelected == 2 || (i_PerspectiveSelected == 0 && i_LevelSelected == 2))
        {
            canGenerate = false;
            GUILayout.Label("\nThis is not yet implemented, sorry.", EditorStyles.miniLabel);
        }

        //Show various Advanced Options
        if (i_CreationModeSelected == 1)
        {
            GUILayout.Label("\nTilemap Options:", EditorStyles.boldLabel);
            i_CellSize = EditorGUILayout.IntField("(*) Cell Size", i_CellSize, EditorStyles.miniTextField);
            i_PixelPerUnit = EditorGUILayout.IntField("(*) Pixels Per Unit", i_PixelPerUnit, EditorStyles.miniTextField);
            //GUILayout.Label("Do not change unless you know what you are doing. The default is the same as the demo Tile Pallete.", EditorStyles.miniTextField);
            f_TilemapFrameRate = EditorGUILayout.FloatField("(*) Tilemap Frame Rate", f_TilemapFrameRate, EditorStyles.miniTextField);
            //GUILayout.Label("This affects the Frame Rate of Animated Tiles, Default is 60.", EditorStyles.miniTextField);
            GUILayout.Label("\n", EditorStyles.boldLabel);

        }

        //Layers for Collision, Walls (Dungeon only) and Foliage (Topdown World Map and Sidescrolling only).

        b_CollisionLayer = EditorGUILayout.Toggle("Generate Collision Layer", b_CollisionLayer);

        if ((i_PerspectiveSelected == 0 && i_LevelSelected == 0) || i_PerspectiveSelected == 1) 
        {
            b_FoliageLayer = EditorGUILayout.Toggle("Generate Foliage", b_FoliageLayer);
            if(b_FoliageLayer)
            {
                f_FoliageDensity = EditorGUILayout.FloatField("Foliage Density", f_FoliageDensity, EditorStyles.miniTextField);
                if(f_FoliageDensity <= 0)
                {
                    f_FoliageDensity = 0;
                }

                if (f_FoliageDensity >= 1)
                {
                    f_FoliageDensity = 1;
                }
            }
        }
        if (i_PerspectiveSelected == 0 && i_LevelSelected == 1)
        {
            b_GenerateWalls = EditorGUILayout.Toggle("Generate Walls", b_GenerateWalls);
        }

        //Tile selection for Topdown World Map

        if (i_PerspectiveSelected == 0 && i_LevelSelected == 0)
        {
            GUILayout.Label("\nDrag n' Drop the Tiles you wish to use:", EditorStyles.boldLabel);
            obj_Tile3 = EditorGUILayout.ObjectField("Grass", obj_Tile3, typeof(TileBase), true);
            obj_Tile1 = EditorGUILayout.ObjectField("Water", obj_Tile1, typeof(TileBase), true);
            obj_Tile2 = EditorGUILayout.ObjectField("Shore", obj_Tile2, typeof(TileBase), true);
            obj_Tile4 = EditorGUILayout.ObjectField("Mountain", obj_Tile4, typeof(TileBase), true);
            if (b_FoliageLayer)
                obj_Tile5 = EditorGUILayout.ObjectField("Foliage", obj_Tile5, typeof(TileBase), true);

            if (obj_Tile1 == null || obj_Tile2 == null || obj_Tile3 == null || obj_Tile4 == null)
            {

                canGenerate = false;
            }

            else
            {
                if (b_FoliageLayer)
                {
                    if (obj_Tile5 == null)
                    {
                        canGenerate = false;
                    }
                }
                else
                    canGenerate = true;
            }
        }


        //Tile selection for Topdown Dungeon


        if (i_PerspectiveSelected == 0 && i_LevelSelected == 1)
        {
            GUILayout.Label("\nDrag n' Drop the Tiles you wish to use:", EditorStyles.boldLabel);
            obj_Tile3 = EditorGUILayout.ObjectField("Floor", obj_Tile3, typeof(TileBase), true);
            obj_Tile2 = EditorGUILayout.ObjectField("BG", obj_Tile2, typeof(TileBase), true);

            if (b_GenerateWalls)
                obj_Tile1 = EditorGUILayout.ObjectField("Walls", obj_Tile1, typeof(TileBase), true);

            if (obj_Tile2 == null || obj_Tile3 == null)
            {
                canGenerate = false;
            }
            else
            {
                if (b_GenerateWalls)
                {
                    if (obj_Tile1 == null)
                    {
                        canGenerate = false;
                    }
                }
                else
                    canGenerate = true;
            }
        }


        //Tile selection for Sidescrolling

        if (i_PerspectiveSelected == 1)
        {
            GUILayout.Label("\nDrag n' Drop the Tiles you wish to use:", EditorStyles.boldLabel);
            obj_Tile3 = EditorGUILayout.ObjectField("Grass", obj_Tile3, typeof(TileBase), true);
            obj_Tile2 = EditorGUILayout.ObjectField("Dirt", obj_Tile2, typeof(TileBase), true);
            obj_Tile1 = EditorGUILayout.ObjectField("Stone", obj_Tile1, typeof(TileBase), true);
            //obj_Tile4 = EditorGUILayout.ObjectField("Water", obj_Tile4, typeof(TileBase), true);
            if (b_FoliageLayer)
                obj_Tile5 = EditorGUILayout.ObjectField("Foliage", obj_Tile5, typeof(TileBase), true);

            if (obj_Tile1 == null || obj_Tile2 == null || obj_Tile3 == null)// || obj_Tile4 == null)
            {

                canGenerate = false;
            }

            else
            {
                if (b_FoliageLayer)
                {
                    if (obj_Tile5 == null)
                    {
                        canGenerate = false;
                    }
                }
                else
                    canGenerate = true;
            }
        }

        //Checking whether the Generate button should be avaliable

        if (!canGenerate)
        {
            GUI.enabled = false;
        }

        /*
         The following code checks the different Perspective and Level types selected. 
         It adds the component for the Grid and Tilemap, Adding the specific Generation Script to the Tilemap.
         The information selected previously is sent to the Generation Script added to the Tilemap.
         */

        GUILayout.Label("\n");
        if (GUILayout.Button("Generate", EditorStyles.miniButton))
        {

            if (i_PerspectiveSelected == 0 && i_LevelSelected == 0) //World Map
            {
                Debug.Log("Topdown : World Map : Grid: " + s_LevelName + "will now generate a World Map using Perlin Noise");

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
                if (b_FoliageLayer)
                {
                    newTilemap.GetComponent<TopdownWorldMapGenerator>().Foliage = obj_Tile5 as TileBase;
                    newTilemap.GetComponent<TopdownWorldMapGenerator>().EnableFoliage(true);
                    newTilemap.GetComponent<TopdownWorldMapGenerator>().SetFoliageDensity(f_FoliageDensity);
                }
                newTilemap.GetComponent<Tilemap>().animationFrameRate = f_TilemapFrameRate;

                if (b_CollisionLayer)
                {
                    newTilemap.GetComponent<TopdownWorldMapGenerator>().GenerateCollisionLayer = true;
                }
            }
            if (i_PerspectiveSelected == 0 && i_LevelSelected == 1) //Dungeon
            {
                Debug.Log("Topdown: Dungeon : Grid: " + s_LevelName + "will now generate a Dungeon using Binary Space Partitioning");

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
                newTilemap.GetComponent<Tilemap>().animationFrameRate = f_TilemapFrameRate;

                if (b_CollisionLayer)
                {
                    newTilemap.GetComponent<TopdownDungeonGenerator>().GenerateCollisionLayer = true;
                }
                if (b_GenerateWalls)
                {
                    newTilemap.GetComponent<TopdownDungeonGenerator>().GenerateWalls = true;
                }
            }
            if (i_PerspectiveSelected == 1 && i_LevelSelected == 1) //Platformer
            {
                Debug.Log("Sidescrolling: Level : Grid: " + s_LevelName + "will now generate a Sidescrolling Level");// using Binary Space Partitioning");

                GameObject newGrid = new GameObject("Sidescrolling : Level : Grid: " + s_LevelName, typeof(Grid));
                GameObject newTilemap = new GameObject("Tilemap", typeof(Tilemap));
                newTilemap.GetComponent<Transform>().SetParent(newGrid.transform);

                newTilemap.AddComponent<PlatformerGenerator>();
                newTilemap.GetComponent<PlatformerGenerator>().SetGrid(i_Grid[0], i_Grid[1]);
                newGrid.GetComponent<Grid>().cellSize = new Vector3(i_CellSize, i_CellSize, 0);
                newTilemap.AddComponent<TilemapRenderer>();

                newTilemap.GetComponent<PlatformerGenerator>().Stone = obj_Tile1 as TileBase;
                newTilemap.GetComponent<PlatformerGenerator>().Dirt = obj_Tile2 as TileBase;
                newTilemap.GetComponent<PlatformerGenerator>().Grass = obj_Tile3 as TileBase;
                //newTilemap.GetComponent<PlatformerGenerator>().Grass = obj_Tile1 as TileBase;
                newTilemap.GetComponent<Tilemap>().animationFrameRate = f_TilemapFrameRate;

                if (b_CollisionLayer)
                {
                    newTilemap.GetComponent<PlatformerGenerator>().GenerateCollisionLayer = true;
                }

                if (b_FoliageLayer)
                {
                    newTilemap.GetComponent<TopdownWorldMapGenerator>().Foliage = obj_Tile5 as TileBase;
                    newTilemap.GetComponent<TopdownWorldMapGenerator>().EnableFoliage(true);
                    newTilemap.GetComponent<TopdownWorldMapGenerator>().SetFoliageDensity(f_FoliageDensity);
                }
            }
        }
        GUILayout.Label("\n");

        //Re-enabling buttons for newly created objects such as the Reset Button, incase it is disabled by 'canGenerate'
        //Resets all values to default
        GUI.enabled = true;
        if (GUILayout.Button("Reset", EditorStyles.miniButton))
        {
            i_CreationModeSelected = 0;
            i_PerspectiveSelected = 0;
            i_LevelSelected = 0;
            i_Grid = new int[] { 32, 32 };
            i_CellSize = 1;
            i_PixelPerUnit = 32;

            f_TilemapFrameRate = 60f;
            f_FoliageDensity = 0.2f;

            s_LevelName = "Untitled";
            s_CreationModeOptions = new string[] { "Simple", "Advanced" };
            s_PerspectiveOptions = new string[] { "2D Topdown", "2D Platformer", "Isometric" };
            s_TopdownLevelOptions = new string[] { "World Map", "Dungeon", "Town" };

            b_CollisionLayer = true;
            b_FoliageLayer = true;
            b_GenerateWalls = true;
            b_DisableGridCap = false;



            obj_Tile1 = null;
            obj_Tile2 = null;
            obj_Tile3 = null;
            obj_Tile4 = null;
            obj_Tile5 = null;
        }


        GUI.enabled = true;

        //
        GUILayout.EndScrollView();

    }


}
