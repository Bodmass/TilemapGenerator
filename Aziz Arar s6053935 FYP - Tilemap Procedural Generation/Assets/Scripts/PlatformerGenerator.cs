using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformerGenerator : TilemapGenerator
{
    [Header("Tiles")]
    [SerializeField] public TileBase Grass;
    [SerializeField] public TileBase Dirt;
    [SerializeField] public TileBase Stone;
    [SerializeField] public TileBase Foliage;
    [Header("Generation Settings")]
    [SerializeField] private long seed = 0;
    [SerializeField] private int StoneDepth = 4;
    [SerializeField] private int MaxHillSteepness = 2;
    [SerializeField] int PixelsPerChunk = 32;

    [SerializeField] private bool GenerateFoliageLayer = false;
    [SerializeField] private float foliageDensity = 0.2f;

    // Use this for initialization
    private void Start()
    {
        arrayLength = gridX * gridY;
        positions = new Vector3Int[arrayLength];
        tileArray = new TileBase[arrayLength];

        thisMap = GetComponent<Tilemap>();
        
        if (seed == 0)
        {
            seed = Random.Range(1000000, 10000000);
        }
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
        GameObject foliageMap = new GameObject("FoliageMap", typeof(Tilemap));
        foliageMap.GetComponent<Transform>().SetParent(thisMap.GetComponentInParent<Grid>().transform);

        for (int i = 0; i < gridX; i++)
            for (int j = 0; j < gridY; j++)
            {
                if ((thisMap.GetTile(new Vector3Int(i, j, 0)) == Grass) && (thisMap.GetTile(new Vector3Int(i, j+1, 0)) == null))
                {
                    float ChanceofFoliage = Random.Range(0f, 1f);
                    if (ChanceofFoliage <= foliageDensity)
                    {
                        foliageMap.GetComponent<Tilemap>().SetTile(new Vector3Int(i, j+1, 0), Foliage);
                    }
                }
            }

        foliageMap.AddComponent<TilemapRenderer>();

    }
    protected override void GenerateCollisions()
    {
        GameObject collisionMap = new GameObject("CollisionMap", typeof(Tilemap));
        collisionMap.GetComponent<Transform>().SetParent(thisMap.GetComponentInParent<Grid>().transform);

        for (int i = 0; i < gridX; i++)
            for (int j = 0; j < gridY; j++)
            {
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) == Grass)
                {
                    collisionMap.GetComponent<Tilemap>().SetTile(new Vector3Int(i, j, 0), Grass);
                }

            }

        collisionMap.AddComponent<TilemapCollider2D>();
        collisionMap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        collisionMap.AddComponent<Rigidbody2D>();
        collisionMap.GetComponent<Rigidbody2D>().isKinematic = true;
        collisionMap.AddComponent<CompositeCollider2D>();
    }

    private void PerlinNoise()
    {


        for (int i = 0; i < gridX; i++)
        {
            //For every tile in the X axis, calculate the noise.
            float n = 0;
            int range = (gridY - 2) / 2;
            int PPC = PixelsPerChunk;

            while (PPC > 0)
            {
                int chunkIndex = i / PPC;

                float progress = (i % PPC) / (PPC * 1f);

                float leftChunk = (int)(((chunkIndex + seed) ^ 5) % range);
                float rightChunk = (int)((((chunkIndex + 1) + seed) ^ 5) % range);


                n += (1 - progress) * leftChunk + progress * rightChunk;

                PPC /= 2;
                range /= 2;

                range = Mathf.Max(1, range);
            }

            int Noise = MaxHillSteepness + (int)Mathf.Round(n);

            for (int j = 0; j < 0 + Noise; j++)
            {
                thisMap.SetTile(new Vector3Int(i, j, 0), Dirt);
            }
        }

        for (int i = 0; i < gridX; i++)
        {
           for(int j = 0; j < gridY; j++)
            {
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) != Grass)
                    {
                    if ((thisMap.GetTile(new Vector3Int(i, j - 1, 0)) == Dirt) && (thisMap.GetTile(new Vector3Int(i, j + 1, 0)) == null))
                    {
                        //Make the grass appear on the top most layer
                        thisMap.SetTile(new Vector3Int(i, j, 0), Grass);
                       
                    }

                    bool Stonetrue = true;

                    //After a certain amount of tiles determined by StoneDepth, start generatin gsy
                    for (int s = 0; s < StoneDepth; s++)
                    {
                        if (thisMap.GetTile(new Vector3Int(i, j + s, 0)) != Dirt)
                        {
                            Stonetrue = false;
                        }
                    }

                    if (Stonetrue)
                    {
                        thisMap.SetTile(new Vector3Int(i, j, 0), Stone);
                    }
                }
            }

        }
    }

    public override void Regenerate()
    {
        //
    }

}


