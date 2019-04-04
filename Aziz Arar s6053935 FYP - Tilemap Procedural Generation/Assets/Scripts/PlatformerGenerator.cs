using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformerGenerator : TilemapGenerator
{

    [Header("Tiles")]
    [SerializeField] public TileBase Terrain;

    // Use this for initialization
    private void Start()
    {
        arrayLength = gridX * gridY;
        positions = new Vector3Int[arrayLength];
        tileArray = new TileBase[arrayLength];

        thisMap = GetComponent<Tilemap>();


        //end
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
                if (thisMap.GetTile(new Vector3Int(i, j, 0)) == Terrain)
                {
                    collisionMap.GetComponent<Tilemap>().SetTile(new Vector3Int(i, j, 0), Terrain);
                }
            }

        collisionMap.AddComponent<TilemapCollider2D>();
        collisionMap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        collisionMap.AddComponent<Rigidbody2D>();
        collisionMap.GetComponent<Rigidbody2D>().isKinematic = true;
        collisionMap.AddComponent<CompositeCollider2D>();
    }

    public override void Regenerate()
    {
        CorridorList.Clear();
        //
    }

}


