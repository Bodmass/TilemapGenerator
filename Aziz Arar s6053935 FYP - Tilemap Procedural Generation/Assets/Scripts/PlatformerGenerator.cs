using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformerGenerator : TilemapGenerator
{
    [Header("Tiles")]
    [SerializeField] public TileBase Grass;
    [Header("Generation Settings")]
    [SerializeField] private long seed = 0;

    long M = 4294967296,
    // a - 1 should be divisible by m's prime factors
    A = 1664525,
    // c and m should be co-prime
    C = 1;

    float Z;

    int x = 0, y = 0;

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
            seed = Random.Range(-1000000, 1000000);
}
        //PerlinNoise(seed);
        //end
        PerlinNoise();
        DrawLevel();

        if (GenerateCollisionLayer)
        {
            GenerateCollisions();
        }
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

    
    private void PerlinNoise()
    {

        int x = 0,
        y = (int)gridY / 2,
        amp = 100, //amplitude
        wl = 100, //wavelength
        fq = 1 / wl; //frequency
        float a = Rando(),
        b = Rando();

        while (x < gridX)
        {
            if (x % wl == 0)
            {
                a = b;
                b = Rando();
                y = (int)(gridY / 2 + a * amp);
                Debug.Log("1: " + y);

            }
            else
            {
                y = (int)((gridY / 2) + interpolate(a, b, (x % wl) / wl) * amp);
                Debug.Log("2: " + y);
            }
            Rects.Add(new Rect(x, y, 1, 1));
            //Debug.Log(new Rect(x, y, 1, 1));
            x += 1;
        }

        //Rect rect = new Rect(x, y, 1, 1);

        
        //for (int i = 0; i < gridX; i++)
        //{
        //    for(int j = 0; j < gridY; j++)
        //    {
                
        //    }
        //}

    }



    private float Rando()
    {
        Z = (A * Z + C) % M;
        return Z / M;
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


