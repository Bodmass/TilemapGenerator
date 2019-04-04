using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExampleCharacterController : MonoBehaviour {

    public float speed = 3;
    private Rigidbody2D RB;
	// Use this for initialization
	void Start () {
        RB = GetComponent<Rigidbody2D>();
        Spawn();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            RB.MovePosition(new Vector2(transform.position.x +  (Input.GetAxis("Horizontal") * Time.deltaTime * speed), transform.position.y + (Input.GetAxis("Vertical") * Time.deltaTime * speed)));
        }
	}

    private void Spawn()
    {
        //    TilemapGenerator grid = GameObject.FindObjectOfType<TilemapGenerator>();

        //    bool viable = false;

        //    while(!viable)
        //    {

        //        Debug.Log("Teleporting Character...");
        //        int x = (int)Random.Range(0, grid.getGridSize().x);
        //        int y = (int)Random.Range(0, grid.getGridSize().y);

        //        TilemapCollider2D collider = grid.GetComponentInChildren<TilemapCollider2D>();

        //        //if(collider == null)
        //        //{
        //        //    viable = true;
        //        //    break;
        //        //}

        //        RB.MovePosition(new Vector2(x, y));

        //        if (!GetComponent<BoxCollider2D>().IsTouching(collider))
        //        {
        //            Debug.Log("Location Acceptable");
        //            viable = true;
        //            break;
        //        }

        //    }

        //
    }
}
