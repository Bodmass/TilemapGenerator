using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{ 
    /*
    This is ase Class for Tilemap Generation Scripts

    The following variable are avaliable for all scripts which inherit from this.

    The scripts which currently inherit from this are:

    PlatformerGenerator.cs
    TopdownDungeonGenerator.cs
    TopdownWorldMapGenerator.cs

    */
    [SerializeField] protected int gridX = 32;
    [SerializeField] protected int gridY = 32;
    protected TileBase[] tileArray;
    protected Vector3Int[] positions;
    protected Tilemap thisMap;
    protected int arrayLength;
    public bool GenerateCollisionLayer = false;

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

}
