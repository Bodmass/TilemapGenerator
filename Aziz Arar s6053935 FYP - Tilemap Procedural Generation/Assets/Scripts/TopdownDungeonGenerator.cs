using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TopdownDungeonGenerator : MonoBehaviour
{

    [Header("Grid Layout")]
    [SerializeField] private int gridX = 32;
    [SerializeField] private int gridY = 32;

    [Header("Tiles")]
    private TileBase[] tileArray;
    private Vector3Int[] positions;
    private Tilemap thisMap;
    private int arrayLength;
    [SerializeField] public TileBase Floor;
    [SerializeField] public TileBase Walls;
    [Header("BSP Settings")]
    [SerializeField] private float Min_Leaf_Size = 5;
    [SerializeField] private float Max_Leaf_Size = 20;

    public int minRoomSize, maxRoomSize;

    public class SubDungeon
    {
        public SubDungeon left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);
        public int debugId;




        public void CreateRoom()
        {
            if (left != null)
            {
                left.CreateRoom();
            }
            if (right != null)
            {
                right.CreateRoom();
            }
            if (IAmLeaf())
            {
                int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2);
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                // room position will be absolute in the board, not relative to the sub-dungeon
                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                Debug.Log("Created room " + room + " in sub-dungeon " + debugId + " " + rect);
            }
        }

               private static int debugCounter = 0;
        
        public SubDungeon(Rect mrect)
        {
            rect = mrect;
            debugId = debugCounter;
            debugCounter++;

        }

        public bool IAmLeaf()
        {
            return left == null && right == null;
        }

        public bool Split(int minRoomSize, int maxRoomSize)
        {
            if(!IAmLeaf())
            {
                return false;
            }

            bool splitH;
            if (rect.width / rect.height >= 1.25){
                splitH = false;
            }
            else if(rect.height / rect.width >= 1.25){
                splitH = true;
            }
            else
            {
                splitH = Random.Range(0.0f, 1.0f) > 0.5;
            }

            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                Debug.Log("Sub-dungeon " + debugId + " will b e a leaf");
                return false;
            }
            if (splitH)
            {
                // split so that the resulting sub-dungeons widths are not too small
                // (since we are splitting horizontally)
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(
                  new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(
                  new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;
        }
    }

    public void CreateBSP(SubDungeon subDungeon)
    {
        Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);
        if (subDungeon.IAmLeaf())
        {
            // if the sub-dungeon is too large
            if (subDungeon.rect.width > maxRoomSize
              || subDungeon.rect.height > maxRoomSize
              || Random.Range(0.0f, 1.0f) > 0.25)
            {

                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {
                    Debug.Log("Splitted sub-dungeon " + subDungeon.debugId + " in "
                      + subDungeon.left.debugId + ": " + subDungeon.left.rect + ", "
                      + subDungeon.right.debugId + ": " + subDungeon.right.rect);

                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
            }
        }
    }

    public void DrawRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null)
        {
            return;
        }
        if (subDungeon.IAmLeaf())
        {


            for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
            {
                for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
                {


                    thisMap.SetTile(new Vector3Int(i, j, 0), Floor);

                }
            }

        }
        else
        {
            DrawRooms(subDungeon.left);
            DrawRooms(subDungeon.right);
        }
    }

    // Use this for initialization
    void Start()
    {

        thisMap = GetComponent<Tilemap>();
        arrayLength = gridX * gridY;
        positions = new Vector3Int[arrayLength];

        tileArray = new TileBase[arrayLength];
        SetupDungeon();
        SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, gridX, gridY));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRoom();
        DrawRooms(rootSubDungeon);

    }

    private void SetupDungeon()
    {

        for (int index = 0; index < arrayLength; ++index)
        {
            positions[index] = new Vector3Int(index % (gridX), index / (gridY), 0);
            positions[index].x = index % gridX;
            positions[index].y = index / gridY;
            tileArray[index] = Walls;

        }
        thisMap.SetTiles(positions, tileArray);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void SetGrid(int i, int j)
    {
        gridX = i;
        gridY = j;
    }
    // Update is called once per frame


    public void Regenerate()
    {
     //   BSP();
    }

    //http://www.rombdn.com/blog/2018/01/12/random-dungeon-bsp-unity/
}

