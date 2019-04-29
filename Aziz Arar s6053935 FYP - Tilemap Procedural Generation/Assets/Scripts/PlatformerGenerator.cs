using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformerGenerator : TilemapGenerator
{
    [Header("Tiles")]
    [SerializeField] public TileBase Grass;
    [SerializeField] public TileBase Dirt;
    [SerializeField] public TileBase Foliage;
    [Header("Generation Settings")]
    [SerializeField] private long seed = 0;

    float heightScale = 1.0f;
    float xScale = 1.0f;

    [SerializeField] private bool GenerateFoliageLayer = false;
    [SerializeField] private float foliageDensity = 0.2f;

    List<Rect> Rects = new List<Rect>();
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
        //PerlinNoise(seed);
        //end
        PN();
        //DrawLevel();

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

    //private void GenerateNoise(int amp, int wl, int octaves, int divisor)
    //{
    //    //List<int> result = new List<int>();
    //    //int[] result = new int[octaves];

    //    for(int i = 0; i < octaves; i++)
    //    {
    //        PerlinNoise(amp, wl);
    //        amp /= divisor;
    //        wl /= divisor;
    //    }

        
    //    //Debug.Log(result.Length);
    //    ///return result;
    //}

    private void DrawLevel()
    {
        foreach(Rect r in Rects)
        {
            thisMap.SetTile(new Vector3Int((int)r.x, (int)r.y/2, 0), Grass);
            
        }
    }
    private void CombineNoise(int pl)
    {
        
    }

    private int random(long x, int range)
    {
        return (int)(((x + seed) ^ 5) % range);
    }//TEST

    public int getNoise(int x, int range)
    {
        int chunkSize = 16;

        float noise = 0;

        range /= 2;

        while (chunkSize > 0)
        {
            int chunkIndex = x / chunkSize;

            float prog = (x % chunkSize) / (chunkSize * 1f);

            float left_random = random(chunkIndex, range);
            float right_random = random(chunkIndex + 1, range);


            noise += (1 - prog) * left_random + prog * right_random;

            chunkSize /= 2;
            range /= 2;

            range = Mathf.Max(1, range);
        }

        return (int)Mathf.Round(noise);
    }//TEST

    private void PerlinNoise()
    {

        ////int x = 0,
        ////y = (int)gridY / 2,
        ////amp = 100, //amplitude
        ////wl = 100, //wavelength
        ////fq = 1 / wl; //frequency
        ////float a = Rando(),
        ////b = Rando();

        ////while (x < gridX)
        ////{
        ////    if (x % wl == 0)
        ////    {
        ////        a = b;
        ////        b = Rando();
        ////        y = (int)(gridY / 2 + a * amp);
        ////        Debug.Log("1: " + y);

        ////    }
        ////    else
        ////    {
        ////        y = (int)((gridY / 2) + interpolate(a, b, (x % wl) / wl) * amp);
        ////        Debug.Log("2: " + y);
        ////    }

        //float offsetX = (int)Random.Range(-10000f, 10000f);



        //for (int index = 0; index < arrayLength; ++index)
        //{

            
        //    positions[index] = new Vector3Int(index % (gridX), index / (gridY), 0);
        //    positions[index].x = index % gridX;
        //    positions[index].y = index / gridY;

        //    float height = heightScale * Mathf.PerlinNoise(Time.time * (((float)positions[index].x / size) + offsetX) * xScale, 0.0f);

        //    positions[index].y = (int)height;
        //    //Rects.Add(new Rect(x, y, 1, 1));
        //    //Debug.Log(new Rect(x, y, 1, 1));

        //    tileArray[index] = Grass;

        //    Debug.Log(positions[index]);

        //    //x += 1;
        //}

        //thisMap.SetTiles(positions, tileArray);

        ////Rect rect = new Rect(x, y, 1, 1);

        
        ////for (int i = 0; i < gridX; i++)
        ////{
        ////    for(int j = 0; j < gridY; j++)
        ////    {
                
        ////    }
        //}

    }

    private void PN()
    {
        //float width = dirtPrefab.transform.lossyScale.x;
        //float height = dirtPrefab.transform.lossyScale.y;

        for (int i = 0; i < gridX; i++)
        {//columns (x values
            int columnHeight = 2 + getNoise(i , gridY - 2);
            for (int j = 0; j < 0 + columnHeight; j++)
            {//rows (y values)
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
                        thisMap.SetTile(new Vector3Int(i, j, 0), Grass);
                        thisMap.SetTile(new Vector3Int(i, j-1, 0), Grass);
                    }
                }
            }

        }
    }

    private float Rando()
    {
        //Z = (A * Z + C) % M;
        //return Z / M;

        return 0;
    }

    private float interpolate(float pa, float pb, float px)
    {
        float ft = px *Mathf.PI;

        float f = (1 - Mathf.Cos(ft)) * 0.5f;

        return pa * (1 - f) + pb * f;

    }
    public override void Regenerate()
    {
        CorridorList.Clear();
        //
    }

}


