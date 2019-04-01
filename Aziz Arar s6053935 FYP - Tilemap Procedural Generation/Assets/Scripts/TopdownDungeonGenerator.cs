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
    [SerializeField] public TileBase Test;
    [Header("BSP Settings")]
    [SerializeField] private float Min_Leaf_Size = 5;
    [SerializeField] private float Max_Leaf_Size = 20;
    public List<Rect> CorridorList = new List<Rect>();

    public int minRoomSize, maxRoomSize;

    public class SubDungeon
    {
        public SubDungeon left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);
        public int debugId;



        public Rect GetRoom()
        {
            if (IAmLeaf())
            {
                return room;
            }
            if (left != null)
            {
                Rect lroom = left.GetRoom();
                if (lroom.x != -1)
                    return lroom;
            }
            if (right != null)
            {
                Rect rroom = right.GetRoom();
                if (rroom.x != -1)
                    return rroom;
            }

            return new Rect(-1, -1, 0, 0);
        }

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
            if (!IAmLeaf())
            {
                return false;
            }

            bool splitH;
            if (rect.width / rect.height >= 1.25)
            {
                splitH = false;
            }
            else if (rect.height / rect.width >= 1.25)
            {
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

    public void ConnectRooms(SubDungeon left, SubDungeon right)
    {
        Rect lroom = left.GetRoom();
        Rect rroom = right.GetRoom();


        Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
        Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

        if (lpoint.x > rpoint.x) //Switch the lowest point to the left
        {
            Vector2 temp = lpoint;
            lpoint = rpoint;
            rpoint = temp;
        }

        int w = (int)(lpoint.x - rpoint.x);
        int h = (int)(lpoint.y - rpoint.y);

        List<Rect> newCorridor = new List<Rect>();

        if (w != 0)
        {


            if (Random.Range(0, 1) > 2)
            {

                newCorridor.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs(w) + 1, 1));

                if (h < 0)
                {

                    newCorridor.Add(new Rect(rpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                }
                else
                {

                    newCorridor.Add(new Rect(rpoint.x, lpoint.y, 1, -Mathf.Abs(h)));
                }

            }

            else
            {
                if (h < 0)
                {

                    newCorridor.Add(new Rect(lpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                }
                else
                {

                    newCorridor.Add(new Rect(lpoint.x, rpoint.y, 1, Mathf.Abs(h)));
                }
            }

            newCorridor.Add(new Rect(lpoint.x, rpoint.y, Mathf.Abs(w) + 1, 1));

        }

        else
        {
            if (h < 0)
            {
                //thisMap.SetTile(new Vector3Int((int)lpoint.x, (int)lpoint.y, 0), Test);
                newCorridor.Add(new Rect((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(h)));
            }
            else
            {
                //thisMap.SetTile(new Vector3Int((int)rpoint.x, (int)rpoint.y, 0), Test);
                newCorridor.Add(new Rect((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(h)));
            }
        }

        foreach (Rect c in newCorridor)
        {
            for (int i = (int)c.x; i < c.xMax; i++)
            {
                for (int j = (int)c.y; j < c.yMax; j++)
                {
                    thisMap.SetTile(new Vector3Int(i, j, 0), Floor);

                }
            }
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
                    Debug.Log("Drawing Floor Tile @ " + new Vector2Int(i, j));

                }
            }

        }
        else
        {
            DrawRooms(subDungeon.left);
            DrawRooms(subDungeon.right);

            if(subDungeon.left !=null && subDungeon.right != null)
                ConnectRooms(subDungeon.left, subDungeon.right);
        }
    }

    public void DrawCorridors(SubDungeon subdungeon)
    {

        if (subdungeon == null)
        {
            return;
        }

        


        DrawCorridors(subdungeon.left);
        DrawCorridors(subdungeon.right);

        foreach (Rect corridor in CorridorList)
        {
            for (int i = (int)corridor.x; i < corridor.xMax; i++)
            {
                for (int j = (int)corridor.y; j < corridor.yMax; i++)
                {
                    thisMap.SetTile(new Vector3Int(i, j, 0), Floor);
                    Debug.Log("Drawing Corridor Tile @ " + new Vector2Int(i, j));
                }
            }
        }
    }
    // Use this for initialization
    void Start()
    {
        CorridorList = new List<Rect>();
        thisMap = GetComponent<Tilemap>();
        arrayLength = gridX * gridY;
        positions = new Vector3Int[arrayLength];

        tileArray = new TileBase[arrayLength];
        SetupDungeon();
        SubDungeon rootSubDungeon = new SubDungeon(new Rect(0, 0, gridX, gridY));
        CreateBSP(rootSubDungeon);
        rootSubDungeon.CreateRoom();
        DrawRooms(rootSubDungeon);

        //DrawCorridors(rootSubDungeon);

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

    public void SetGrid(int i, int j)
    {
        gridX = i;
        gridY = j;
    }

    public void Regenerate()
    {
        //   BSP();
    }

    //http://www.rombdn.com/blog/2018/01/12/random-dungeon-bsp-unity/
}

