using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Jobs;
using Unity.Collections;

public struct PerlinJob : IJobParallelFor
{
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

    private float seed = 0;
    bool isGenerated = false;

    [SerializeField] private float size = 10f;
    private int offsetX = 0;
    private int offsetY = 0;

    private PerlinJob job;
    private JobHandle handle;
    private float[] heights;
    [Header("Misc")]
    private bool GenerateFoliageLayer = true;

    // Use this for initialization

    private void Start()
    {
        arrayLength = gridX * gridY;
        positions = new Vector3Int[arrayLength];
        heights = new float[arrayLength];
        tileArray =  new TileBase[arrayLength];

        thisMap = GetComponent<Tilemap>();

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

        if (GenerateCollisionLayer)
        {
            GenerateCollisions();
        }

        if (GenerateFoliageLayer)
        {
            GenerateFoliage();
        }
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
	
	// Update is called once per frame


    public override void Regenerate()
    {
        CorridorList.Clear();
        PerlinNoise();
    }

    protected override void GenerateCollisions()
    {
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

    private void GenerateFoliage()
    {
        GameObject foliageMap = new GameObject("foliageMap", typeof(Tilemap));
        foliageMap.GetComponent<Transform>().SetParent(thisMap.GetComponentInParent<Grid>().transform);

        for (int i = 0; i < gridX; i++)
            for (int j = 0; j < gridY; j++)
            {
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) == Grass)
                {
                    foliageMap.GetComponent<Tilemap>().SetTile(new Vector3Int(i, j, 0), Foliage);
                }
            }

        foliageMap.AddComponent<TilemapRenderer>();
    }

    void PerlinNoise()
    {
        job.offsetX = (int)Random.Range(-1000f, 1000f);
        job.offsetY = (int)Random.Range(-1000f, 1000f);


        handle = job.Schedule(arrayLength, 1);

        handle.Complete();

        job.positions.CopyTo(positions);
        job.heights.CopyTo(heights);


        //seed = (int)Random.Range(0f, 32f);
        for (int index = 0; index < arrayLength; ++index)
        {

           // Debug.Log(seed);

            //positions[index] = new Vector3Int(index % (gridX), index / (gridY), 0);
            //positions[index].x = index % gridX;
            //positions[index].y = index / gridY;

            //float height = Mathf.PerlinNoise(((float)positions[index].x / size) + offsetX, ((float)positions[index].y / size) + offsetY);



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
    }

}
