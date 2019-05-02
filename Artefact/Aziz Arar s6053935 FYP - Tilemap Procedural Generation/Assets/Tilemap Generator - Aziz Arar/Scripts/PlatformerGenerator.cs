using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformerGenerator : TilemapGenerator
{
    /*
    This script inherits from Tilemap Generator
    The information for the grid size, arrays for tileArray and positions,
    variable directly to the tilemap and information about collision and Tilemap frame rate is obtained.
    */

    [Header("Tiles")]
    [SerializeField] public TileBase Grass;
    [SerializeField] public TileBase Dirt;
    [SerializeField] public TileBase Stone;
    [SerializeField] public TileBase Foliage;
    [Header("Generation Settings")]
    [SerializeField] private long seed = 0;
    [SerializeField] private int StoneDepth = 4;
    [SerializeField] private int heightModifier = 2;
    [SerializeField] int DetailPerChunk = 32;

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
        /*
         If Foliage Generation is checked in the Tilemap Generator Window, Create a new Tilemap called "FoliageMap".
         Depending on the foliageDensity, place a Foliage tile above a grass tile if there is nothing there already.
         */
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
        /*
        Adding a TilemapCollider with a CompositeCollider, which requires a Rigidbody which we can set to Kinematic.
        This allows the creation of a collision map which automatically connects colliders inside or beside it, creating one big collider.
        Any grass, dirt or stone tiles will have a collider added to it.
        */
        GameObject collisionMap = new GameObject("CollisionMap", typeof(Tilemap));
        collisionMap.GetComponent<Transform>().SetParent(thisMap.GetComponentInParent<Grid>().transform);

        for (int i = 0; i < gridX; i++)
            for (int j = 0; j < gridY; j++)
            {
                if ((thisMap.GetTile(new Vector3Int(i, j, 0)) == Grass) || (thisMap.GetTile(new Vector3Int(i, j, 0)) == Dirt) || (thisMap.GetTile(new Vector3Int(i, j, 0)) == Stone))
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

        /*
         This is a 1D Perlin Nosie function.
         This function goes through each value in the X axis, and determines a height for the Y.
         
         The DetailPerChunk determines how much detail and smoothness is between each new chunk.
         This will affect the amount of times the function will run, the recommended is between 32 to 64
         Lower values will create more noisy steeper terrain, and Higher values will create smoother terrain, to flat.
         For each DetailPerChunk, The values of the noise is calculated, then added by the heightModifier, which changes how high or low the level generates
         */
        for (int i = 0; i < gridX; i++)
        {
            //For every tile in the X axis, calculate the noise.
            float n = 0;
            int heightReduce = (gridY - 2) / 2;
            int DPC = DetailPerChunk;

            while (DPC > 0)
            {
                int index = i / DPC;
                float progress = (i % DPC) / (DPC * 1f);
                float left = (int)(((index + seed) ^ 5) % heightReduce);
                float right = (int)((((index + 1) + seed) ^ 5) % heightReduce);
                n += (1 - progress) * left + progress * right;
                
                //Debug.Log("dpc:" + i + " progress:" + progress + " Noise:" + n + " left:"+left + " right:" +right);

                DPC /= 2;
                heightReduce /= 2;
                //ensures it doesn't drop below 1
                heightReduce = Mathf.Max(1, heightReduce);
            }

            //Debug.Log(n);

            int Noise = heightModifier + (int)Mathf.Round(n);

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
        //Regenerate a seed, set all tiles to null and rerun the Perlin Noise
        seed = Random.Range(1000000, 10000000);

        for (int i = 0; i < gridX; i++)
        {
            for (int j = 0; j < gridY; j++)
            {
                thisMap.SetTile(new Vector3Int(i, j, 0), null);
            }
        }

        PerlinNoise();
    }

}


