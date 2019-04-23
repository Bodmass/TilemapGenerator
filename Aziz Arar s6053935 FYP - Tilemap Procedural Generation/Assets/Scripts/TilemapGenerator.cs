using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{ 


    [SerializeField] protected int gridX = 32;
    [SerializeField] protected int gridY = 32;
    protected TileBase[] tileArray;
    //protected List<TileBase> tiles = new List<TileBase>();
    protected Vector3Int[] positions;
    protected Tilemap thisMap;
    protected int arrayLength;
    public bool GenerateCollisionLayer = false;
    protected List<Rect> CorridorList = new List<Rect>();
    public float animationFramerate = 60;

    public virtual void Regenerate()
    {

    }
    protected virtual void GenerateCollisions()
    {
    }
    public void SetGrid(int i, int j)
    {
        gridX = i;
        gridY = j;
    }

    public Vector2 getGridSize()
    {
        return new Vector2(gridX, gridY);
    }

    //public virtual List<TileBase> GetTileList()
    //{
    //    return tiles;
    //}
}
