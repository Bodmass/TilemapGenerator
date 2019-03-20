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

public class TopdownWorldMapGenerator : MonoBehaviour {
    [Header("Grid Layout")]
    [SerializeField] private int gridX = 32;
    [SerializeField] private int gridY = 32;
    private Tilemap thisMap;
    [Header("Tiles")]
    [SerializeField] public TileBase Shore;
    [SerializeField] public TileBase Water;
    [SerializeField] public TileBase Grass;
    [SerializeField] public TileBase Mountain;
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
    private int arrayLength;
    private TileBase[] tileArray;
    private Vector3Int[] positions;
    private float[] heights;

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
	
    public void SetGrid(int i, int j)
    {
        gridX = i;
        gridY = j;
    }
	// Update is called once per frame


    public void Regenerate()
    {
        PerlinNoise();
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
