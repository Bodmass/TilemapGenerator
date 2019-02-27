using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour {

    [SerializeField] private int gridX = 32;
    [SerializeField] private int gridY = 32;
    private Tilemap thisMap;
    [SerializeField] private TileBase Shore;
    [SerializeField] private TileBase Water;
    [SerializeField] private TileBase Grass;
    [SerializeField] private TileBase Mountain;

    public float shoreheight = .45f;
    public float grassheight = .5f;

    private float seed = 0;
    bool isGenerated = false;

    [SerializeField] private float size = 10f;
    private int offsetX = 0;
    private int offsetY = 0;



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


    public void Regenerate()
    {
        PerlinNoise();
    }

    void PerlinNoise()
    {
        offsetX = (int)Random.Range(-1000f, 1000f);
        offsetY = (int)Random.Range(-1000f, 1000f);

        Vector3Int[] positions = new Vector3Int[gridX * gridY];
        TileBase[] tileArray = new TileBase[positions.Length];
        //seed = (int)Random.Range(0f, 32f);
        for (int index = 0; index < positions.Length; index++)
        {

            //Debug.Log(seed);

            //positions[index] = new Vector3Int(index % (gridX), index / (gridY), 0);
            positions[index].x = index % gridX;
            positions[index].y = index / gridY;

            float height = Mathf.PerlinNoise(((float)positions[index].x / size) + offsetX, ((float)positions[index].y / size) + offsetY);



            if(height >grassheight)
            {
                tileArray[index] = Grass;
            }
            else if(height > shoreheight)
            {
                tileArray[index] = Shore;
            }
            else
            {
                tileArray[index] = Water;
            }


            thisMap.SetTile(positions[index], tileArray[index]);
        }
        
    }


}
