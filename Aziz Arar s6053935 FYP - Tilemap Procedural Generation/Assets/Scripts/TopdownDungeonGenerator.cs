using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TopdownDungeonGenerator : TilemapGenerator
{


    [Header("Tiles")]
    [SerializeField] public TileBase Floor;
    [SerializeField] public TileBase BG;
    [SerializeField] public TileBase Wall;

    [Header("BSP Settings")]
    public int minRoomSize = 5;
    public int maxRoomSize = 20;

    

    public class Leaf
    {
        public Leaf left, right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);




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

            }
        }

        public Leaf(Rect mrect)
        {
            rect = mrect;

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

                return false;
            }
            if (splitH)
            {
                // split so that the resulting sub-dungeons widths are not too small
                // (since we are splitting horizontally)
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                left = new Leaf(new Rect(rect.x, rect.y, rect.width, split));
                right = new Leaf(
                  new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                left = new Leaf(new Rect(rect.x, rect.y, split, rect.height));
                right = new Leaf(
                  new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;
        }
    }

    public void ConnectRooms(Leaf left, Leaf right)
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

                CorridorList.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs(w) + 1, 1));

                if (h < 0)
                {

                    CorridorList.Add(new Rect(rpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                }
                else
                {

                    CorridorList.Add(new Rect(rpoint.x, lpoint.y, 1, -Mathf.Abs(h)));
                }

            }

            else
            {
                if (h < 0)
                {

                    CorridorList.Add(new Rect(lpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                }
                else
                {

                    CorridorList.Add(new Rect(lpoint.x, rpoint.y, 1, Mathf.Abs(h)));
                }
            }

            CorridorList.Add(new Rect(lpoint.x, rpoint.y, Mathf.Abs(w) + 1, 1));

        }

        else
        {
            if (h < 0)
            {

                CorridorList.Add(new Rect((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(h)));
            }
            else
            {

                CorridorList.Add(new Rect((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(h)));
            }
        }



    }
    public void CreateBSP(Leaf subDungeon)
    {

        if (subDungeon.IAmLeaf())
        {
            // if the sub-dungeon is too large
            if (subDungeon.rect.width > maxRoomSize
              || subDungeon.rect.height > maxRoomSize
              || Random.Range(0.0f, 1.0f) > 0.25)
            {

                if (subDungeon.Split(minRoomSize, maxRoomSize))
                {

                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);


                }
            }
        }
    }

    public void DrawRooms(Leaf subDungeon)
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

    public void DrawCorridors()
    {
        foreach (Rect c in CorridorList)
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
    // Use this for initialization
    void Start()
    {
        CorridorList = new List<Rect>();
        thisMap = GetComponent<Tilemap>();
        arrayLength = gridX * gridY;
        positions = new Vector3Int[arrayLength];
        tileArray = new TileBase[arrayLength];

        SetupDungeon();
        Leaf currenDungeon = new Leaf(new Rect(0, 0, gridX, gridY));
        CreateBSP(currenDungeon);
        currenDungeon.CreateRoom();
        DrawRooms(currenDungeon);
        DrawCorridors();


        if(GenerateCollisionLayer)
        {
            GenerateCollisions();
        }

        PlaceWalls();

    }

    private void PlaceWalls()
    {
        for (int i = 0; i < gridX; i++)
            for (int j = 0; j < gridY; j++)
            {
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) == Floor)
                {
                    if(thisMap.GetTile(new Vector3Int(i, j+1, 0)) == BG)
                    {
                        thisMap.SetTile(new Vector3Int(i, j+1, 0), Wall);
                    }
                }
            }
    }


    protected override void GenerateCollisions()
    {
        GameObject collisionMap = new GameObject("CollisionMap", typeof(Tilemap));
        collisionMap.GetComponent<Transform>().SetParent(thisMap.GetComponentInParent<Grid>().transform);
       
        for (int i = 0; i < gridX; i++)
            for(int j = 0; j < gridY; j++)
            {
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) == BG)
                {
                    collisionMap.GetComponent<Tilemap>().SetTile(new Vector3Int(i,j,0), Floor);
                }
            }

        collisionMap.AddComponent<TilemapCollider2D>();
        collisionMap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        collisionMap.AddComponent<Rigidbody2D>();
        collisionMap.GetComponent<Rigidbody2D>().isKinematic = true;
        collisionMap.AddComponent<CompositeCollider2D>();
    }
    private void SetupDungeon()
    {

        for (int index = 0; index < arrayLength; ++index)
        {
            positions[index] = new Vector3Int(index % (gridX), index / (gridY), 0);
            positions[index].x = index % gridX;
            positions[index].y = index / gridY;
            tileArray[index] = BG;

        }
        thisMap.SetTiles(positions, tileArray);
    }


    public override void Regenerate()
    {
        CorridorList.Clear();
        SetupDungeon();
        Leaf newDungeon = new Leaf(new Rect(0, 0, gridX, gridY));
        CreateBSP(newDungeon);
        newDungeon.CreateRoom();
        DrawRooms(newDungeon);
        DrawCorridors();

        if (GenerateCollisionLayer)
        {
            GameObject.Destroy(GameObject.Find("CollisionMap"));
            GenerateCollisions();
        }
    }

    //http://www.rombdn.com/blog/2018/01/12/random-dungeon-bsp-unity/
}

