using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExampleCharacterController : MonoBehaviour
{

    public float speed = 3;
    private Rigidbody2D RB;
    // Use this for initialization
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            RB.MovePosition(new Vector2(transform.position.x + (Input.GetAxis("Horizontal") * Time.deltaTime * speed), transform.position.y + (Input.GetAxis("Vertical") * Time.deltaTime * speed)));
        }
    }
}


