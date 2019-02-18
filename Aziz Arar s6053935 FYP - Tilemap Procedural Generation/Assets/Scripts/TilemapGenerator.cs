using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour {

    [SerializeField] private int gridX = 32;
    [SerializeField] private int gridY = 32;
    private Tilemap thisMap;
    [SerializeField] private TileBase thisBase;
    [SerializeField] private TileBase Water;
    [SerializeField] private TileBase Grass;
    [SerializeField] private TileBase Mountain;

    private float seed = 0;
    bool isGenerated = false;

    // Use this for initialization

    private void Awake()
    {
        thisMap = GetComponent<Tilemap>();
    }
    void Start () {
        PerlinNoise();
    }
	
    public void SetGrid(int i, int j)
    {
        gridX = i;
        gridY = j;
    }
	// Update is called once per frame
	void Update () {

	}

    void PerlinNoise()
    {
        Vector3Int[] positions = new Vector3Int[gridX * gridY];
        TileBase[] tileArray = new TileBase[positions.Length];
        seed = (int)Random.Range(0f, 32f);
        for (int index = 0; index < positions.Length; index++)
        {
            
            //Debug.Log(seed);

            positions[index] = new Vector3Int(index % gridX, index / gridY, 0);
            float height = Mathf.PerlinNoise((float)positions[index].x / 10, (float)positions[index].y / 10);// + seed;


            tileArray[index] = thisBase;

            if(height >.5f)
            {
                tileArray[index] = Grass;
            }
            else if(height >.4f)
            {
                tileArray[index] = thisBase;
            }
            else
            {
                tileArray[index] = Water;
            }

            //if (height < 2f)
            //{
            //    tileArray[index] = Water;
            //}
            //else if (height >= 2f || height < 4f)
            //{
            //    tileArray[index] = Grass;
            //}
            //else if (height >= 4f)
            //{
            //    tileArray[index] = Mountain;
            //}
            //else
            //{
            //    //rip
            //}


        }
        thisMap.SetTiles(positions, tileArray);
    }


}
