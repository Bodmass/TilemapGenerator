using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using Unity.Collections;

public struct PerlinJob : IJobParallelFor
{
    //Perlin Job for Multithreading
    public float size;
    public int gridX;
    public int gridY;
    public int offsetX;
    public int offsetY;


    public NativeArray<Vector3Int> positions;
    public NativeArray<float> heights;
    public void Execute(int i)
    {
        Vector3Int pos = positions[i];
        pos.x = i % gridX;
        pos.y = i / gridY;

        //Running a 2D Perlin Noise function, offset by Random.Range run before the job is scheduled
        float height = Mathf.PerlinNoise(((float)pos.x / size) + offsetX, ((float)pos.y / size) + offsetY);

        positions[i] = pos;
        heights[i] = height;
    }
}

public class TopdownWorldMapGenerator : TilemapGenerator{

    [Header("Tiles")]
    [SerializeField] public TileBase Shore;
    [SerializeField] public TileBase Water;
    [SerializeField] public TileBase Grass;
    [SerializeField] public TileBase Mountain;
    [SerializeField] public TileBase Foliage;

    [Header("Perlin Settings")]
    public float shoreheight = .45f;
    public float grassheight = .5f;

    private int seed = 0;
    bool isGenerated = false;

    private float size = 20f;
    private int offsetX = 0;
    private int offsetY = 0;

    private PerlinJob job;
    private JobHandle handle;
    private float[] heights;
    [Header("Misc")]
    [SerializeField] private bool GenerateFoliageLayer = false;
    [SerializeField] private float foliageDensity = 0.2f;

    // Use this for initialization

    private void Start()
    {
        arrayLength = gridX * gridY;
        positions = new Vector3Int[arrayLength];
        heights = new float[arrayLength];
        tileArray =  new TileBase[arrayLength];

        thisMap = GetComponent<Tilemap>();

        //Setting up a New Perlin Job and sending it the data from the Generation Window
        job = new PerlinJob
        {
            size = size,
            offsetX = offsetX,
            offsetY = offsetY,
            gridX = gridX,
            gridY = gridY,
            heights = new NativeArray<float>(arrayLength, Allocator.Persistent),
            positions = new NativeArray<Vector3Int>(arrayLength, Allocator.Persistent)
        };

        PerlinNoise();


    }

    private void OnDisable()
    {
        handle.Complete();
        job.heights.Dispose();
        job.positions.Dispose();
    }

    void Update ()
    {
        
    }


    public override void Regenerate()
    {
        Debug.Log("Regenerating...");
        if (GenerateCollisionLayer)
        {
            GameObject.Destroy(GameObject.Find("CollisionMap"));
        }

        if (GenerateFoliageLayer)
        {
            GameObject.Destroy(GameObject.Find("FoliageMap"));

        }

        PerlinNoise(); //We can just re-run the Perlin Noise as there will always be a tile which replaces what is currently down. 
        //This means we don't need to delete the entire Tilemap to regenerate, as long as the grid size remains the same.

        FixWater();
    }

    private void FixWater()
    {
        //for (int i = 0; i <gridX; i++)
        //    for (int j = 0; j < gridY; j++)
        //    {
        //        if (thisMap.GetTile(new Vector3Int(i, j, 0)) == Water)
        //        {

        //            TileBase tile = thisMap.GetTile(new Vector3Int(i, j, 0));

        //           tile.
        //    }

    }

    protected override void GenerateCollisions()
    {
        /*
         Adding a TilemapCollider with a CompositeCollider, which requires a Rigidbody which we can set to Kinematic.
         This allows the creation of a collision map which automatically connects colliders inside or beside it, creating one big collider.
         */
        GameObject collisionMap = new GameObject("CollisionMap", typeof(Tilemap));
        collisionMap.GetComponent<Transform>().SetParent(thisMap.GetComponentInParent<Grid>().transform);

        for (int i = 0; i < gridX; i++)
            for (int j = 0; j < gridY; j++)
            {
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) == Water)
                {
                    collisionMap.GetComponent<Tilemap>().SetTile(new Vector3Int(i, j, 0), Mountain);
                }
            }

        collisionMap.AddComponent<TilemapCollider2D>();
        collisionMap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        collisionMap.AddComponent<Rigidbody2D>();
        collisionMap.GetComponent<Rigidbody2D>().isKinematic = true;
        collisionMap.AddComponent<CompositeCollider2D>();
    }

    public void EnableFoliage(bool yn)
    {
        GenerateFoliageLayer = yn;
    }

    public void SetFoliageDensity(float density)
    {
        foliageDensity = density;

        if (foliageDensity > 1)
            foliageDensity = 1;
        if (foliageDensity < 0)
            foliageDensity = 0;
    }

    private void GenerateFoliage()
    {
        //Making Foliage on a seperate Tilemap layer
        GameObject foliageMap = new GameObject("FoliageMap", typeof(Tilemap));
        foliageMap.GetComponent<Transform>().SetParent(thisMap.GetComponentInParent<Grid>().transform);

        for (int i = 0; i < gridX; i++)
            for (int j = 0; j < gridY; j++)
            {
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) == Grass)
                {
                    float ChanceofFoliage = Random.Range(0f, 1f);
                    if (ChanceofFoliage <= foliageDensity)
                    {
                        foliageMap.GetComponent<Tilemap>().SetTile(new Vector3Int(i, j, 0), Foliage);
                    }
                }
            }

        foliageMap.AddComponent<TilemapRenderer>();

    }

    void PerlinNoise()
    {
        //Randomizing position on the Perlin Noise
        job.offsetX = (int)Random.Range(-10000f, 10000f);
        job.offsetY = (int)Random.Range(-10000f, 10000f);


        handle = job.Schedule(arrayLength, 1);

        handle.Complete();

        //Grabbing the height information from the Perlin Job
        job.positions.CopyTo(positions);
        job.heights.CopyTo(heights);

        //Setting the Tiles depending on the height
        for (int index = 0; index < arrayLength; ++index)
        {

            if (heights[index] > grassheight)
            {
                tileArray[index] = Grass;
            }
            else if(heights[index] > shoreheight)
            {
                tileArray[index] = Shore;
            }
            else
            {
                tileArray[index] = Water;
            }
        }

        thisMap.SetTiles(positions, tileArray);

        if (GenerateCollisionLayer)
        {
            GenerateCollisions();
        }

        if (GenerateFoliageLayer)
        {
            GenerateFoliage();
        }
    }


}
